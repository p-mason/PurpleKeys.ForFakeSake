namespace PurpleKeys.ForFakeSake;

public static class FakeBuilderFluidExtensions
{
    [FakeBuilderHelper(ForPropertyGet = true)]
    public static P? Execute<T, P>(this FakeBuilder<T> builder, Func<P> func)
        where T: class 
    {
        return default;
    }
}