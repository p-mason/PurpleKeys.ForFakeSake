using Castle.DynamicProxy;
using PurpleKeys.ForFakeSake.Spy;

namespace PurpleKeys.ForFakeSake;

/// <summary>
/// Create Fakes or a Builder to create a Fake of type <typeparamref name="T" />
/// </summary>
/// <typeparam name="T">Type to fake.</typeparam>
public static class Fake<T>
    where T : class
{
    /// <summary>
    /// Creates a Fake Builder to customize a fake instance.
    /// </summary>
    /// <typeparam name="T">Type to fake.</typeparam>
    /// <returns>An instance of a Fake Builder</returns>
    public static FakeBuilder<T> Builder()
    {
        return new FakeBuilder<T>();
    }
    
    /// <summary>
    /// Creates an Empty Fake. Properties work as simple properties and are initialized to their types default.
    /// Functions return the return type default. 
    /// </summary>
    /// <typeparam name="T">Type of Fake to create</typeparam>
    /// <returns>A Fake of type <typeparamref name="T"/></returns>
    public static T Empty()
    {
        return CreateEmpty(new FakeInterceptor<T>());
    }

    /// <summary>
    /// Creates an Empty Fake, but with a Spy for invocations of it's members.
    /// </summary>
    /// <param name="spy">Spy Instance.</param>
    /// <typeparam name="T">Type of Fake to create</typeparam>
    /// <returns>A Fake of type <typeparamref name="T"/> that is spied on.</returns>
    public static T EmptyWithSpy(SpyInterceptor spy)
    {
        return EmptyWithInterceptors(spy);
    }
    
    /// <summary>
    /// Creates an Empty Fake, with additional Interceptors.
    /// </summary>
    /// <param name="interceptors">Interceptors to add to the Proxy.</param>
    /// <typeparam name="T">Type of Fake to create</typeparam>
    /// <returns>A Fake of type <typeparamref name="T"/> created with additional interceptors.</returns>
    public static T EmptyWithInterceptors(params IInterceptor[] interceptors) {
        var interceptorsForFake = interceptors.Union(new[] { new FakeInterceptor<T>() }).ToArray();
        return CreateEmpty(interceptorsForFake);
    }

    private static T CreateEmpty(params IInterceptor[] interceptors)
    {
        var proxyGenerator = new ProxyGenerator();
        
        return typeof(T).IsInterface 
            ? proxyGenerator.CreateInterfaceProxyWithoutTarget<T>(ProxyGenerationOptions.Default, interceptors) 
            : proxyGenerator.CreateClassProxy<T>(ProxyGenerationOptions.Default, interceptors);
    }
}