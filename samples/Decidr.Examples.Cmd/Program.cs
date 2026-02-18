// See https://aka.ms/new-console-template for more information
using Decidr.Examples.Cmd.Domain.Customers;
using Decidr.Examples.Cmd.Domain.Customers.Commands;
using Decidr.Examples.Cmd.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Se4sonal.Decidr.EventStream.Storage;
using Se4sonal.Decidr.EventStream.Storage.Entities;

// Setup db context options
var ctxOptions = new DbContextOptionsBuilder()
    .UseInMemoryDatabase("MyInMemoryDB")
    .ConfigureWarnings(b => b.Ignore(InMemoryEventId.TransactionIgnoredWarning)) // Ignore transaction warnings since we are using the EFCore.InMemory provider.
    .Options;

// Simulate start start-up procedure
using (var ctx = new AppDbContext(ctxOptions))
{
    // Ensure that the in-memory db is created
    if (await ctx.Database.EnsureCreatedAsync())
    {
        Console.WriteLine("Database successfully created!");
    }
    else
    {
        Console.WriteLine("Database could not be created");
        return;
    }
}

// Example 1
var customerId = 1;
var currentVersion = IEventStore.DefaultVersion;
using (var ctx = new AppDbContext(ctxOptions))
{
    // Create a customer decider
    var customerDecider = new CustomerDecider();

    // Start a transaction
    await ctx.Database.BeginTransactionAsync();

    // Load default, create and rename
    currentVersion = await ctx.CreateMutationBuilder(customerDecider)
        .LoadDefault(customerId)
        .AddCommand(new CreateCustomerCommand(customerId, "Customer1"))
        .AddCommand(new RenameCustomerCommand("CustomerRenamed1"))
        .SaveAsync();

    // Commit changes
    await ctx.CommitAsync();
}

// Example 2
using (var ctx = new AppDbContext(ctxOptions))
{
    // Create a customer decider
    var customerDecider = new CustomerDecider();

    // Start a transaction
    await ctx.Database.BeginTransactionAsync();

    // Load last, rename
    currentVersion = await ctx.CreateMutationBuilder(customerDecider)
        .LoadLast(customerId)
        .AddCommand(new RenameCustomerCommand("CustomerRenamed2"))
        .SaveAsync();

    // Commit changes
    await ctx.CommitAsync();
}

// Example 3
using (var ctx = new AppDbContext(ctxOptions))
{
    // Create a customer decider
    var customerDecider = new CustomerDecider();

    // Start a transaction
    await ctx.Database.BeginTransactionAsync();

    // Load specific, rename
    currentVersion = await ctx.CreateMutationBuilder(customerDecider)
        .LoadSpecific(customerId, currentVersion)
        .AddCommand(new RenameCustomerCommand("CustomerRenamed3"))
        .SaveAsync();

    // Commit changes
    await ctx.CommitAsync();
}

// Example 4, (Uncomment to delete)
//using (var ctx = new AppDbContext(ctxOptions))
//{
//    // Create a customer decider
//    var customerDecider = new CustomerDecider();

//    // Start a transaction
//    await ctx.Database.BeginTransactionAsync();

//    // Load specific, rename
//    currentVersion = await ctx.CreateMutationBuilder(customerDecider)
//        .LoadLast(customerId)
//        .AddCommand(new DeleteCustomerCommand())
//        .SaveAsync();

//    // Commit changes
//    await ctx.CommitAsync();
//}

// Print table contents
using (var ctx = new AppDbContext(ctxOptions))
{
    PrintDbContext(ctx);
}

Console.WriteLine("Successful execution");

// Helper methods
void PrintDbContext(AppDbContext ctx)
{
    PrintHeaders(ctx);
    Console.WriteLine();

    PrintEvents(ctx);
    Console.WriteLine();

    PrintSnapshots(ctx);
    Console.WriteLine();
}

void PrintHeaders(EventStoreDbContext ctx)
{
    Console.WriteLine("## Header Table");
    Console.WriteLine();
    var items = ctx.Set<HeaderEntity>().ToList();
    for (int i = 0; i < items.Count; i++)
    {
        var itm = items[i];
        Console.WriteLine($"# Row {i+1}");
        Console.WriteLine($"{nameof(itm.Id)}: {itm.Id}");
        Console.WriteLine($"{nameof(itm.StreamName)}: {itm.StreamName}");
        Console.WriteLine($"{nameof(itm.StreamId)}: {itm.StreamId}");
        Console.WriteLine();
    }
}

void PrintEvents(EventStoreDbContext ctx)
{
    Console.WriteLine("## Event Table");
    Console.WriteLine();
    var items = ctx.Set<EventEntity>().ToList();
    for (int i = 0; i < items.Count; i++)
    {
        var itm = items[i];
        Console.WriteLine($"# Row {i + 1}");
        Console.WriteLine($"{nameof(itm.Id)}: {itm.Id}");
        Console.WriteLine($"{nameof(itm.HeaderId)}: {itm.HeaderId}");
        Console.WriteLine($"{nameof(itm.Version)}: {itm.Version}");
        Console.WriteLine($"{nameof(itm.EventName)}: {itm.EventName}");
        Console.WriteLine($"{nameof(itm.Json)}: {itm.Json}");
        Console.WriteLine();
    }
}

void PrintSnapshots(EventStoreDbContext ctx)
{
    Console.WriteLine("## Snapshot Table");
    Console.WriteLine();
    var items = ctx.Set<SnapshotEntity>().ToList();
    for (int i = 0; i < items.Count; i++)
    {
        var itm = items[i];
        Console.WriteLine($"# Row {i + 1}");
        Console.WriteLine($"{nameof(itm.Id)}: {itm.Id}");
        Console.WriteLine($"{nameof(itm.Version)}: {itm.Version}");
        Console.WriteLine($"{nameof(itm.Json)}: {itm.Json}");
        Console.WriteLine();
    }
}