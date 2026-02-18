using DecidrV2.Domain;
using DecidrV2.Domain.Commands;

namespace DecidrV2;

public class SyntaxText
{
    public void DoStuff(IEventStoreSession session)
    {
        var id = Guid.NewGuid();

        ProductAggregate.WriteToAggregate(
            session,
            id,
            agg => agg.Decide(new CreateProduct(id, "MyProduct")));

        var agg = ProductAggregate.StartStream(id);
    }
}
