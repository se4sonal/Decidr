namespace Se4sonal.Decidr.EventStream.Exceptions;

public class ExpectedVersionInPastException : StreamVersionConflictException
{
    // Constructor
    public ExpectedVersionInPastException(
        int expectedVersion,
        int actualVersion) : base(expectedVersion, $"The expected version is in the past. Expected: {expectedVersion}, Actual: {actualVersion}")
    {
        ActualVersion = actualVersion;
    }

    // Properties
    public int ActualVersion { get; }
}
