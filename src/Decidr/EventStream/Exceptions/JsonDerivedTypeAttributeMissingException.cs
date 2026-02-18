namespace Se4sonal.Decidr.EventStream.Exceptions;

public class JsonDerivedTypeAttributeMissingException : Exception
{
    // Constructor
    public JsonDerivedTypeAttributeMissingException(
        Type baseType,
        Type derivedType) : base($"{baseType.Name} is missing JsonDerivedTypeAttribute for {derivedType.Name}")
    {
        BaseTypeName = baseType.Name;
        BaseTypeFullName = derivedType.FullName;
        DerivedTypeName = derivedType.Name;
        DerivedTypeFullName = derivedType.FullName;
    }

    // Properties
    public string BaseTypeName { get; }
    public string? BaseTypeFullName { get; }
    public string DerivedTypeName { get; }
    public string? DerivedTypeFullName { get; }
}
