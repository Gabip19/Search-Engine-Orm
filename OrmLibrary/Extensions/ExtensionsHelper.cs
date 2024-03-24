using System.Reflection;

namespace OrmLibrary.Extensions;

public static class ExtensionsHelper
{
    private const string NULLABLE_ATTRIBUTE_TYPE = "System.Runtime.CompilerServices.NullableAttribute";
    
    public static bool IsNullable(this PropertyInfo property)
    {
        if (Nullable.GetUnderlyingType(property.PropertyType) != null)
        {
            return true;
        }

        if (!property.PropertyType.IsValueType)
        {
            var nullableAttribute = property.CustomAttributes.FirstOrDefault(attr =>
                attr.AttributeType.FullName == NULLABLE_ATTRIBUTE_TYPE);

            if (nullableAttribute is { ConstructorArguments.Count: > 0 })
            {
                var attributeArgument = nullableAttribute.ConstructorArguments[0];
                
                if (attributeArgument.ArgumentType == typeof(byte))
                {
                    if ((byte)attributeArgument.Value! == 2)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    public static Type GetBaseType(this PropertyInfo property) => 
        Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
}