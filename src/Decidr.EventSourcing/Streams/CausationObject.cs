namespace Decidr.Streams;

public record CausationObject<T>(
    Guid Id,
    T Data);
