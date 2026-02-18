using Se4sonal.Decidr;
using Se4sonal.Decidr.EventStream;
using Se4sonal.Decidr.EventStream.Mutation;
using Se4sonal.Decidr.EventStream.Storage;

namespace Decidr.EventStream.Mutation.Strategies;

public class LoadDefaultStrategy<TId, TState, TEventBase, TCommandBase> : ILoadStrategy<TId, TState>
    where TId : struct, IEquatable<TId>
    where TState : IDeciderState
    where TEventBase : IDeciderEvent<TState>
    where TCommandBase : IDeciderCommand<TState, TEventBase>
{
    // Fields
    private readonly IStreamDecider<TId, TState, TEventBase, TCommandBase> _decider;
    private readonly TId _streamId;

    // Constructor
    public LoadDefaultStrategy(
        IStreamDecider<TId,TState,TEventBase,TCommandBase> decider,
        TId streamId)
    {
        _decider = decider;
        _streamId = streamId;
    }

    // Methods
    public ValueTask<(TId Id, int Version, TState State, int? SnapshotVersion)> LoadAsync(
        CancellationToken cancellationToken = default)
    {
        return new((
            _streamId, 
            IEventStore.DefaultVersion, 
            _decider.CreateDefault(), 
            null));
    }
}
