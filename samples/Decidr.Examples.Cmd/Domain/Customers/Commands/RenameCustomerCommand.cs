using Decidr.Examples.Cmd.Domain.Customers.Events;
using Decidr.Examples.Cmd.Domain.Customers.State;

namespace Decidr.Examples.Cmd.Domain.Customers.Commands;

public record RenameCustomerCommand(
    string NewName) : CustomerCommand
{
    public override IEnumerable<CustomerEvent> Decide(Customer state)
    {
        yield return new CustomerRenamedEvent(NewName);
    }
}
