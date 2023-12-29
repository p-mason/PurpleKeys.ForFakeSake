namespace PurpleKeys.ForFakeSake;

public static class FakeProperty<TReturn>
{
    public static FakeCondition<TReturn> When(Func<bool> condition)
    {
        return new FakeCondition<TReturn>(_ => condition());
    }
}