using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Se4sonal.Decidr.EventStream.Storage.Entities;

public class SnapshotEntity
{
    // Properties
    public int Id { get; set; }
    public int Version { get; set; }
    public string Json { get; set; } = default!;

    // Navigation Properties
    public HeaderEntity Header { get; set; } = default!;
}

public class SnapshotConfiguration : IEntityTypeConfiguration<SnapshotEntity>
{
    public void Configure(EntityTypeBuilder<SnapshotEntity> builder)
    {
        // Keys
        builder.HasKey(x => x.Id);

        // Properties
        builder.Property(x => x.Version).IsRequired();
        builder.Property(x => x.Json).IsRequired();

        // Relationship
        builder
            .HasOne(x => x.Header)
            .WithOne(x => x.Snapshot)
            .HasForeignKey<SnapshotEntity>(x => x.Id)
            .IsRequired(false);
    }
}
