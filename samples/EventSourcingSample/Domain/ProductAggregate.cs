using Decidr.Aggregates;
using Decidr.Json;
using EventSourcingSample.Domain.State;

namespace EventSourcingSample.Domain;

public partial interface IProductEvent : IGenerateJsonDerivedType;
public partial interface IProductCommand : IGenerateJsonDerivedType;

public class ProductAggregate : IAggregateRoot<Product, IProductEvent, IProductCommand>
{
}
