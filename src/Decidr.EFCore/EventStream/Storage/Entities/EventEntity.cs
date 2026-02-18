using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Se4sonal.Decidr.EventStream.Storage.Entities;

public class EventEntity
{
    // Properties
    public int Id { get; set; }
    public int HeaderId { get; set; }
    public int Version { get; set; }
    public string EventName { get; set; } = default!;
    public string Json { get; set; } = default!;

    // Navigation Properties
    public HeaderEntity Header { get; set; } = default!;
}

public class EventConfiguration : IEntityTypeConfiguration<EventEntity>
{
    public void Configure(EntityTypeBuilder<EventEntity> builder)
    {
        // Keys
        builder.HasKey(x => x.Id);

        // Properties
        builder.Property(x => x.Version).IsRequired();
        builder.Property(x => x.EventName).IsRequired();
        builder.Property(x => x.Json).IsRequired();

        // Relationships
        builder
            .HasOne(x => x.Header)
            .WithMany(x => x.Events)
            .HasForeignKey(x => x.HeaderId);

        // Indexes
        builder.HasIndex(x => new { x.HeaderId, x.Version }).IsUnique();
    }
}
