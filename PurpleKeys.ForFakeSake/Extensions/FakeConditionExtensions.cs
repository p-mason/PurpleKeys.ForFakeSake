namespace PurpleKeys.ForFakeSake.Extensions;

public static class FakeConditionExtensions
{
    public static FakeSetup Returns<T>(this FakeCondition condition, T returnValue)
    {
        if (typeof(T) == typeof(void))
        {
            throw new InvalidOperationException("Trying to setup a return on a Void Member");
        }
        return new FakeSetup(condition, true, null, (_) => returnValue);
    }
    
    public static FakeSetup GetReturns<T>(this FakeCondition condition, T returnValue)
    {
        if (typeof(T) == typeof(void))
        {
            throw new InvalidOperationException("Trying to setup a return on a Void Member");
        }
        return new PropertyGetFakeSetup(condition, (_) => returnValue);
    }
    
    public static FakeSetup SetExecutes(this FakeCondition condition, Action<object> executes)
    {
        return new PropertySetFakeSetup(condition, executes);
    }
    
    public static FakeSetup Returns<T>(this FakeCondition condition, Func<IDictionary<string, object>, object> returnValue)
    {
        if (typeof(T) == typeof(void))
        {
            throw new InvalidOperationException("Trying to setup a return on a Void Member");
        }
        return new FakeSetup(condition, true, null, returnValue);
    }

    public static FakeSetup Execute<T>(this FakeCondition condition, Action<IDictionary<string, object>> executes)
    {
        if (typeof(T) != typeof(void))
        {
            throw new InvalidOperationException("Trying to setup an execute on a Void Member, use a return.");
        }
        
        return new FakeSetup(condition, false, executes, null);
    }
}