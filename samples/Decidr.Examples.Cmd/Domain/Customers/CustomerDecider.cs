using Decidr.Examples.Cmd.Domain.Customers.State;
using Se4sonal.Decidr.EventStream;

namespace Decidr.Examples.Cmd.Domain.Customers;

public record CustomerDecider() : IStreamDecider<int, Customer, CustomerEvent, CustomerCommand>
{
    // Properties
    public string StreamName => "customer";
    public int? SnapshotInterval => 3;
    public bool SnapshotWhenTerminal => true;

    // Methods - Public
    public Customer CreateDefault()
        => new(false, default, string.Empty);

    public bool IsTerminal(Customer state)
        => state.IsDeleted;
}
