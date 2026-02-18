using Decidr.Examples.Cmd.Domain.Customers.State;
using Se4sonal.Decidr;

namespace Decidr.Examples.Cmd.Domain.Customers;

public abstract record CustomerCommand : IDeciderCommand<Customer, CustomerEvent>
{
    public abstract IEnumerable<CustomerEvent> Decide(Customer state);
}
