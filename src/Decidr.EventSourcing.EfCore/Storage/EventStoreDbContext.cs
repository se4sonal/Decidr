using Decidr.Storage.Configs;
using Decidr.Storage.Entities;
using Microsoft.EntityFrameworkCore;

namespace Decidr.Storage;

public class EventStoreDbContext : DbContext
{
    // Constructor
    public EventStoreDbContext(DbContextOptions options) : base(options)
    {
    }

    // Properties
    public DbSet<DatabaseEvent> Events { get; set; }

    // Methods - Protected
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new DatabaseEventConfig());
    }
}
