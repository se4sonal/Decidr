namespace DecidrV2.Domain.Commands;

public record CreateProduct(
    Guid Id,
    string Name
) : IProductCommand;
