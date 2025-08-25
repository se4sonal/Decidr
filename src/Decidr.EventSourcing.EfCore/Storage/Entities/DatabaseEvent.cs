using Decidr.Events;
using System.Text.Json;

namespace Decidr.Storage.Entities;

public record DatabaseEvent
{
    // Properties
    public Guid EventId { get; set; }
    public Guid StreamId { get; set; }
    public long Version { get; set; }
    public DateTimeOffset Timestamp { get; set; }
    public string? EventBody { get; set; }
    public byte[]? RowVersion { get; set; }

    // Methods
    public StoredEvent<TEvent> ToStoredEvent<TEvent>()
    {
        if (EventBody == null)
            throw new Exception("EventBody should not be null");

        var eventData = JsonSerializer.Deserialize<TEvent>(EventBody);
        if (eventData == null)
            throw new Exception($"Could not deserialize EventBody as {typeof(TEvent).Name}");

        return new StoredEvent<TEvent>(
            EventId,
            StreamId,
            Version,
            Timestamp,
            eventData);
    }
}
