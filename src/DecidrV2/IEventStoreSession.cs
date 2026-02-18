namespace DecidrV2;

public interface IEventStoreSession
{
    IAsyncEnumerable<StoredEvent<TEvent>> GetEventsAsync<TEvent>(CancellationToken cancellationToken);
    Task SaveAsync<TEvent>(IEnumerable<StreamAction<TEvent>> actions, CancellationToken cancellationToken);
    Task SaveAsync<TEvent,TCommand>(IEnumerable<StreamAction<TEvent, TCommand>> actions, CancellationToken cancellationToken);
}
