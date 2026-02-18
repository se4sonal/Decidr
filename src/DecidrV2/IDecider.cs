namespace DecidrV2;

public interface IDecider<in TState, out TEvent, in TCommand>
{
    bool IsTerminal(TState current);
    IEnumerable<TEvent> Decide(TCommand command, TState state);
}
