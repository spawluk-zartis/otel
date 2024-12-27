using Microsoft.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();
var connectionString = app.Configuration.GetConnectionString("Database");

app.UseHttpsRedirection();

app.MapGet("/roll", async (ILogger<Program> logger) =>
{
    logger.LogInformation("Executing SQL command...");
    await ExecuteSql("SELECT 1");

    logger.LogInformation("Rolling dice...");
    var random = new Random();
    var result = random.Next(0, 6) + 1;

    return result;
});

app.Run();

async Task ExecuteSql(string sql)
{
    using var connection = new SqlConnection(connectionString);
    await connection.OpenAsync();
    using var command = new SqlCommand(sql, connection);
    using var reader = await command.ExecuteReaderAsync();
}