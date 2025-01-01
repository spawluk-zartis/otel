using System.Data;
using System.Diagnostics;
using CodeInstrumentation.Service.Extensions;
using CodeInstrumentation.Service.Messages;
using CodeInstrumentation.Service.Metrics;
using Microsoft.Data.SqlClient;

var random = new Random();

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.AddSqlServerClient("database");

builder.Services.AddSingleton<RollMetrics>();
builder.Services.AddOpenTelemetry()
    .WithMetrics(b => b.AddMeter(RollMetrics.Name));

builder.ConfigureMessaging();

var app = builder.Build();

app.UseHttpsRedirection();

app.MapGet("/roll", async (ILogger<Program> logger, SqlConnection connection, IMessageSession ms) =>
{
    // Create random exception to demonstrate errors tracing
    if (random.NextDouble() < 0.1)
    {
        throw new Exception();
    }

    // Demonstrate SQL Connection tracing
    logger.LogInformation("Executing SQL command...");
    await ExecuteSql("SELECT NEWID()", connection);

    // Perform logic
    logger.LogInformation("Rolling dice...");
    var result = random.Next(0, 6) + 1;

    // Add custom property to active span
    Activity.Current?.RecordRoll(result);

    // Publish event to demonstrate NServiceBus tracing
    var options = new PublishOptions();
    // options.ContinueExistingTraceOnReceive();

    await ms.Publish<DiceRolledEvent>(@event => @event.Result = result, options);

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

    await connection.CloseAsync();
}