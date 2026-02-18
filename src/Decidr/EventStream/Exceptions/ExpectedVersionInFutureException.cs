namespace Se4sonal.Decidr.EventStream.Exceptions;

public class ExpectedVersionInFutureException : StreamVersionConflictException
{
    // Constructor
    public ExpectedVersionInFutureException(
        int expectedVersion,
        int actualVersion) : base(expectedVersion, $"The expected version is in the future. Expected: {expectedVersion}, Actual: {actualVersion}")
    {
        ActualVersion = actualVersion;
    }

    // Properties
    public int ActualVersion { get; }
}
