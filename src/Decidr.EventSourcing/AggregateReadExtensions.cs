using Decidr.Aggregates;

namespace Decidr;

public static class AggregateReadExtensions
{
    public static async Task<TState?> AggregateEventsAsync<TState,TEvent,TCommand>(
        this IAggregateRoot<TState, TEvent, TCommand> aggregate,
        IEventStoreSession session,
        Guid streamId,
        long toVersion = 0,
        long fromVersion = 0,
        TState? fromState = null,
        CancellationToken token = default) where TState : class
    {
        // Guards
        if (fromVersion > 0 && fromState == null) throw new InvalidOperationException("Aggregate cannot be null if FromVersion is specified");

        // Get evolver
        var evolver = session.CreateEvolver<TState, TEvent>(aggregate.GetType());

        // Fetch events in stream and evolve aggregate
        TState state = fromState ?? evolver.CreateInitial();
        await foreach (var storedEvent in session.Events.FetchEventsAsync<TEvent>(streamId, toVersion, fromVersion, token).ConfigureAwait(false))
        {
            state = evolver.Evolve(state, storedEvent.Data);
        }

        // Return result
        return state;
    }
}
