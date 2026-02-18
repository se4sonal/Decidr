namespace Se4sonal.Decidr.EventStream.Exceptions;

public class StreamNotFoundException : Exception
{
    // Constructor
    public StreamNotFoundException(
        string streamName,
        string streamId) : base($"Stream {streamName} with id {streamId} not found")
    {
        StreamName = streamName;
        StreamId = streamId;
    }

    // Properties
    public string StreamName { get; }
    public string StreamId { get; }
}
