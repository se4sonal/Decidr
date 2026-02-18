namespace DecidrV2.Domain;

public partial class ProductAggregate
{
    public Guid Id { get; set; } = Guid.Empty;
    public string Name { get; set; } = string.Empty;
}
