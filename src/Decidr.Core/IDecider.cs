namespace Decidr;

public interface IDecider<in TState, out TEvent, in TCommand>
{
    bool IsTerminal(TState current);
    IEnumerable<TEvent> Decide(TState current, TCommand command);
}
