namespace Se4sonal.Decidr.EventStream;

public interface IStreamEvent<TId, TState, TEventBase>
    where TId : struct, IEquatable<TId>
    where TState : IDeciderState
    where TEventBase : IDeciderEvent<TState>
{
    string StreamName { get; }
    TId StreamId { get; }
    int Version { get; }
    TEventBase Data { get; }
}
