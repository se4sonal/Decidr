using DecidrV2.Domain.Commands;
using DecidrV2.Domain.Events;

namespace DecidrV2.Domain;

public partial class ProductAggregate
{
    public static bool IsTerminal(ProductAggregate state) => false;
    public static IEnumerable<IProductEvent> Decide(CreateProduct cmd, ProductAggregate state)
    {
        yield return new ProductCreated(cmd.Id, cmd.Name);
    }
}
