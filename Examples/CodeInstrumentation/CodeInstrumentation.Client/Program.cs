using CodeInstrumentation.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddOpenTelemetry()
    .WithTracing(b => b.AddSource(nameof(RollDiceClient)));

builder.Services.AddHttpClient(Options.DefaultName, c => c.BaseAddress = new("https://service"));
builder.Services.AddSingleton<RollDiceClient>();

var app = builder.Build();

app.Start();

var client = app.Services.GetRequiredService<RollDiceClient>();

await client.RollDiceAsync();