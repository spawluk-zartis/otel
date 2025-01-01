namespace CodeInstrumentation.Service.Messages;

public class DiceRolledEvent : IEvent
{
    public int Result { get; set; }
}