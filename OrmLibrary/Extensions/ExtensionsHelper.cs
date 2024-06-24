using System.Linq.Expressions;
using System.Reflection;
using OrmLibrary.Attributes;
using OrmLibrary.Attributes.Relational;
using OrmLibrary.Mappings;

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
        => assembly.GetTypes().Where(type => type.GetCustomAttribute(decoratorAttributeType) != null);

    public static bool IsForeignKeyProperty(this PropertyInfo property) =>
        property.GetCustomAttribute<ForeignKeyAttribute>() != null;
    
    public static bool IsForeignKeyProperty(this PropertyInfo property, out ForeignKeyAttribute? oneToOneAttribute)
    {
        oneToOneAttribute = property.GetCustomAttribute<ForeignKeyAttribute>();
        return oneToOneAttribute != null;
    }
    
    public static bool IsOneToManyProperty(this PropertyInfo property) =>
        property.GetCustomAttribute<OneToManyAttribute>() != null;
    
    public static bool IsManyToManyProperty(this PropertyInfo property) =>
        property.GetCustomAttribute<ManyToManyAttribute>() != null;
    
    public static bool IsManyToOneProperty(this PropertyInfo property) =>
        property.GetCustomAttribute<ManyToOneAttribute>() != null;
    
    public static bool IsOneToOneProperty(this PropertyInfo property) =>
        property.GetCustomAttribute<OneToOneAttribute>() != null;
    
    public static bool IsOneToOneProperty(this PropertyInfo property, out OneToOneAttribute? oneToOneAttribute)
    {
        oneToOneAttribute = property.GetCustomAttribute<OneToOneAttribute>();
        return oneToOneAttribute != null;
    }
    
    public static string GetPropertyName<TEntity, TKey>(Expression<Func<TEntity, TKey>> expr)
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
        => entityType.GetProperties().Where(p => p.GetCustomAttribute<PrimaryKeyAttribute>() != null).ToList();

    public static string GetTableName(Type entityType)
    {
        var tableAttr = entityType.GetCustomAttribute<TableAttribute>();
        return tableAttr?.Name ?? entityType.Name;
    }

    public static int? GetMaxLength(this PropertyInfo property)
        => property.GetCustomAttribute<MaxLengthAttribute>()?.Length;

    public static bool HasUniqueValue(this PropertyInfo property)
        => property.GetCustomAttribute<UniqueAttribute>() != null;
    
    public static bool IsMappedEntityType(this Type entityType) =>
        entityType.GetCustomAttribute<TableAttribute>() != null;

    public static bool IsPrimaryKeyProperty(this PropertyInfo property) =>
        property.GetCustomAttribute<PrimaryKeyAttribute>() != null;
    
    public static string GetColumnName(PropertyInfo property) =>
        property.GetCustomAttribute<ColumnAttribute>()?.Name ?? property.Name;

    public static bool IsSameColumnAs(this ColumnProperties thisColumn, ColumnProperties otherColumn)
    {
        if (thisColumn.TableName != otherColumn.TableName) return false;
        
        if (thisColumn.Name == otherColumn.Name)
        {
            return true;
        }

        return thisColumn.PropertyName is not null && thisColumn.PropertyName == otherColumn.PropertyName;
    }

    public static string GetColumnsNamesPrefix(PropertyInfo propertyInfo)
    {
        return propertyInfo.GetCustomAttribute<OneToOneAttribute>()?.ColumnsNamesPrefix ??
               propertyInfo.GetCustomAttribute<ManyToOneAttribute>()?.ColumnsNamesPrefix ??
               GetColumnName(propertyInfo);
    }

    private static string GetCodeFilePath(Type type)
    {
        var startingDirectory = Directory.GetCurrentDirectory();
        var baseProjectPath = startingDirectory[..startingDirectory.LastIndexOf('\\')];
        var typeNamespace = type.FullName;
        var assemblyName = type.Assembly.GetName().Name!;
        var projectFilePath = typeNamespace![(typeNamespace.IndexOf(assemblyName, StringComparison.Ordinal) + assemblyName.Length)..].Replace('.', '\\');
        
        return $@"{baseProjectPath}\{type.Assembly.GetName().Name}{projectFilePath}.cs";
    }

    public static DateTime GetLastModificationDate(Type type)
    {
        return File.GetLastWriteTime(GetCodeFilePath(type)).ToUniversalTime();
    }
}