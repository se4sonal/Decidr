namespace Se4sonal.Decidr.EventStream.Exceptions;

public class StreamVersionConflictException : Exception
{
    // Constructors
    public StreamVersionConflictException(
        int expectedVersion) : base($"Actual version does not match expected version {expectedVersion}")
    {
        ExpectedVersion = expectedVersion;
    }
    public StreamVersionConflictException(
        int expectedVersion,
        string message) : base(message)
    {
        ExpectedVersion = expectedVersion;
    }

    // Properties
    public int ExpectedVersion { get; }
}
