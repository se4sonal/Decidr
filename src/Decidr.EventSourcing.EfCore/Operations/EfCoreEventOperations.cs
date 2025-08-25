using Decidr.Events;
using Decidr.Operations.Abstractions;
using Decidr.Storage;
using Microsoft.EntityFrameworkCore;

namespace Decidr.Operations;

public class EfCoreEventOperations : EventOperations
{
    // Fields
    private readonly EventStoreDbContext _ctx;

    // Constructor
    public EfCoreEventOperations(EventStoreDbContext ctx)
    {
        _ctx = ctx;
    }

    // Methods
    public override IAsyncEnumerable<StoredEvent<TEvent>> FetchEventsAsync<TEvent>(
        Guid streamId, 
        long toVersion = 0, 
        long fromVersion = 0, 
        CancellationToken token = default)
    {
        // Initialize query
        var query = _ctx.Events
            .AsNoTracking();

        // Specify where condition
        if (toVersion <= 0 && fromVersion <= 0)
        {
            query = query.Where(x => x.StreamId == streamId);
        }
        else if (toVersion > 0 && fromVersion <= 0)
        {
            query = query.Where(x => x.StreamId == streamId && x.Version <= toVersion);
        }
        else if (toVersion <= 0 && fromVersion > 0)
        {
            query = query.Where(x => x.StreamId == streamId && x.Version > fromVersion);
        }
        else
        {
            if (toVersion < fromVersion)
            {
                throw new ArgumentException($"{nameof(fromVersion)} cannot be greater than {nameof(toVersion)}.");
            }
            else if (toVersion == fromVersion)
            {
                query = query.Where(x => x.StreamId == streamId && x.Version == toVersion);
            }
            else
            {
                query = query.Where(x => x.StreamId == streamId && x.Version > fromVersion && x.Version <= toVersion);
            }
        }

        // Return async enumerable
        return query
            .OrderBy(x => x.Version)
            .Select(x => x.ToStoredEvent<TEvent>())
            .AsAsyncEnumerable();
    }
}
