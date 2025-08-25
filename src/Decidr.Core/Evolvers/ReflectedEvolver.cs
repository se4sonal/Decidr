using System.Reflection;

namespace Decidr.Evolvers;

public class ReflectedEvolver<TState, TEvent> : IEvolver<TState, TEvent>
{
    // Fields
    private readonly Type _evolverType;
    private readonly Dictionary<Type, MethodInfo> _evolveMethods = [];
    private readonly MethodInfo _createInitial;

    // Constructor
    public ReflectedEvolver(
        Type evolverType,
        Dictionary<Type, MethodInfo> evolveMethods,
        MethodInfo createInitial) 
    {
        _evolverType = evolverType;
        _evolveMethods = evolveMethods;
        _createInitial = createInitial;
    }

    // Methods
    public TState CreateInitial()
    {
        return (TState?)_createInitial.Invoke(null, []) ?? throw new InvalidOperationException($"{nameof(CreateInitial)}-method in {_evolverType.Name} returned null.");
    }

    public TState Evolve(TState current, TEvent evnt)
    {
        var evntTyp = evnt?.GetType() ?? throw new InvalidOperationException("Could not get event type");

        if (_evolveMethods.TryGetValue(evntTyp, out var method))
        {
            return (TState)method.Invoke(null, [current, evnt])!;
        }

        throw new InvalidOperationException($"No matching {Evolve}-method found for event type {evntTyp.Name} in {_evolverType.Name}.");
    }
}
