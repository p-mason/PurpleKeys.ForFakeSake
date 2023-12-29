namespace PurpleKeys.ForFakeSake.Extensions;

public static class DictionaryExtensions
{
    public static bool Contains<T>(this IDictionary<string, object> args, string key,  T value)
    {
        return Contains(args, key, value, EqualityComparer<T>.Default);
    }
    
    public static bool Contains<T>(this IDictionary<string, object> args, string key,  T value, IEqualityComparer<T> comparer)
    {
        return args.TryGetValue(key, out var condition) && comparer.Equals((T)condition, value);
    }
    
    public static bool NotContaining<T>(this IDictionary<string, object> args, string key,  T value)
    {
        return NotContaining(args, key, value, EqualityComparer<T>.Default);
    }
    
    public static bool NotContaining<T>(this IDictionary<string, object> args, string key,  T value, IEqualityComparer<T> comparer)
    {
        return args.TryGetValue(key, out var condition) && !comparer.Equals((T)condition, value);
    }
}