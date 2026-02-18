using System.Reflection;

namespace Decidr.Deciders;

public class ReflectedDecider<TState, TEvent, TCommand> : IDecider<TState, TEvent, TCommand>
{
    // Fields
    private readonly Type _deciderType;
    private readonly Dictionary<Type, MethodInfo> _decideMethods;
    private readonly MethodInfo _isTerminal;

    // Constructor
    public ReflectedDecider(
        Type deciderType,
        Dictionary<Type, MethodInfo> decideMethods,
        MethodInfo isTerminal) 
    {
        _deciderType = deciderType;
        _decideMethods = decideMethods;
        _isTerminal = isTerminal;
    }

    // Methods - Public
    public bool IsTerminal(TState current)
    {
        return (bool?)_isTerminal.Invoke(null, [current]) ?? throw new InvalidOperationException($"{nameof(IsTerminal)}-method in {_deciderType.Name} returned null.");
    }

    public IEnumerable<TEvent> Decide(TState current, TCommand cmd)
    {
        var cmdTyp = cmd?.GetType() ?? throw new InvalidOperationException("Could not get command type.");

        if (_decideMethods.TryGetValue(cmdTyp, out var method))
        {
            return (IEnumerable<TEvent>)method.Invoke(null, [current, cmd])!;
        }

        throw new InvalidOperationException($"No matching {nameof(Decide)}-method found for command type {cmdTyp.Name} in {_deciderType.Name}.");
    }
}
