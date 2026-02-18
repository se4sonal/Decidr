using Decidr.Operations;

namespace Decidr;

public interface IEventStoreSession
{
    // Properties
    IEventOperations Events { get; }

    // Methods
    IDecider<TState,TEvent,TCommand> CreateDecider<TState,TEvent,TCommand>(Type decider);
    IEvolver<TState,TEvent> CreateEvolver<TState, TEvent>(Type evolver);
}
