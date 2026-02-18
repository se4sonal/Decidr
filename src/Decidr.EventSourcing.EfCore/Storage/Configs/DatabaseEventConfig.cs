using Decidr.Storage.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Decidr.Storage.Configs;

public class DatabaseEventConfig : IEntityTypeConfiguration<DatabaseEvent>
{
    public void Configure(EntityTypeBuilder<DatabaseEvent> builder)
    {
        // Identities
        builder.HasKey(x => new { x.StreamId, x.Version });
        builder.HasAlternateKey(x => x.EventId);

        // Properties
        builder.Property(x => x.Timestamp).IsRequired();
        builder.Property(x => x.EventBody).IsRequired();
        builder.Property(x => x.RowVersion).IsRowVersion();

        // Indexes
        builder.HasIndex(x => x.EventId).IsUnique();
    }
}
