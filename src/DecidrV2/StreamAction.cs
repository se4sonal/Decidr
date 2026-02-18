using System.Collections.Immutable;

namespace DecidrV2;

public class StreamAction<TEvent>
{
    // Constructor
    public StreamAction(
        Guid streamId,
        long serverVersion,
        ImmutableList<TEvent> events)
    {
        StreamId = streamId;
        ServerVersion = serverVersion;
        CurrentVersion = events.Count + serverVersion;
        Events = events;
    }

    // Properties
    public Guid StreamId { get; }
    public long ServerVersion { get; }
    public long CurrentVersion { get; }
    public ImmutableList<TEvent> Events { get; }
}

public class StreamAction<TEvent, TCommand> : StreamAction<TEvent>
{
    // Constructor
    public StreamAction(
        Guid streamId,
        long serverVersion,
        ImmutableList<TEvent> events,
        TCommand command) : base(streamId, serverVersion, events)
    {
        Command = command;
    }

    // Properties
    public TCommand Command { get; }
}
