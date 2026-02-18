namespace Decidr.EventSourcingV2;

public class SyntaxTest
{
    record MyState();
    record MyEventBase();
    record MyCommandBase();
    record MyAggregateRoot() : IAggregateRoot<MyState, MyEventBase, MyCommandBase>
    {
    }

    public static void MyTest()
    {
        IEventStoreSession session = default!;
        var stream = session.Events.StartStream<MyState,MyEventBase,MyCommandBase>();
    }
}
