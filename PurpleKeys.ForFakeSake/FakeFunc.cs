using System.Linq.Expressions;

namespace PurpleKeys.ForFakeSake;

/// <summary>
/// Fluid Helper for creating Fakes for Functions
/// </summary>
/// <typeparam name="TReturn">Return type of the function.</typeparam>
public static class FakeFunc<TReturn>
{
    /// <summary>
    /// Creates an Expression that throws an exception.
    /// </summary>
    /// <param name="exception">Exception to throw</param>
    /// <returns>Expression that throws an exception.</returns>
    public static Expression<Func<IDictionary<string, object>,TReturn>> Throws(Exception exception)
    {
        var parameters = Expression.Parameter(typeof(IDictionary<string, object>), "args");
        var def =Expression.Convert(Expression.Constant(default(TReturn)), typeof(TReturn));
        var exp = Expression.Constant(exception);
        var body = Expression.Block(Expression.Throw(exp), def);
        return Expression.Lambda<Func<IDictionary<string, object>, TReturn>>(body, new []{parameters});
    }

    /// <summary>
    /// Creates a FakeCondition that applies to a Fake Function setup with access to invocation arguments.
    /// </summary>
    /// <param name="condition">Predicate that must be met for a setup to be executed.</param>
    /// <returns>Instance of a FakeCondition.</returns>
    public static FakeCondition WhenArgs(Func<IDictionary<string, object>, bool> condition)
    {
        return new FakeCondition(condition);
    }

    /// <summary>
    /// Creates a FakeCondition that applies to a Fake Function setup.
    /// </summary>
    /// <param name="condition">Predicate that must be met for a setup to be executed.</param>
    /// <returns>Instance of a FakeCondition.</returns>
    public static FakeCondition When(Func<bool> condition)
    {
        return new FakeCondition(_ => condition());
    }
}