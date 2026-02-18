using Decidr.Examples.Cmd.Domain.Customers.State;

namespace Decidr.Examples.Cmd.Domain.Customers.Events;

public record CustomerRenamedEvent(
    string NewName) : CustomerEvent
{
    public override Customer Evolve(Customer state)
    {
        return state with
        {
            Name = NewName
        };
    }
}
