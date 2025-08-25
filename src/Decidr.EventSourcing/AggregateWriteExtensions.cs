using Decidr.Aggregates;
using Decidr.Exceptions;
using Decidr.Streams;

namespace Decidr;

public static class AggregateWriteExtensions
{
    public static EventStream<TState, TEvent, TCommand> StartStream<TState, TEvent, TCommand>(
        this IAggregateRoot<TState, TEvent, TCommand> aggregate,
        IEventStoreSession session,
        Guid id) where TState : class
    {
        var aggTyp = aggregate.GetType();
        return new EventStream<TState, TEvent, TCommand>(
            id,
            0,
            session.CreateDecider<TState, TEvent, TCommand>(aggTyp),
            session.CreateEvolver<TState, TEvent>(aggTyp),
            null);
    }

    public static async Task<EventStream<TState, TEvent, TCommand>> FetchForWriting<TState, TEvent, TCommand>(
        this IAggregateRoot<TState, TEvent, TCommand> aggregate,
        IEventStoreSession session,
        Guid id,
        CancellationToken token = default) where TState : class
    {
        var aggTyp = aggregate.GetType();

        // TODO: Consider building logic for retrieving snapshots

        // Get evolver
        var evolver = session.CreateEvolver<TState, TEvent>(aggTyp);

        // Fetch stream events to evolve current state
        var state = evolver.CreateInitial();
        long serverVersion = 0;
        await foreach (var storedEvent in session.Events.FetchEventsAsync<TEvent>(id, token: token).ConfigureAwait(false))
        {
            state = evolver.Evolve(state, storedEvent.Data);
            serverVersion = storedEvent.Version;
        }

        // Create and return event stream
        return new EventStream<TState, TEvent, TCommand>(
            id,
            serverVersion,
            session.CreateDecider<TState, TEvent, TCommand>(aggTyp),
            evolver,
            state);
    }

    public static async Task<EventStream<TState, TEvent, TCommand>> FetchForWriting<TState, TEvent, TCommand>(
        this IAggregateRoot<TState, TEvent, TCommand> aggregate,
        IEventStoreSession session,
        Guid id,
        long expectedVersion,
        CancellationToken token = default) where TState : class
    {
        // Ensure valid expected version
        if (expectedVersion < 0)
            expectedVersion = 0;

        var aggTyp = aggregate.GetType();

        // TODO: Consider building logic for retrieving snapshots

        // Get evolver
        var evolver = session.CreateEvolver<TState, TEvent>(aggTyp);

        // Fetch stream events to evolve current state
        var state = evolver.CreateInitial();
        long serverVersion = 0;
        await foreach (var storedEvent in session.Events.FetchEventsAsync<TEvent>(id, token: token).ConfigureAwait(false))
        {
            state = evolver.Evolve(state, storedEvent.Data);
            serverVersion = storedEvent.Version;
        }

        // Ensure correct expected version
        if (serverVersion != expectedVersion)
        {
            throw new ConcurrencyException(expectedVersion, serverVersion);
        }

        // Create and return event stream
        return new EventStream<TState, TEvent, TCommand>(
            id,
            serverVersion,
            session.CreateDecider<TState, TEvent, TCommand>(aggTyp),
            evolver,
            state);
    }

    public static async Task WriteToAggregate<TState, TEvent, TCommand>(
        this IAggregateRoot<TState, TEvent, TCommand> aggregate,
        IEventStoreSession session,
        Guid id,
        Action<EventStream<TState, TEvent, TCommand>> writing,
        CancellationToken token = default) where TState : class
    {
        var stream = await FetchForWriting(aggregate, session, id, token).ConfigureAwait(false);
        writing(stream);
        await session.Events.SaveStreamAsync(stream, token).ConfigureAwait(false);
    }

    public static async Task WriteToAggregate<TState, TEvent, TCommand>(
        this IAggregateRoot<TState, TEvent, TCommand> aggregate,
        IEventStoreSession session,
        Guid id,
        Func<EventStream<TState, TEvent, TCommand>, Task> writing,
        CancellationToken token = default) where TState : class
    {
        var stream = await FetchForWriting(aggregate, session, id, token).ConfigureAwait(false);
        await writing(stream);
        await session.Events.SaveStreamAsync(stream, token).ConfigureAwait(false);
    }

    public static async Task WriteToAggregate<TState, TEvent, TCommand>(
        this IAggregateRoot<TState, TEvent, TCommand> aggregate,
        IEventStoreSession session,
        Guid id,
        long expectedVersion,
        Action<EventStream<TState, TEvent, TCommand>> writing,
        CancellationToken token = default) where TState : class
    {
        var stream = await FetchForWriting(aggregate, session, id, expectedVersion, token).ConfigureAwait(false);
        writing(stream);
        await session.Events.SaveStreamAsync(stream, token).ConfigureAwait(false);
    }

    public static async Task WriteToAggregate<TState, TEvent, TCommand>(
        this IAggregateRoot<TState, TEvent, TCommand> aggregate,
        IEventStoreSession session,
        Guid id,
        long expectedVersion,
        Func<EventStream<TState, TEvent, TCommand>, Task> writing,
        CancellationToken token = default) where TState : class
    {
        var stream = await FetchForWriting(aggregate, session, id, expectedVersion, token).ConfigureAwait(false);
        await writing(stream);
        await session.Events.SaveStreamAsync(stream, token).ConfigureAwait(false);
    }
}
