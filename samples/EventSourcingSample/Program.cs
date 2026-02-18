using Decidr;
using EventSourcingSample.Domain;

IEventStoreSession session = default!;
var stream = new ProductAggregate().StartStream(session, Guid.NewGuid());
