﻿using System.Reflection;
using OrmLibrary.Attributes;
using OrmLibrary.Attributes.Relational;
using OrmLibrary.Enums;
using OrmLibrary.Mappings;

namespace OrmLibrary.Extensions;

public static class ExtensionsHelper
{
    private const string NullableAttributeType = "System.Runtime.CompilerServices.NullableAttribute";
     
    public static bool IsNullable(this PropertyInfo property)
    {
        if (Nullable.GetUnderlyingType(property.PropertyType) != null)
        {
            return true;
        }

        if (!property.PropertyType.IsValueType)
        {
            var nullableAttribute = property.CustomAttributes.FirstOrDefault(attr =>
                attr.AttributeType.FullName == NullableAttributeType);

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

    public static string GetAssemblyPath(Assembly assembly)
    {
        var startingDirectory = Directory.GetCurrentDirectory();
        var baseProjectPath = startingDirectory[..startingDirectory.LastIndexOf(Path.DirectorySeparatorChar)];
        var assemblyName = assembly.GetName().Name!;

        return Path.Combine(baseProjectPath, assemblyName);
    }
    
    public static string GetCodeFilePath(Type type)
    {
        var assemblyName = type.Assembly.GetName().Name!;
        var typeNamespace = type.FullName;
        var projectFilePath =
            typeNamespace![(typeNamespace.IndexOf(assemblyName, StringComparison.Ordinal) + assemblyName.Length)..]
                .Replace('.', Path.DirectorySeparatorChar);
        
        var a = $"{GetAssemblyPath(type.Assembly)}{projectFilePath}.cs";
        return a;
    }

    public static DateTime GetLastModificationDate(Type type)
    {
        return File.GetLastWriteTime(GetCodeFilePath(type)).ToUniversalTime();
    }

    public static bool TypeRequiresMaxLength(SqlType sqlColumType)
    {
        return sqlColumType is SqlType.NVarChar or SqlType.VarChar;
    }
}