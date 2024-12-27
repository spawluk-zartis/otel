var builder = DistributedApplication.CreateBuilder(args);

var sql = builder.AddSqlServer("sqlserver")
    .WithBindMount("./mssqlconfig", "/usr/config")
    .WithBindMount("./data", "/docker-entrypoint-initdb.d")
    .WithEntrypoint("/usr/config/entrypoint.sh")
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent);

var database = sql.AddDatabase("database");

var service = builder.AddProject<Projects.CodeInstrumentation_Service>("service")
    .WithReference(database)
    .WaitFor(database)
    .WithHttpsEndpoint(port: 44342, name: "api");

builder.AddProject<Projects.CodeInstrumentation_Client>("client")
    .WithReference(service)
    .WaitFor(service);

builder.Build().Run();
