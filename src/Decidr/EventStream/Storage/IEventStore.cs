using Se4sonal.Decidr.EventStream.Mutation;

namespace Se4sonal.Decidr.EventStream.Storage;

public interface IEventStore
{
    // Constants
    const int DefaultVersion = -1;
    const int VersionStep = 1;

    // Methods - Static
    static bool ShouldSnapshot(int streamVersion, int? snapshotVersion, int? snapshotInterval)
    {
        if (snapshotInterval == null || snapshotInterval < 1)
            return false;

        return (streamVersion - (snapshotVersion ?? IEventStore.DefaultVersion)) >= snapshotInterval.Value;
    }

    // Methods
    IMutationBuilder<TId, TState, TEventBase, TCommandBase> CreateMutationBuilder<TId, TState, TEventBase, TCommandBase>(
        IStreamDecider<TId, TState, TEventBase, TCommandBase> decider)
        where TId : struct, IEquatable<TId>
        where TState : IDeciderState
        where TEventBase : IDeciderEvent<TState>
        where TCommandBase : IDeciderCommand<TState, TEventBase>;

    ValueTask<(int Version, TState State)?> TryGetSnapshotAsync<TId, TState>(
        string streamName,
        TId streamId,
        CancellationToken cancellationToken = default)
        where TId : struct, IEquatable<TId>
        where TState : IDeciderState;

    IAsyncEnumerable<IStreamEvent<TId, TState, TEventBase>> GetEventsAsync<TId, TState, TEventBase>(
        string streamName,
        TId streamId,
        int fromVersion = 0,
        int toVersion = int.MaxValue,
        CancellationToken cancellationToken = default)
        where TId : struct, IEquatable<TId>
        where TState : IDeciderState
        where TEventBase : IDeciderEvent<TState>;

    Task<int> AddEventsAsync<TId, TState, TEventBase>(
        string streamName,
        TId streamId,
        int expectedVersion,
        IList<TEventBase> events,
        CancellationToken cancellationToken = default)
        where TId : struct, IEquatable<TId>
        where TState : IDeciderState
        where TEventBase : IDeciderEvent<TState>;

    Task UpdateSnapshotAsync<TId, TState>(
        string streamName,
        TId streamId,
        int snapshotVersion,
        TState snapshotState,
        CancellationToken cancellationToken = default)
        where TId : struct, IEquatable<TId>
        where TState : IDeciderState;

    Task CommitAsync(CancellationToken cancellationToken = default);
}
