using Decidr.Examples.Cmd.Domain.Customers.Events;
using Decidr.Examples.Cmd.Domain.Customers.State;
using Se4sonal.Decidr;
using System.Text.Json.Serialization;

namespace Decidr.Examples.Cmd.Domain.Customers;

[JsonDerivedType(typeof(CustomerCreatedEvent), "customer-created")]
[JsonDerivedType(typeof(CustomerRenamedEvent), "customer-renamed")]
[JsonDerivedType(typeof(CustomerDeletedEvent), "customer-deleted")]
public abstract record CustomerEvent : IDeciderEvent<Customer>
{
    public abstract Customer Evolve(Customer state);
}
