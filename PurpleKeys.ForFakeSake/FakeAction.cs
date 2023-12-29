using System.Linq.Expressions;

namespace PurpleKeys.ForFakeSake;

public static class FakeAction
{
    public static Expression<Action<IDictionary<string, object>>> Throws(Exception exception)
    {
        var parameters = Expression.Parameter(typeof(IDictionary<string, object>), "args");
        var exp = Expression.Constant(exception);
        var body = Expression.Throw(exp);
        return Expression.Lambda<Action<IDictionary<string, object>>>(body, new []{parameters});
    }

    public static object Execute(Action<IDictionary<string, object>> action)
    {
        throw new NotImplementedException();
    }
}