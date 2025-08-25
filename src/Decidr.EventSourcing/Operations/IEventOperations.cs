using Decidr.Events;
using Decidr.Streams;

namespace Decidr.Operations;

public interface IEventOperations
{
    IAsyncEnumerable<StoredEvent<TEvent>> FetchEventsAsync<TEvent>(
        Guid streamId,
        long toVersion = 0,
        long fromVersion = 0,
        CancellationToken token = default);

    Task SaveStreamAsync<TState,TEvent,TCommand>(
        EventStream<TState,TEvent,TCommand> stream,
        CancellationToken token = default) where TState : class;
}
