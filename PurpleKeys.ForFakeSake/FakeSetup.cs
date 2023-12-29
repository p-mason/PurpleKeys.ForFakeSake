namespace PurpleKeys.ForFakeSake;

public class PropertyGetFakeSetup : FakeSetup
{
    public PropertyGetFakeSetup(
        FakeCondition meetsCondition,
        Func<IDictionary<string, object>, object>? func) 
        : base(meetsCondition,
        true,
        null,
        func)
    {
    }
}

public class PropertySetFakeSetup : FakeSetup
{
    public PropertySetFakeSetup(
        FakeCondition meetsCondition,
        Action<IDictionary<string, object>>? action) 
        : base(meetsCondition,
        false,
        action,
        null)
    {
    }
}

public class FakeSetup
{
    public FakeSetup(
        FakeCondition meetsCondition,
        bool hasReturnValue,
        Action<IDictionary<string, object>>? action,
        Func<IDictionary<string, object>, object>? func)
    {
        MeetsCondition = meetsCondition;
        HasReturnValue = hasReturnValue;
        Action = action;
        Func = func;
    }

    public FakeCondition MeetsCondition { get; }
    public bool HasReturnValue { get; }
    public Action<IDictionary<string, object>>? Action { get; }
    public Func<IDictionary<string, object>, object>? Func { get; }
}