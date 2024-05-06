using System.Reflection;
using OrmLibrary.Attributes;
using OrmLibrary.Attributes.Relational;

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

    public static IEnumerable<Type> GetDecoratedTypes(this Assembly assembly, Type decoratorAttributeType)
    {
        return assembly.GetTypes().Where(type => type.GetCustomAttribute(decoratorAttributeType) != null);
    }

    public static bool IsForeignKeyReference(PropertyInfo property) =>
        property.GetCustomAttribute<ForeignKeyAttribute>() != null;
    
    public static List<PropertyInfo> GetPrimaryKeyProperties(Type entityType)
    {
        return entityType.GetProperties().Where(p => p.GetCustomAttribute<PrimaryKeyAttribute>() != null).ToList();
    }

    public static string GetTableName(Type entityType)
    {
        var tableAttr = entityType.GetCustomAttribute<TableAttribute>();
        return tableAttr?.Name ?? entityType.Name;
    }

    public static string GetColumnName(PropertyInfo property) =>
        property.GetCustomAttribute<ColumnAttribute>()?.Name ?? property.Name;
}