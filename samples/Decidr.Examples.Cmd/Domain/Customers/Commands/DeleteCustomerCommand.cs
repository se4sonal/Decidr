using Decidr.Examples.Cmd.Domain.Customers.Events;
using Decidr.Examples.Cmd.Domain.Customers.State;

namespace Decidr.Examples.Cmd.Domain.Customers.Commands;

public record DeleteCustomerCommand() : CustomerCommand
{
    public override IEnumerable<CustomerEvent> Decide(Customer state)
    {
        if (state.IsDeleted) yield break;
        yield return new CustomerDeletedEvent();
    }
}
