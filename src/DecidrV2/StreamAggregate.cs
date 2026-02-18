using DecidrV2.Exceptions;
using System.Collections.Immutable;

namespace DecidrV2;

public class StreamAggregate<TState,TEvent>
{
    // Fields
    protected readonly List<StreamAction<TEvent>> _actions = [];

    // Constructor
    public StreamAggregate(
        Guid streamId,
        long serverVersion,
        TState state,
        IEvolver<TState, TEvent> evolver)
    {
        StreamId = streamId;
        ServerVersion = serverVersion;
        CurrentVersion = serverVersion;
        State = state;
        Evolver = evolver;
    }

    // Properties
    public Guid StreamId { get; }
    public long ServerVersion { get; private set; }
    public long CurrentVersion { get; private set; }
    public TState State { get; private set; }
    public IEvolver<TState, TEvent> Evolver { get; }

    // Methods
    public StreamAction<TEvent> EvolveOne(TEvent e)
    {
        var act = new StreamAction<TEvent>(StreamId, CurrentVersion, [e]);
        Evolve(act);
        return act;
    }

    public StreamAction<TEvent> EvolveMany(IEnumerable<TEvent> events)
    {
        var act = new StreamAction<TEvent>(StreamId, CurrentVersion, [.. events]);
        Evolve(act);
        return act;
    }

    public StreamAction<TEvent> EvolveMany(params TEvent[] events)
    {
        var act = new StreamAction<TEvent>(StreamId, CurrentVersion, [.. events]);
        Evolve(act);
        return act;
    }

    public StreamAction<TEvent> EvolveFrom(Action<ImmutableList<TEvent>.Builder> action)
    {
        var builder = ImmutableList.CreateBuilder<TEvent>();
        action(builder);
        var act = new StreamAction<TEvent>(StreamId, CurrentVersion, builder.ToImmutable());
        Evolve(act);
        return act;
    }

    public void Evolve(StreamAction<TEvent> action)
    {
        // Try to exit early
        if (action.Events.Count == 0)
            return;

        // Check if versions is consistent
        if (action.ServerVersion != CurrentVersion)
            throw new ConcurrencyException(action.ServerVersion, CurrentVersion);

        // Evolve the state
        foreach (var e in action.Events)
        {
            State = Evolver.Evolve(e, State);
        }

        // Add to actions
        _actions.Add(action);

        // Update current version
        CurrentVersion = action.CurrentVersion;
    }

    public StreamActionBuilder<TEvent> CreateActionBuilder()
    {
        return new StreamActionBuilder<TEvent>(StreamId, ServerVersion);
    }
}

public class StreamAggregate<TState,TEvent,TCommand> : StreamAggregate<TState,TEvent>
{
    // Constructor
    public StreamAggregate(
        Guid streamId,
        long serverVersion,
        TState state,
        IEvolver<TState, TEvent> evolver,
        IDecider<TState, TEvent, TCommand> decider) : base(streamId, serverVersion, state, evolver)
    {
        Decider = decider;
    }

    // Properties
    public IDecider<TState, TEvent, TCommand> Decider { get; }

    // Methods
    public StreamAction<TEvent,TCommand> Decide(TCommand cmd)
    {
        var act = new StreamAction<TEvent, TCommand>(
            StreamId, 
            CurrentVersion, 
            [.. Decider.Decide(cmd, State)], 
            cmd);
        Evolve(act);
        return act;
    }
}
