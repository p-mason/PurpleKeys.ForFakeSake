namespace PurpleKeys.ForFakeSake.Extensions;

public static class FakeConditionExtensions
{
    public static FakeSetup Returns<T>(this FakeCondition<T> condition, T returnValue)
    {
        if (typeof(T) == typeof(void))
        {
            throw new InvalidOperationException("Trying to setup a return on a Void Member");
        }
        return new FakeSetup(condition.Condition, true, null, (_) => returnValue);
    }
    
    public static FakeSetup GetReturns<T>(this FakeCondition<T> condition, T returnValue)
    {
        if (typeof(T) == typeof(void))
        {
            throw new InvalidOperationException("Trying to setup a return on a Void Member");
        }
        return new PropertyGetFakeSetup(condition.Condition, (_) => returnValue);
    }
    
    public static FakeSetup SetExecutes<T>(this FakeCondition<T> condition, Action<object> executes)
    {
        return new PropertySetFakeSetup(condition.Condition, executes);
    }
    
    public static FakeSetup Returns<T>(this FakeCondition<T> condition, Func<IDictionary<string, object>, object> returnValue)
    {
        if (typeof(T) == typeof(void))
        {
            throw new InvalidOperationException("Trying to setup a return on a Void Member");
        }
        return new FakeSetup(condition.Condition, true, null, returnValue);
    }

    public static FakeSetup Execute<T>(this FakeCondition<T> condition, Action<IDictionary<string, object>> executes)
    {
        if (typeof(T) != typeof(void))
        {
            throw new InvalidOperationException("Trying to setup an execute on a Void Member, use a return.");
        }
        
        return new FakeSetup(condition.Condition, false, executes, null);
    }
}