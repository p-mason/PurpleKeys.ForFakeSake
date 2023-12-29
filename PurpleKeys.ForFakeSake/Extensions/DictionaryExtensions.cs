namespace PurpleKeys.ForFakeSake.Extensions;

public static class DictionaryExtensions
{
    public static bool Equals<T>(this IDictionary<string, object> args, string key,  T value)
    {
        return Equals(args, key, value, EqualityComparer<T>.Default);
    }
    
    public static bool Equals<T>(this IDictionary<string, object> args, string key,  T value, IEqualityComparer<T> comparer)
    {
        return args.TryGetValue(key, out var condition) && comparer.Equals((T)condition, value);
    }
    
    public static bool NotEquals<T>(this IDictionary<string, object> args, string key,  T value)
    {
        return NotEquals(args, key, value, EqualityComparer<T>.Default);
    }
    
    public static bool NotEquals<T>(this IDictionary<string, object> args, string key,  T value, IEqualityComparer<T> comparer)
    {
        return args.TryGetValue(key, out var condition) && !comparer.Equals((T)condition, value);
    }
}