using Decidr.Examples.Cmd.Domain.Customers.State;

namespace Decidr.Examples.Cmd.Domain.Customers.Events;

public record CustomerCreatedEvent(
    int Id,
    string Name) : CustomerEvent
{
    public override Customer Evolve(Customer state)
    {
        return state with
        {
            Id = Id,
            Name = Name
        };
    }
}
