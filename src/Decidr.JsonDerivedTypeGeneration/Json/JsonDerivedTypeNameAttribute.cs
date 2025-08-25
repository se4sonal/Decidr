namespace Decidr.Json;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class JsonDerivedTypeNameAttribute : Attribute
{
    // Constructor
    public JsonDerivedTypeNameAttribute(string name)
    {
        Name = name;
    }

    // Properties
    public string Name { get; }
}
