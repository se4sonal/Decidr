using Decidr.Examples.Cmd.Domain.Customers.State;

namespace Decidr.Examples.Cmd.Domain.Customers.Events;

public record CustomerDeletedEvent() : CustomerEvent
{
    public override Customer Evolve(Customer state)
    {
        return state with
        {
            IsDeleted = true
        };
    }
}
