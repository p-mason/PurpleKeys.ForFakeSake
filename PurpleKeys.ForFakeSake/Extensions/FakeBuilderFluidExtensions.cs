using Castle.DynamicProxy;
using PurpleKeys.ForFakeSake.Spy;

namespace PurpleKeys.ForFakeSake.Extensions;

public static class FakeBuilderFluidExtensions
{
    [FakeBuilderHelper(ForPropertyGet = true)]
    public static P? Execute<T, P>(this FakeBuilder<T> builder, Func<P> func)
        where T: class 
    {
        return default;
    }

    /// <summary>
    /// Adds a Spy to the Builder.
    /// </summary>
    /// <param name="builder">Fake Builder.</param>
    /// <param name="spy">Spy instance.</param>
    /// <returns>The builder.</returns>
    public static FakeBuilder<T> WithSpy<T>(this FakeBuilder<T> builder, SpyInterceptor spy) 
        where T :class {
        
        return builder.WithInterceptor(spy);
    }
}