using Decidr.EventStream.Mutation.Strategies;
using Se4sonal.Decidr;
using Se4sonal.Decidr.EventStream;
using Se4sonal.Decidr.EventStream.Mutation;
using Se4sonal.Decidr.EventStream.Mutation.Strategies;
using Se4sonal.Decidr.EventStream.Storage;

namespace Decidr.EventStream.Mutation;

public class MutationBuilder<TId, TState, TEventBase, TCommandBase> 
    : IMutationBuilder<TId, TState, TEventBase, TCommandBase>
    , IMutationBuilderCanCommand<TId, TState, TEventBase, TCommandBase>
    , IMutationBuilderCanExecute<TId, TState, TEventBase, TCommandBase>
    where TId : struct, IEquatable<TId>
    where TState : IDeciderState
    where TEventBase : IDeciderEvent<TState>
    where TCommandBase : IDeciderCommand<TState, TEventBase>
{
    // Fields
    private readonly IStreamDecider<TId, TState, TEventBase, TCommandBase> _decider;
    private readonly IEventStore _eventStore;
    private readonly List<TCommandBase> _commands = [];
    private ILoadStrategy<TId, TState> _loadStrategy = default!;

    // Constructor
    private MutationBuilder(
        IStreamDecider<TId, TState, TEventBase,TCommandBase> decider,
        IEventStore eventStore)
    {
        _decider = decider;
        _eventStore = eventStore;
    }

    // Methods - Public - Static
    public static IMutationBuilder<TId, TState, TEventBase, TCommandBase> Create(
        IStreamDecider<TId, TState, TEventBase, TCommandBase> decider,
        IEventStore eventStore)
    {
        return new MutationBuilder<TId, TState, TEventBase, TCommandBase>(decider, eventStore);
    }

    // Methods - Public
    public IMutationBuilderCanCommand<TId, TState, TEventBase, TCommandBase> LoadDefault(TId id)
    {
        _loadStrategy = new LoadDefaultStrategy<TId, TState, TEventBase, TCommandBase>(_decider, id);
        return this;
    }

    public IMutationBuilderCanCommand<TId, TState, TEventBase, TCommandBase> LoadLast(TId id)
    {
        _loadStrategy = new LoadVersionStrategy<TId, TState, TEventBase, TCommandBase>(_decider, _eventStore, id, IEventStore.DefaultVersion, int.MaxValue);
        return this;
    }

    public IMutationBuilderCanCommand<TId, TState, TEventBase, TCommandBase> LoadSpecific(TId id, int expectedVersion)
    {
        _loadStrategy = new LoadVersionStrategy<TId, TState, TEventBase, TCommandBase>(_decider, _eventStore, id, expectedVersion, expectedVersion);
        return this;
    }

    public IMutationBuilderCanExecute<TId, TState, TEventBase, TCommandBase> AddCommand(TCommandBase command)
    {
        _commands.Add(command);
        return this;
    }

    public async ValueTask<int> SaveAsync(CancellationToken cancellationToken = default)
    {
        // Load current state
        (TId id, int version, TState state, int? snapshotVersion) = await _loadStrategy.LoadAsync(cancellationToken);

        // Invoke commands to evolve current state
        var currentState = state;
        var events = new List<TEventBase>();
        foreach (var cmd in _commands)
        {
            var cmdEvents = cmd.Decide(currentState);
            foreach (var e in cmdEvents)
            {
                events.Add(e);
                currentState = e.Evolve(currentState);
            }
        }

        // Exit early if it didn't trigger any events
        if (events.Count == 0)
        {
            return version;
        }

        // Save events to event store
        var newVersion = await _eventStore.AddEventsAsync<TId, TState, TEventBase>(
            _decider.StreamName,
            id,
            version,
            events,
            cancellationToken);

        // Save snapshot, if relevant
        var snapshotDueToInterval = _decider.SnapshotInterval != null && IEventStore.ShouldSnapshot(newVersion, snapshotVersion, _decider.SnapshotInterval);
        var snapshotDueToTerminal = _decider.SnapshotWhenTerminal && _decider.IsTerminal(currentState);
        if (snapshotDueToInterval || snapshotDueToTerminal)
        {
            await _eventStore.UpdateSnapshotAsync(
                    _decider.StreamName,
                    id,
                    newVersion,
                    currentState,
                    cancellationToken);
        }

        // Return new version
        return newVersion;
    }
}
