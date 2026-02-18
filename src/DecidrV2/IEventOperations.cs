namespace DecidrV2;

public interface IEventOperations<TState,TEvent>
{
    abstract static StreamAggregate<TState, TEvent> StartStream(Guid id);
    abstract static Task<StreamAggregate<TState, TEvent>> FetchForWritingAsync(IEventStoreSession session, Guid id, CancellationToken cancellationToken = default);
    abstract static Task WriteToAggregate(IEventStoreSession session, Guid id, Action<StreamAggregate<TState,TEvent>> writing, CancellationToken cancellationToken = default);
    abstract static Task WriteToAggregate(IEventStoreSession session, Guid id, Func<StreamAggregate<TState, TEvent>,Task> writing, CancellationToken cancellationToken = default);
}

public interface IEventOperations<TState, TEvent, TCommand>
{
    abstract static StreamAggregate<TState, TEvent, TCommand> StartStream(Guid id);
    abstract static Task<StreamAggregate<TState, TEvent,TCommand>> FetchForWritingAsync(IEventStoreSession session, Guid id);
    abstract static Task WriteToAggregate(IEventStoreSession session, Guid id, Action<StreamAggregate<TState, TEvent, TCommand>> writing, CancellationToken cancellationToken = default);
    abstract static Task WriteToAggregate(IEventStoreSession session, Guid id, Func<StreamAggregate<TState, TEvent, TCommand>, Task> writing, CancellationToken cancellationToken = default);
}