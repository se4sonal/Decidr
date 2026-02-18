namespace DecidrV2.Domain.Events;

public record ProductCreated(
    Guid Id,
    string Name
) : IProductEvent;
