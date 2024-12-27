using AspireShop.BasketService;
using AspireShop.BasketService.Metrics;
using AspireShop.BasketService.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddOpenTelemetry()
    .WithMetrics(metrics => metrics.AddMeter(BasketServiceMetrics.MeterName));

builder.AddRedisClient("basketcache");

builder.Services.AddGrpc();
builder.Services.AddGrpcHealthChecks();
builder.Services.AddTransient<IBasketRepository, RedisBasketRepository>();
builder.Services.AddSingleton<IBasketServiceMetrics, BasketServiceMetrics>();

var app = builder.Build();

app.MapGrpcService<BasketService>();

app.MapGrpcHealthChecksService();

app.MapDefaultEndpoints();

app.Run();
