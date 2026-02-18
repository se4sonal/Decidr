namespace Se4sonal.Decidr.EventStream;

/// <summary>
/// Marker interface to represent an event stream decider.
/// </summary>
/// <typeparam name="TId">The stream identity type.</typeparam>
public interface IStreamDecider<TId, TState, TEventBase, TCommandBase>
    : IDecider<TState, TEventBase, TCommandBase>
    where TId : struct, IEquatable<TId>
    where TState : IDeciderState
    where TEventBase : IDeciderEvent<TState>
    where TCommandBase : IDeciderCommand<TState, TEventBase>
{
    /// <summary>
    /// The name of the event stream.
    /// </summary>
    string StreamName { get; }

    /// <summary>
    /// (Optional) Specify the snapshot interval. Null means that snapshot logic is disabled.
    /// </summary>
    int? SnapshotInterval { get; }

    /// <summary>
    /// (Optional) Specify if a snapshot should be created when the state is terminal.
    /// </summary>
    bool SnapshotWhenTerminal { get; }
}
