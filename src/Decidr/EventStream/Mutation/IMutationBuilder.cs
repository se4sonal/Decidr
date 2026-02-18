namespace Se4sonal.Decidr.EventStream.Mutation;

public interface IMutationBuilder<TId, TState, TEventBase, TCommandBase>
    where TId : struct, IEquatable<TId>
    where TState : IDeciderState
    where TEventBase : IDeciderEvent<TState>
    where TCommandBase : IDeciderCommand<TState, TEventBase>
{
    IMutationBuilderCanCommand<TId, TState, TEventBase, TCommandBase> LoadDefault(TId id);
    IMutationBuilderCanCommand<TId, TState, TEventBase, TCommandBase> LoadLast(TId id);
    IMutationBuilderCanCommand<TId, TState, TEventBase, TCommandBase> LoadSpecific(TId id, int expectedVersion);
}

public interface IMutationBuilderCanCommand<TId, TState, TEventBase, TCommandBase>
    where TId : struct, IEquatable<TId>
    where TState : IDeciderState
    where TEventBase : IDeciderEvent<TState>
    where TCommandBase : IDeciderCommand<TState, TEventBase>
{
    IMutationBuilderCanExecute<TId, TState, TEventBase, TCommandBase> AddCommand(TCommandBase command);
}

public interface IMutationBuilderCanExecute<TId, TState, TEventBase, TCommandBase>
    : IMutationBuilderCanCommand<TId, TState, TEventBase, TCommandBase>
    where TId : struct, IEquatable<TId>
    where TState : IDeciderState
    where TEventBase : IDeciderEvent<TState>
    where TCommandBase : IDeciderCommand<TState, TEventBase>
{
    ValueTask<int> SaveAsync(CancellationToken cancellationToken = default);
}
