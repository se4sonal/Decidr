namespace Decidr.Streams;

public class StreamActionBuilder<TEvent,TCommand>
{
    // Fields
    private readonly List<TEvent> _events = [];

    // Constructor
    public StreamActionBuilder(long expectedAppendOnVersion)
    {
        ExpectedAppendOnVersion = expectedAppendOnVersion;
    }

    // Properties
    public long ExpectedAppendOnVersion { get; private set; }
    public CausationObject<TCommand>? Causation { get; private set; }
    public IReadOnlyList<TEvent> Events => _events;

    // Methods
    public StreamActionBuilder<TEvent,TCommand> SetCausation(CausationObject<TCommand>? causation)
    {
        Causation = causation;
        return this;
    }

    public StreamActionBuilder<TEvent,TCommand> AddEvent(TEvent evnt)
    {
        _events.Add(evnt);
        return this;
    }

    public StreamActionBuilder<TEvent, TCommand> AddEventRange(params TEvent[] evnts)
    {
        _events.AddRange(evnts);
        return this;
    }

    public StreamActionBuilder<TEvent, TCommand> AddEventRange(IEnumerable<TEvent> evnts)
    {
        _events.AddRange(evnts);
        return this;
    }

    public StreamAction<TEvent,TCommand> Build()
    {
        // Create action
        var act = new StreamAction<TEvent, TCommand>(
            ExpectedAppendOnVersion,
            Causation,
            [.. _events]);

        // Reset builder
        ExpectedAppendOnVersion = ExpectedAppendOnVersion + Events.Count;
        Causation = null;
        _events.Clear();

        // Return action
        return act;
    }
}
