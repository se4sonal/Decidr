namespace DecidrV2.Exceptions;

public class ConcurrencyException : Exception
{
    // Constructor
    public ConcurrencyException(
        long expectedVersion,
        long actualVersion) : base($"Stream version concurrency issue. Expected: {expectedVersion}, Actual: {actualVersion}")
    {
        ExpectedVersion = expectedVersion;
        ActualVersion = actualVersion;
    }

    // Properties
    public long ExpectedVersion { get; }
    public long ActualVersion { get; }
}
