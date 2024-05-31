using System.Linq.Expressions;
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

    public static bool IsForeignKeyProperty(this PropertyInfo property) =>
        property.GetCustomAttribute<ForeignKeyAttribute>() != null;
    
    public static bool IsOneToManyProperty(this PropertyInfo property) =>
        property.GetCustomAttribute<OneToManyAttribute>() != null;
    
    public static bool IsManyToOneProperty(this PropertyInfo property) =>
        property.GetCustomAttribute<ManyToOneAttribute>() != null;
    
    public static bool IsOneToOneProperty(this PropertyInfo property) =>
        property.GetCustomAttribute<OneToOneAttribute>() != null;
    
    public static string GetPropertyName<T>(Expression<Func<T, object>> expr)
    {
        return expr.Body switch
        {
            // Check if the expression simply accesses a member, such as a reference type property
            MemberExpression member => member.Member.Name,
            // Check if there's a conversion in the expression, such as converting a value type to object
            UnaryExpression unaryExpr when unaryExpr.Operand is MemberExpression unaryMember => unaryMember.Member.Name,
            _ => throw new ArgumentException("Expression is not a member access", nameof(expr))
        };
    }
    
    public static List<PropertyInfo> GetPrimaryKeyProperties(Type entityType)
    {
        return entityType.GetProperties().Where(p => p.GetCustomAttribute<PrimaryKeyAttribute>() != null).ToList();
    }

    public static string GetTableName(Type entityType)
    {
        var tableAttr = entityType.GetCustomAttribute<TableAttribute>();
        return tableAttr?.Name ?? entityType.Name;
    }

    public static bool IsMappedEntityType(this Type entityType) =>
        entityType.GetCustomAttribute<TableAttribute>() != null;

    public static bool IsPrimaryKeyProperty(this PropertyInfo property) =>
        property.GetCustomAttribute<PrimaryKeyAttribute>() != null;
    
    public static string GetColumnName(PropertyInfo property) =>
        property.GetCustomAttribute<ColumnAttribute>()?.Name ?? property.Name;
}