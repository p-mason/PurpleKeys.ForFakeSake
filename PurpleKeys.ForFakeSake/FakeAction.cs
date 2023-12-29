using System.Linq.Expressions;

namespace PurpleKeys.ForFakeSake;

/// <summary>
/// Helps Registration of Fake Actions
/// </summary>
public static class FakeAction
{
    /// <summary>
    /// Register a Action Fake that throws an exception when called.
    /// </summary>
    /// <param name="exception">Exception to throw when called.</param>
    /// <returns>Lambda that throws an exception</returns>
    public static Expression<Action<IDictionary<string, object>>> Throws(Exception exception)
    {
        var parameters = Expression.Parameter(typeof(IDictionary<string, object>), "args");
        var exp = Expression.Constant(exception);
        var body = Expression.Throw(exp);
        return Expression.Lambda<Action<IDictionary<string, object>>>(body, parameters);
    }
}