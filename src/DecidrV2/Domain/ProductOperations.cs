
namespace DecidrV2.Domain;

public partial class ProductAggregate : IEventOperations<ProductAggregate, IProductEvent, IProductCommand>
{
    public static StreamAggregate<ProductAggregate, IProductEvent, IProductCommand> StartStream(
        Guid id)
    {
        throw new NotImplementedException();
    }

    public static Task<StreamAggregate<ProductAggregate, IProductEvent, IProductCommand>> FetchForWritingAsync(
        IEventStoreSession session, 
        Guid id)
    {
        throw new NotImplementedException();
    }

    public static Task WriteToAggregate(
        IEventStoreSession session, 
        Guid id, 
        Action<StreamAggregate<ProductAggregate, IProductEvent, IProductCommand>> writing, 
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public static Task WriteToAggregate(
        IEventStoreSession session, 
        Guid id, Func<StreamAggregate<ProductAggregate, IProductEvent, IProductCommand>, Task> writing, 
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
