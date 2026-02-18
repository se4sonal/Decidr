namespace Se4sonal.Decidr.EventStream;

public record StreamEvent<TId, TState, TEventBase>(
    string StreamName,
    TId StreamId,
    int Version,
    TEventBase Data)
    : IStreamEvent<TId, TState, TEventBase>
    where TId : struct, IEquatable<TId>
    where TState : IDeciderState
    where TEventBase : IDeciderEvent<TState>;
