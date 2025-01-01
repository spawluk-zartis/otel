using CodeInstrumentation.Service.Messages;
using CodeInstrumentation.Service.Metrics;

namespace CodeInstrumentation.Service.EventHandlers;

public class DiceRolledEventHandler(ILogger<DiceRolledEventHandler> logger, RollMetrics metrics) : IHandleMessages<DiceRolledEvent>
{
    public async Task Handle(DiceRolledEvent message, IMessageHandlerContext context)
    {
        logger.LogInformation("Dice roll result: {DiceRollResult}", message.Result);

        metrics.RecordResult(message.Result);

        await Task.CompletedTask;
    }
}
