using Microsoft.Extensions.Logging;
using OpenTelemetry.Trace;

namespace CodeInstrumentation.Client;

public class RollDiceClient(ILogger<RollDiceClient> logger, HttpClient httpClient, TracerProvider tracerProvider)
{
    private readonly Tracer _tracer = tracerProvider.GetTracer(nameof(RollDiceClient));

    public async Task RollDiceAsync()
    {
        while (true)
        {
            using var activity = _tracer.StartActiveSpan("Roll dice");

            try
            {
                var content = await httpClient.GetStringAsync("/roll");

                logger.LogInformation("Roll result is: {RollResult}", content);

                activity?.SetStatus(Status.Ok);
            }
            catch (HttpRequestException ex)
            {
                logger.LogError(ex, "Could not get roll result");
                activity?.SetStatus(Status.Error);
            }

            using var delayActivity = _tracer.StartActiveSpan("Roll delay");
            Thread.Sleep(1000);
        }
    }
}
