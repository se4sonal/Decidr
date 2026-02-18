namespace DecidrV2;

public class StreamActionBuilder<TEvent>
{
    // Fields
    protected readonly List<TEvent> _events = [];

    // Constructor
    public StreamActionBuilder(
        Guid streamId,
        long serverVersion)
    {
        StreamId = streamId;
        ServerVersion = serverVersion;
        CurrentVersion = serverVersion;
    }

    // Properties
    public Guid StreamId { get; }
    public long ServerVersion { get; protected set; }
    public long CurrentVersion { get; protected set; }
    public IReadOnlyList<TEvent> Events => _events;

    // Methods
    public void Add(TEvent e)
    {
        _events.Add(e);
        CurrentVersion = _events.Count + ServerVersion;
    }

    public StreamAction<TEvent> Build()
    {
        var act = new StreamAction<TEvent>(StreamId, ServerVersion, [.. _events]);
        _events.Clear();
        ServerVersion = act.CurrentVersion;
        CurrentVersion = act.CurrentVersion;
        return act;
    }
}
