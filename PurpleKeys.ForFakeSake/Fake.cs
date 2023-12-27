using Castle.DynamicProxy;

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
        var proxyGenerator = new ProxyGenerator();
        var interceptor = new FakeInterceptor<T>();
        
        return typeof(T).IsInterface 
            ? proxyGenerator.CreateInterfaceProxyWithoutTarget<T>(ProxyGenerationOptions.Default, interceptor) 
            : proxyGenerator.CreateClassProxy<T>(ProxyGenerationOptions.Default, interceptor);
    }
}