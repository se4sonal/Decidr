using System.Reflection;

namespace Decidr.Evolvers;

public class ReflectedEvolverBuilder<TState,TEvent>
{
    // Fields
    private readonly Type _evolverType;
    private readonly Dictionary<Type, MethodInfo> _evolveMethods = [];
    private MethodInfo? _createInitial = null;

    // Constructor
    public ReflectedEvolverBuilder(Type evolverType)
    {
        _evolverType = evolverType;
    }

    // Properties
    public bool HasEvolveMethods => _evolveMethods.Count > 0;
    public bool HasCreateInitial => _createInitial != null;

    // Methods - Public - Static
    public static ReflectedEvolver<TState, TEvent> CreateEvolver<TEvolver>()
    {
        var builder = new ReflectedEvolverBuilder<TState,TEvent>(typeof(TEvolver));
        var methods = typeof(TEvolver).GetMethods(BindingFlags.Public | BindingFlags.Static);
        foreach (var method in methods)
        {
            builder.TryAddDecideMethod(method);
            builder.TrySetCreateInitial(method);
        }
        return builder.Build();
    }

    public static ReflectedEvolver<TState, TEvent> CreateEvolver(Type evolver)
    {
        var builder = new ReflectedEvolverBuilder<TState, TEvent>(evolver);
        var methods = evolver.GetMethods(BindingFlags.Public | BindingFlags.Static);
        foreach (var method in methods)
        {
            builder.TryAddDecideMethod(method);
            builder.TrySetCreateInitial(method);
        }
        return builder.Build();
    }

    // Methods - Public
    public bool TryAddDecideMethod(MethodInfo method)
    {
        if (method.Name != nameof(IEvolver<TState, TEvent>.Evolve)) return false;
        if (method.ReturnType != typeof(TState)) return false;

        var p = method.GetParameters();

        if (p.Length != 2) return false;
        if (p[0].ParameterType != _evolverType) return false;

        _evolveMethods.TryAdd(p[1].ParameterType, method);

        return true;
    }

    public bool TrySetCreateInitial(MethodInfo method)
    {
        if (method.Name != nameof(IEvolver<TState, TEvent>.CreateInitial)) return false;
        if (method.ReturnType != _evolverType) return false;

        var p = method.GetParameters();

        if (p.Length != 0) return false;

        _createInitial = method;

        return true;
    }

    public ReflectedEvolver<TState, TEvent> Build()
    {
        var evolver = new ReflectedEvolver<TState, TEvent>(
            _evolverType,
            new(_evolveMethods),
            _createInitial ?? throw new InvalidOperationException($"Evolver {_evolverType.Name} is missing {nameof(ReflectedEvolver<TState, TEvent>.CreateInitial)}-method.")
        );

        _evolveMethods.Clear();
        _createInitial = null;

        return evolver;
    }
}
