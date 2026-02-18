
using Se4sonal.Decidr.EventStream.Exceptions;
using Se4sonal.Decidr.EventStream.Storage;

namespace Se4sonal.Decidr.EventStream.Mutation.Strategies;

public class LoadVersionStrategy<TId, TState, TEventBase, TCommandBase> : ILoadStrategy<TId, TState>
    where TId : struct, IEquatable<TId>
    where TState : IDeciderState
    where TEventBase : IDeciderEvent<TState>
    where TCommandBase : IDeciderCommand<TState, TEventBase>
{
    // Fields
    private readonly IStreamDecider<TId, TState, TEventBase, TCommandBase> _decider;
    private readonly IEventStore _eventStore;
    private readonly TId _streamId;
    private readonly int _minimumVersion;
    private readonly int _maximumVersion;

    // Constructor
    public LoadVersionStrategy(
        IStreamDecider<TId, TState, TEventBase, TCommandBase> decider,
        IEventStore eventStore,
        TId streamId,
        int minimumVersion,
        int maximumVersion)
    {
        _decider = decider;
        _eventStore = eventStore;
        _streamId = streamId;
        _minimumVersion = minimumVersion;
        _maximumVersion = maximumVersion;
    }

    // Methods
    public async ValueTask<(TId Id, int Version, TState State, int? SnapshotVersion)> LoadAsync(
        CancellationToken cancellationToken = default)
    {
        // Try to get snapshot, if relevant
        (int Version, TState State)? snapshot = null;
        if (_decider.SnapshotInterval != null || _decider.SnapshotWhenTerminal)
        {
            snapshot = await _eventStore.TryGetSnapshotAsync<TId, TState>(
                streamName: _decider.StreamName,
                streamId: _streamId,
                cancellationToken: cancellationToken);
        }

        // Define current state, version and event query from version
        TState currentState;
        int currentVersion;
        int fromVersion;
        if (snapshot == null)
        {
            // If no snapshot, create default state
            currentState = _decider.CreateDefault();
            currentVersion = IEventStore.DefaultVersion;
            fromVersion = IEventStore.DefaultVersion + IEventStore.VersionStep;
        }
        else
        {
            // Exit if snapshot is terminal
            if (_decider.IsTerminal(snapshot.Value.State))
                throw new StreamTerminalException(_decider.StreamName, _streamId.ToString() ?? string.Empty);

            // If snapshot exists, use its state and version
            currentState = snapshot.Value.State;
            currentVersion = snapshot.Value.Version;
            fromVersion = snapshot.Value.Version + IEventStore.VersionStep;
        }

        // Evolve state with events
        var query = _eventStore.GetEventsAsync<TId, TState, TEventBase>(
            _decider.StreamName,
            _streamId,
            fromVersion,
            cancellationToken: cancellationToken);
        await foreach (var e in query)
        {
            currentState = e.Data.Evolve(currentState);
            currentVersion = e.Version;
        }

        // Check for version conflicts
        if (currentVersion == IEventStore.DefaultVersion)
        {
            throw new StreamNotFoundException(_decider.StreamName, _streamId.ToString() ?? string.Empty);
        }
        else if (currentVersion < _minimumVersion)
        {
            throw new ExpectedVersionInFutureException(_minimumVersion, currentVersion);
        }
        else if (currentVersion > _maximumVersion)
        {
            throw new ExpectedVersionInPastException(_maximumVersion, currentVersion); // Need to check this due to snapshot logic
        }

        // Exit if current state is terminal
        if (_decider.IsTerminal(currentState))
            throw new StreamTerminalException(_decider.StreamName, _streamId.ToString() ?? string.Empty);

        // Return response
        return (
            _streamId,
            currentVersion,
            currentState,
            snapshot?.Version);
    }
}
