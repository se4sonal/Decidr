namespace Decidr.EventSourcingV2;

public interface IEventStoreSession
{
    IEventOperations Events { get; }
}
