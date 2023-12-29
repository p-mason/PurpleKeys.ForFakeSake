using System.Reflection;

namespace PurpleKeys.ForFakeSake.Internal;

public static class ReflectionUtility
{
    private const BindingFlags PublicInstance = BindingFlags.Instance | BindingFlags.Public;

    public static object? CreatePropertyDefaultValue(PropertyInfo property)
    {
        return property.PropertyType.IsValueType
            ? Activator.CreateInstance(property.PropertyType)
            : null;
    }

    public static bool IsPropertyOnType(Type type, MemberInfo member)
    {
        return member.DeclaringType == type &&
               member.MemberType == MemberTypes.Property;
    }

    public static string PropertySetSignature<T>(string propertyName)
    {
        var key =  typeof(T).GetProperty(propertyName)!.SetMethod!.ToString()!;
        if (key == null)
        {
            throw new Exception();
        }

        return key;
    }

    public static IEnumerable<PropertyInfo> GetProperties<T>(string propertyName)
    {
        return typeof(T).GetProperties(PublicInstance)
            .Where(m => m.Name == propertyName);
    }
    
    public static IEnumerable<PropertyInfo> GetProperties<T>()
    {
        return typeof(T).GetProperties(PublicInstance);
    }
    
    public static IEnumerable<MethodInfo> GetFunctionOverloads<T, TReturn>(string functionName)
    {
        return typeof(T)
            .GetMethods(PublicInstance)
            .Where(m => m.Name.Equals(functionName) && m.ReturnType == typeof(TReturn))
            .ToArray();
    }
    
    public static IEnumerable<MethodInfo> GetActionOverloads<T>(string actionName)
    {
        return typeof(T)
            .GetMethods(PublicInstance)
            .Where(m => m.Name.Equals(actionName) && m.ReturnType == typeof(void))
            .ToArray();
    }
    
    public static IEnumerable<MethodInfo> GetMethodOverloads<T>(string memberName)
    {
        return typeof(T)
            .GetMethods(PublicInstance)
            .Where(m => m.Name.Equals(memberName))
            .ToArray();
    }

    public static IDictionary<string, object> ZipParametersAndArguments(MethodInfo method, object[] arguments)
    {
        return method
            .GetParameters()
            .Zip(arguments)
            .ToDictionary(
                k => k.First.Name ?? throw new NotSupportedException("Parameters Must Have a Name."), 
                v => v.Second);
    }

    public static bool TryFindProperty<T>(MethodInfo propertyAccessor, out PropertyInfo? property)
    {
        if (propertyAccessor.IsSpecialName && propertyAccessor.IsHideBySig)
        {
            var propertyName = propertyAccessor.Name.Substring(4);
            property = typeof(T).GetProperty(propertyName, PublicInstance);
        }
        else
        {
            property = null;
        }
        
        return property != null;
    }
}