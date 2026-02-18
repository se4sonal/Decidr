namespace Se4sonal.Decidr;

/// <summary>
/// Marker interface to represent a decider.
/// </summary>
/// <typeparam name="TState">The state type of the decider.</typeparam>
/// <typeparam name="TEventBase">The event base-type for which all command events can be derived from.</typeparam>
/// <typeparam name="TCommandBase">The command base-type for which all allowed commands can be derived from.</typeparam>
public interface IDecider<TState, TEventBase, TCommandBase>
    where TState : IDeciderState
    where TEventBase : IDeciderEvent<TState>
    where TCommandBase : IDeciderCommand<TState, TEventBase>
{
    /// <summary>
    /// A factory method for creating a state where now events have been applied.
    /// </summary>
    TState CreateDefault();

    /// <summary>
    /// A method to discover if the state is terminal (deleted/not-in-use/archived/..).
    /// </summary>
    bool IsTerminal(TState state);
}