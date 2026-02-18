namespace Se4sonal.Decidr;

/// <summary>
/// Marker interface to represent a command.
/// </summary>
/// <typeparam name="TState">The state type for which the command can act upon.</typeparam>
/// <typeparam name="TEventBase">The event base-type for which all output events can be derived from.</typeparam>
public interface IDeciderCommand<in TState, out TEventBase>
    where TState : IDeciderState
    where TEventBase : IDeciderEvent<TState>
{
    /// <summary>
    /// Decide which events to emit based on the current state.
    /// </summary>
    /// <param name="state">The state which the command should decide from.</param>
    /// <returns>Zero, one or many events.</returns>
    IEnumerable<TEventBase> Decide(TState state);
}
