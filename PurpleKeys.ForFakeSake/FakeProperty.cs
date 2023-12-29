namespace PurpleKeys.ForFakeSake;

public static class FakeProperty<TReturn>
{
    public static FakeCondition When(Func<bool> condition)
    {
        return new FakeCondition(_ => condition());
    }
}