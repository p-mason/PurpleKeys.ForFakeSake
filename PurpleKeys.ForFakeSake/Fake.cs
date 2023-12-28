using Castle.DynamicProxy;
using PurpleKeys.ForFakeSake.Spy;

namespace PurpleKeys.ForFakeSake;

public static class Fake<T>
    where T : class
{
    public static FakeBuilder<T> Builder()
    {
        return new FakeBuilder<T>();
    }
    
    public static T Empty()
    {
        return CreateEmpty(new FakeInterceptor<T>());
    }

    public static T EmptyWithSpy(SpyInterceptor spy)
    {
        return CreateEmpty(spy, new FakeInterceptor<T>());
    }

    private static T CreateEmpty(params IInterceptor[] interceptors)
    {
        var proxyGenerator = new ProxyGenerator();
        
        return typeof(T).IsInterface 
            ? proxyGenerator.CreateInterfaceProxyWithoutTarget<T>(ProxyGenerationOptions.Default, interceptors) 
            : proxyGenerator.CreateClassProxy<T>(ProxyGenerationOptions.Default, interceptors);
    }
}