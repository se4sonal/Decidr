namespace Se4sonal.Decidr.EventStream.Mutation;

public interface ILoadStrategy<TId, TState>
    where TId : struct, IEquatable<TId>
    where TState : IDeciderState
{
    ValueTask<(TId Id, int Version, TState State, int? SnapshotVersion)> LoadAsync(CancellationToken cancellationToken = default);
}
