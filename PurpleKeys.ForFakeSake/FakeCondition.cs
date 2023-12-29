namespace PurpleKeys.ForFakeSake;

public class FakeCondition<T>
{
    public Func<IDictionary<string, object>, bool> Condition { get; }

    public FakeCondition(Func<IDictionary<string, object>, bool>condition)
    {
        Condition = condition;
    }
}