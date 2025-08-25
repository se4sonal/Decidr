namespace Decidr.Events;

public record StoredEvent<TData>(
    Guid Id,
    Guid StreamId,
    long Version,
    DateTimeOffset Timestamp,
    TData Data
);
