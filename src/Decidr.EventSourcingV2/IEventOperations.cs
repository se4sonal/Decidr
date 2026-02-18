namespace Decidr.EventSourcingV2;

public interface IEventOperations
{
    IEventStream<TState, TEvent, TCommand> StartStream<TState, TEvent, TCommand>();
}
