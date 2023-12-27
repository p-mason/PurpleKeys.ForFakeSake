using System.Reflection;

namespace PurpleKeys.ForFakeSake.Internal;

internal class PropertyInterceptWrapper
{
    private object? _value;
    private readonly Func<object?, object?> _getMethod;
    private readonly Func<object?, object?> _setMethod;

    private PropertyInterceptWrapper(
        object? initialValue,
        Func<object?, object?> getMethod, 
        Func<object?, object?> setMethod)
    {
        _value = initialValue;
        _getMethod = getMethod;
        _setMethod = setMethod;
    }

    public static PropertyInterceptWrapper Automatic(PropertyInfo property)
    {
        Func<object?, object?> read = property.CanRead
            ? new Func<object?, object?>(v => v)
            : _ => throw new InvalidOperationException();

        Func<object?, object?> write = property.CanWrite
            ? new Func<object?, object?>(v => v)
            : _ => throw new InvalidOperationException();

        var initialValue = property.PropertyType.IsValueType
            ? Activator.CreateInstance(property.PropertyType)
            : null;
        
        return new PropertyInterceptWrapper(initialValue, read, write);
    }

    public object? Get() => _getMethod(_value);

    public void Set(object? setValue)
    {
        _value = _setMethod(setValue);
    }
}