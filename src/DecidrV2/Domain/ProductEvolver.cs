using DecidrV2.Domain.Events;

namespace DecidrV2.Domain;

public partial class ProductAggregate
{
    public static ProductAggregate CreateInitial() => new();
    public static ProductAggregate Evolve(ProductCreated e, ProductAggregate state)
    {
        state.Id = e.Id;
        state.Name = e.Name;
        return state;
    }
}
