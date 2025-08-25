using System.Reflection;

namespace Decidr.Deciders;

public class ReflectedDeciderBuilder<TState, TEvent, TCommand>
{
    // Fields
    private readonly Type _deciderType;
    private readonly Dictionary<Type, MethodInfo> _decideMethods = [];
    private MethodInfo? _isTerminal = null;

    // Constructor
    public ReflectedDeciderBuilder(Type deciderType)
    {
        _deciderType = deciderType;
    }

    // Properties
    public bool HasDecideMethods => _decideMethods.Count > 0;
    public bool HasIsTerminal => _isTerminal != null;

    // Methods - Public - Static
    public static ReflectedDecider<TState, TEvent, TCommand> CreateDecider<TDecider>()
    {
        var builder = new ReflectedDeciderBuilder<TState, TEvent, TCommand>(typeof(TDecider));
        var methods = typeof(TDecider).GetMethods(BindingFlags.Public | BindingFlags.Static);
        foreach (var method in methods)
        {
            builder.TryAddDecideMethod(method);
            builder.TrySetIsTerminal(method);
        }
        return builder.Build();
    }

    public static ReflectedDecider<TState, TEvent, TCommand> CreateDecider(Type deciderType)
    {
        var builder = new ReflectedDeciderBuilder<TState, TEvent, TCommand>(deciderType);
        var methods = deciderType.GetMethods(BindingFlags.Public | BindingFlags.Static);
        foreach (var method in methods)
        {
            builder.TryAddDecideMethod(method);
            builder.TrySetIsTerminal(method);
        }
        return builder.Build();
    }

    // Methods - Public
    public bool TryAddDecideMethod(MethodInfo method)
    {
        if (method.Name != nameof(IDecider<TState,TEvent,TCommand>.Decide)) return false;
        if (method.ReturnType != typeof(IEnumerable<TEvent>)) return false;

        var p = method.GetParameters();

        if (p.Length != 2) return false;
        if (p[0].ParameterType != _deciderType) return false;

        _decideMethods.TryAdd(p[1].ParameterType, method);

        return true;
    }

    public bool TrySetIsTerminal(MethodInfo method)
    {
        if (method.Name != nameof(IDecider<TState, TEvent, TCommand>.IsTerminal)) return false;
        if (method.ReturnType != typeof(bool)) return false;

        var p = method.GetParameters();

        if (p.Length != 1) return false;
        if (p[0].ParameterType != _deciderType) return false;

        _isTerminal = method;

        return true;
    }

    public ReflectedDecider<TState,TEvent,TCommand> Build()
    {
        var decider = new ReflectedDecider<TState, TEvent, TCommand>(
            _deciderType,
            new(_decideMethods),
            _isTerminal ?? throw new InvalidOperationException($"Decider {_deciderType.Name} is missing {nameof(ReflectedDecider<TState,TEvent,TCommand>.IsTerminal)}-method.")
        );

        _decideMethods.Clear();
        _isTerminal = null;

        return decider;
    }
}
