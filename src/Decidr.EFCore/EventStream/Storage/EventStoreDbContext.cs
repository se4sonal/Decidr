using Decidr.EventStream.Mutation;
using Microsoft.EntityFrameworkCore;
using Se4sonal.Decidr.EventStream.Exceptions;
using Se4sonal.Decidr.EventStream.Mutation;
using Se4sonal.Decidr.EventStream.Storage.Entities;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace Se4sonal.Decidr.EventStream.Storage;

public class EventStoreDbContext : DbContext, IEventStore
{
    // Fields
    private readonly JsonSerializerOptions _serializerOptions;

    // Constructor
    public EventStoreDbContext(
        DbContextOptions dbContextOptions,
        JsonSerializerOptions? serializerOptions = null) : base(dbContextOptions)
    {
        _serializerOptions = serializerOptions ?? new();
    }

    // Properties
    public DbSet<HeaderEntity> Headers { get; set; }
    public DbSet<EventEntity> Events { get; set; }
    public DbSet<SnapshotEntity> Snapshots { get; set; }

    // Methods - Protected
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new HeaderConfiguration());
        modelBuilder.ApplyConfiguration(new EventConfiguration());
        modelBuilder.ApplyConfiguration(new SnapshotConfiguration());
    }

    protected static string ConvertToString<TId>(TId streamId)
        where TId : notnull
    {
        var strId = streamId.ToString();
        if (string.IsNullOrWhiteSpace(strId)) throw new ArgumentNullException(nameof(streamId));
        return strId;
    }

    protected virtual TEventBase Deserialize<TEventBase>(
        EventEntity streamEvent, 
        string streamName, 
        string streamId)
    {
        return JsonSerializer.Deserialize<TEventBase>(streamEvent.Json, _serializerOptions)
            ?? throw new ArgumentException($"Failed to serialize stream event. [StreamName:{streamName}, EventName:{streamEvent.EventName}, EventId:{streamEvent.Id}]");
    }

    protected virtual TState Deserialize<TState>(
        SnapshotEntity streamSnapshot, 
        string streamName, 
        string streamId)
    {
        return JsonSerializer.Deserialize<TState>(streamSnapshot.Json, _serializerOptions)
            ?? throw new ArgumentException($"Failed to serialize stream snapshot. [StreamName:{streamName}, StreamId:{streamId}, SnapshotId:{streamSnapshot.Id}]");
    }

    // Methods - Public
    public virtual IMutationBuilder<TId, TState, TEventBase, TCommandBase> CreateMutationBuilder<TId, TState, TEventBase, TCommandBase>(
        IStreamDecider<TId, TState, TEventBase, TCommandBase> decider)
        where TId : struct, IEquatable<TId>
        where TState : IDeciderState
        where TEventBase : IDeciderEvent<TState>
        where TCommandBase : IDeciderCommand<TState, TEventBase>
    {
        return MutationBuilder<TId, TState, TEventBase, TCommandBase>.Create(decider, this);
    }

    public virtual async ValueTask<(int Version, TState State)?> TryGetSnapshotAsync<TId, TState>(
        string streamName, 
        TId streamId, 
        CancellationToken cancellationToken = default)
        where TId : struct, IEquatable<TId>
        where TState : IDeciderState
    {
        // Create the string id
        var id = ConvertToString(streamId);

        // Create query
        var entity = await Snapshots
            .Include(x => x.Header)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Header.StreamName == streamName && x.Header.StreamId == id, cancellationToken);

        // Return response
        return (entity == null)
            ? null
            : (entity.Version, Deserialize<TState>(entity, streamName, id));
    }

    public virtual async IAsyncEnumerable<IStreamEvent<TId, TState, TEventBase>> GetEventsAsync<TId, TState, TEventBase>(
        string streamName, 
        TId streamId, 
        int fromVersion = 0, 
        int toVersion = int.MaxValue,
        [EnumeratorCancellation]CancellationToken cancellationToken = default)
        where TId : struct, IEquatable<TId>
        where TState : IDeciderState
        where TEventBase : IDeciderEvent<TState>
    {
        // Create the string id
        var id = ConvertToString(streamId);

        // Create query
        var query = Events
            .Include(x => x.Header)
            .AsNoTracking()
            .Where(x => x.Header.StreamName == streamName && x.Header.StreamId == id && x.Version >= fromVersion)
            .OrderBy(x => x.Version)
            .AsAsyncEnumerable();

        // Convert from entity to IPersistedEvent compatible object
        await foreach (var itm in query)
        {
            yield return new StreamEvent<TId, TState, TEventBase>(
                streamName,
                streamId,
                itm.Version,
                Deserialize<TEventBase>(itm, streamName, id));
        }
    }

    public virtual async Task<int> AddEventsAsync<TId, TState, TEventBase>(
        string streamName, 
        TId streamId, 
        int expectedVersion, 
        IList<TEventBase> events, 
        CancellationToken cancellationToken = default)
        where TId : struct, IEquatable<TId>
        where TState : IDeciderState
        where TEventBase : IDeciderEvent<TState>
    {
        // Exit early if no events is provided
        if (events.Count == 0)
            return expectedVersion;

        // Create the string id
        var id = ConvertToString(streamId);

        // Get stream header with events
        var header = await Headers
            .Include(x => x.Events.Where(x => x.Version >= expectedVersion).OrderBy(x => x.Version))
            .FirstOrDefaultAsync(x => x.StreamName == streamName && x.StreamId == id, cancellationToken);

        // Handle update
        var currentVersion = expectedVersion;
        if (header == null)
        {
            // Ensure correct expected version
            if (expectedVersion != IEventStore.DefaultVersion)
            {
                throw new ExpectedVersionInFutureException(expectedVersion, IEventStore.DefaultVersion);
            }

            // Create and attach stream header
            header = new HeaderEntity
            {
                StreamName = streamName,
                StreamId = id,
                Events = []
            };
            Attach(header);
        }
        else
        {
            // Ensure correct expected version
            if (expectedVersion == IEventStore.DefaultVersion || header.Events.Count != 1)
            {
                throw new ExpectedVersionInPastException(expectedVersion, header.Events.Last().Version);
            }
        }

        // Add new events
        var baseType = typeof(TEventBase);
        foreach (var e in events)
        {
            currentVersion += IEventStore.VersionStep;
            header.Events.Add(new EventEntity
            {
                HeaderId = header.Id,
                Version = currentVersion,
                EventName = JsonHelper.GetJsonDerivedTypeValue(baseType, e.GetType()),
                Json = JsonSerializer.Serialize(e, _serializerOptions)
            });
        }

        // Save changes
        await SaveChangesAsync(cancellationToken);

        // Return response
        return currentVersion;
    }

    public virtual async Task UpdateSnapshotAsync<TId, TState>(
        string streamName, 
        TId streamId, 
        int snapshotVersion, 
        TState snapshotState, 
        CancellationToken cancellationToken = default)
        where TId : struct, IEquatable<TId>
        where TState : IDeciderState
    {
        // Create the string id
        var id = ConvertToString(streamId);

        // Get header with snapshot
        var entity = await Headers
            .Include(x => x.Snapshot)
            .FirstOrDefaultAsync(x => x.StreamName == streamName && x.StreamId == id, cancellationToken);

        // Thorw error if stream dosn't exist
        if (entity == null) throw new StreamNotFoundException(streamName, id);

        // Handle update
        if (entity.Snapshot == null)
        {
            entity.Snapshot = new SnapshotEntity
            {
                Id = entity.Id,
                Version = snapshotVersion,
                Json = JsonSerializer.Serialize(snapshotState, _serializerOptions)
            };
        }
        else
        {
            entity.Snapshot.Version = snapshotVersion;
            entity.Snapshot.Json = JsonSerializer.Serialize(snapshotState, _serializerOptions);
        }

        // Save changes
        await SaveChangesAsync(cancellationToken);
    }

    public virtual async Task CommitAsync(
        CancellationToken cancellationToken = default)
    {
        await Database.CommitTransactionAsync(cancellationToken);
    }
}
