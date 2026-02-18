using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Se4sonal.Decidr.EventStream.Storage.Entities;

public class HeaderEntity
{
    // Properties
    public int Id { get; set; }
    public string StreamName { get; set; } = default!;
    public string StreamId { get; set; } = default!;

    // Navigation Properties
    public SnapshotEntity? Snapshot { get; set; }
    public List<EventEntity> Events { get; set; } = default!;
}

public class HeaderConfiguration : IEntityTypeConfiguration<HeaderEntity>
{
    public void Configure(EntityTypeBuilder<HeaderEntity> builder)
    {
        // Keys
        builder.HasKey(x => x.Id);

        // Properties
        builder.Property(x => x.StreamName).IsRequired();
        builder.Property(x => x.StreamId).IsRequired();

        // Indexes
        builder.HasIndex(x => new { x.StreamName, x.StreamId });
    }
}
