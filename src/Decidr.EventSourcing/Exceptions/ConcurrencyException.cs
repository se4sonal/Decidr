namespace Decidr.Exceptions;

public class ConcurrencyException : Exception
{
    // Constructor
    public ConcurrencyException(
        long expectedVersion,
        long actualVersion) : base($"Expected version does not match actual version. Expected: {expectedVersion}, Actual: {actualVersion}")
    {
        ExpectedVersion = expectedVersion;
        ActualVersion = actualVersion;
    }

    // Properties
    public long ExpectedVersion { get; }
    public long ActualVersion { get; }
}
