using Se4sonal.Decidr.EventStream.Exceptions;
using System.Text.Json.Serialization;

namespace Se4sonal.Decidr.EventStream;

public static class JsonHelper
{
    /// <summary>
    /// Get the value of the JsonDerivedTypeAttribute for a derived type.
    /// </summary>
    public static string GetJsonDerivedTypeValue(
        Type baseType,
        Type derivedType)
    {
        // Try to get json derived type attribute
        var discriminatorValue = baseType
            .GetCustomAttributes(typeof(JsonDerivedTypeAttribute), inherit: false)
            .Cast<JsonDerivedTypeAttribute>()
            .FirstOrDefault(x => x.DerivedType == derivedType)?.TypeDiscriminator?
            .ToString();

        // Throw exception if attribute is missing
        if (string.IsNullOrEmpty(discriminatorValue))
        {
            throw new JsonDerivedTypeAttributeMissingException(baseType, derivedType);
        }

        // Return the value
        return discriminatorValue;
    }
}
