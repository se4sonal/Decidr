using Decidr.Exceptions;

namespace Decidr.Streams;

public class EventStream<TState,TEvent,TCommand>
    where TState : class
{
    // Fields
    private readonly List<StreamAction<TEvent,TCommand>> _changeActions = [];

    // Constructor
    public EventStream(
        Guid id,
        long expectedVersionOnServer,
        IDecider<TState,TEvent,TCommand> decider,
        IEvolver<TState,TEvent> evolver,
        TState? initialState = null)
    {
        Id = id;
        ExpectedVersionOnServer = expectedVersionOnServer;
        Decider = decider;
        Evolver = evolver;
        CurrentState = initialState ?? evolver.CreateInitial();
    }

    // Properties
    public Guid Id { get; }
    public long ExpectedVersionOnServer { get; private set; }
    public long Version => ExpectedVersionOnServer + _changeActions.Sum(x => x.Events.Count);
    public TState CurrentState { get; private set; }
    public IDecider<TState, TEvent, TCommand> Decider { get; }
    public IEvolver<TState, TEvent> Evolver { get; }
    public IReadOnlyList<StreamAction<TEvent, TCommand>> ChangeActions => _changeActions;

    // Methods
    public StreamAction<TEvent, TCommand> Evolve(
        TCommand commandData,
        Guid commandId = default)
    {
        // Initalize an action builder
        var builder = new StreamActionBuilder<TEvent, TCommand>(ExpectedVersionOnServer);

        // Set causation
        builder.SetCausation(new(
            commandId == default ? Guid.NewGuid() : commandId,
            commandData));

        // Decide events from command
        foreach (var evnt in Decider.Decide(CurrentState, commandData))
        {
            // Evolve the current state
            CurrentState = Evolver.Evolve(CurrentState, evnt);

            // Adde event to action
            builder.AddEvent(evnt);
        }

        // Build action
        var act = builder.Build();

        // Add to change list
        _changeActions.Add(act);

        // Build and  Return action
        return act;
    }

    public StreamActionBuilder<TEvent, TCommand> CreateActionBuilder()
    {
        return new(Version);
    }

    public void Evolve(StreamAction<TEvent, TCommand> streamAction)
    {
        // Ensure correct append on version
        if (streamAction.ExpectedAppendOnVersion != Version)
            throw new ConcurrencyException(streamAction.ExpectedAppendOnVersion, Version);

        // Evolve state based on action events
        foreach (var evnt in streamAction.Events)
        {
            CurrentState = Evolver.Evolve(CurrentState, evnt);
        }

        // Add to change list
        _changeActions.Add(streamAction);
    }
}
