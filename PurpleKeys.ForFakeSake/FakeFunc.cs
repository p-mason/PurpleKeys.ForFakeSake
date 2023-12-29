using System.Linq.Expressions;

namespace PurpleKeys.ForFakeSake;

public static class FakeFunc<TReturn>
{
    public static Expression<Func<IDictionary<string, object>,TReturn>> Throws(Exception exception)
    {
        var parameters = Expression.Parameter(typeof(IDictionary<string, object>), "args");
        var def =Expression.Convert(Expression.Constant(default(TReturn)), typeof(TReturn));
        var exp = Expression.Constant(exception);
        var body = Expression.Block(Expression.Throw(exp), def);
        return Expression.Lambda<Func<IDictionary<string, object>, TReturn>>(body, new []{parameters});
    }

    public static FakeCondition<TReturn> WhenArgs(Func<IDictionary<string, object>, bool> condition)
    {
        return new FakeCondition<TReturn>(condition);
    }

    public static FakeCondition<TReturn> When(Func<bool> condition)
    {
        return new FakeCondition<TReturn>(_ => condition());
    }
}