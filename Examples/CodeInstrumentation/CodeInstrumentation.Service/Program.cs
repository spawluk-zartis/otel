using System.Data;
using System.Diagnostics;
using CodeInstrumentation.Service.Extensions;
using CodeInstrumentation.Service.Metrics;
using Microsoft.Data.SqlClient;

var random = new Random();
var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.AddSqlServerClient("database");

builder.Services.AddSingleton<RollMetrics>();
builder.Services.AddOpenTelemetry()
    .WithMetrics(b => b.AddMeter(RollMetrics.Name));

var app = builder.Build();

app.UseHttpsRedirection();

app.MapGet("/roll", async (ILogger<Program> logger, SqlConnection connection, RollMetrics metrics) =>
{
    if (random.NextDouble() < 0.1)
    {
        throw new Exception();
    }

    logger.LogInformation("Executing SQL command...");
    await ExecuteSql("SELECT 1", connection);

    logger.LogInformation("Rolling dice...");
    var result = random.Next(0, 6) + 1;

    Activity.Current?.RecordRoll(result);
    metrics.RecordResult(result);

    return result;
});

app.MapDefaultEndpoints();

app.Run();

return;

static async Task ExecuteSql(string sql, SqlConnection connection)
{
    if (connection.State != ConnectionState.Open)
    {
        await connection.OpenAsync();
    }

    await using var command = new SqlCommand(sql, connection);
    await using var reader = await command.ExecuteReaderAsync();
}