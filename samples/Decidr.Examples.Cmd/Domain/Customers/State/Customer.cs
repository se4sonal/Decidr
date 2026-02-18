using Se4sonal.Decidr;

namespace Decidr.Examples.Cmd.Domain.Customers.State;

public record Customer(
    bool IsDeleted,
    int Id,
    string Name) : IDeciderState;
