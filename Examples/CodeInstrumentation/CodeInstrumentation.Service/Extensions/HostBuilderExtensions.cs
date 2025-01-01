namespace CodeInstrumentation.Service.Extensions;

internal static class HostBuilderExtensions
{
    internal static IHostApplicationBuilder ConfigureMessaging(this IHostApplicationBuilder builder)
    {
        var endpointConfiguration = new EndpointConfiguration("CodeInstrumentation.Service");

        endpointConfiguration.EnableOpenTelemetry();
        endpointConfiguration.UseSerialization<SystemJsonSerializer>();
        endpointConfiguration.UseTransport<LearningTransport>();

        builder.Services.AddOpenTelemetry()
            .WithTracing(b =>
            {
                b.AddSource("NServiceBus.Core");
            });

        builder.UseNServiceBus(endpointConfiguration);

        return builder;
    }
}
