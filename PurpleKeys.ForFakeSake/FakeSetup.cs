namespace PurpleKeys.ForFakeSake;

public class PropertyGetFakeSetup : FakeSetup
{
    public PropertyGetFakeSetup(
        Func<IDictionary<string, object>, bool> meetsCondition,
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
        Func<IDictionary<string, object>, bool> meetsCondition,
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
        Func<IDictionary<string, object>, bool> meetsCondition,
        bool hasReturnValue,
        Action<IDictionary<string, object>>? action,
        Func<IDictionary<string, object>, object>? func)
    {
        MeetsCondition = meetsCondition;
        HasReturnValue = hasReturnValue;
        Action = action;
        Func = func;
    }

    public Func<IDictionary<string, object>, bool> MeetsCondition { get; }
    public bool HasReturnValue { get; }
    public Action<IDictionary<string, object>>? Action { get; }
    public Func<IDictionary<string, object>, object>? Func { get; }
}