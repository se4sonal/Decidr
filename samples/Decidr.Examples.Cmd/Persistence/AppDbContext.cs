using Microsoft.EntityFrameworkCore;
using Se4sonal.Decidr.EventStream.Storage;

namespace Decidr.Examples.Cmd.Persistence;

public class AppDbContext : EventStoreDbContext
{
    // Constructor
    public AppDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
    {
    }
}
