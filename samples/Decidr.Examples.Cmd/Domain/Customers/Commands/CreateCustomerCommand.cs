using Decidr.Examples.Cmd.Domain.Customers.Events;
using Decidr.Examples.Cmd.Domain.Customers.State;

namespace Decidr.Examples.Cmd.Domain.Customers.Commands;

public record CreateCustomerCommand(
    int Id,
    string Name) : CustomerCommand
{
    public override IEnumerable<CustomerEvent> Decide(Customer state)
    {
        yield return new CustomerCreatedEvent(Id, Name);
    }
}
