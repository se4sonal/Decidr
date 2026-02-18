namespace DecidrV2;

public record StoredEvent<TEvent>(
    Guid StreamId,
    long Version,
    DateTimeOffset Timestamp,
    TEvent Data
);
