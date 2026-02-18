using System.Collections.Immutable;

namespace Decidr.Streams;

public record StreamAction<TEvent,TCommand>(
    long ExpectedAppendOnVersion,
    CausationObject<TCommand>? Causation,
    ImmutableList<TEvent> Events
);
