namespace Se4sonal.Decidr;

public interface IDeciderEvent<TState>
    where TState : IDeciderState
{
    /// <summary>
    /// Evolve the state based on the event.
    /// </summary>
    /// <param name="state">The state which the event should be applied to.</param>
    /// <returns>The new evolved state.</returns>
    TState Evolve(TState state);
}
