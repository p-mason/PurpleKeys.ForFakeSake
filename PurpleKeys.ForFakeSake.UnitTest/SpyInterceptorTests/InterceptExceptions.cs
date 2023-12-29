using Castle.DynamicProxy;
using PurpleKeys.ForFakeSake.Spy;

namespace PurpleKeys.ForFakeSake.UnitTest.SpyInterceptorTests;

public class InterceptExceptions
{
    private static readonly SpyInterceptor Spy = new();
    
    static InterceptExceptions()
    {
        var fake = Fake<ITestInterface>
            .Builder()
            .AllActionOverloads("OverloadAction", FakeAction.Throws(new Exception("Boom")))
            .AllFunctionOverloads("OverloadFunc", FakeFunc<string>.Throws(new Exception("Boom")))
            .Build();

        var proxy = new ProxyGenerator().CreateInterfaceProxyWithTarget(fake, Spy);

        Assert.Throws<Exception>(() => proxy.OverloadAction("Boom"));
        Assert.Throws<Exception>(() => proxy.OverloadAction("Boom", 2));
        
        Assert.Throws<Exception>(() => proxy.OverloadFunc("Boom"));
        Assert.Throws<Exception>(() => proxy.OverloadFunc("Boom", 2));
    }

    [Fact]
    public void ActionThrowsException_AllInvocationsAreRecordedWithException()
    {   
        var invocations = Spy.MemberInvocations<ITestInterface>("OverloadAction");
        Assert.Equal(2, invocations.Count());
        Assert.All(invocations, i => Assert.NotNull(i.ThrewException));
    }
    
    [Fact]
    public void FuncThrowsException_AllInvocationsAreRecordedWithException()
    {
        var invocations = Spy.MemberInvocations<ITestInterface>("OverloadFunc");
        
        Assert.Equal(2, invocations.Count());
        Assert.All(invocations, i => Assert.NotNull(i.ThrewException));
    }
}

public class InterceptState
{
    [Fact]
    public void Reset_ClearsPreviousRecords()
    {
        var fake = Fake<ITestInterface>.Empty();
        var spy = new SpyInterceptor();
        var proxy = new ProxyGenerator().CreateInterfaceProxyWithTarget(fake, spy);

        proxy.Property = "TestProperty";
        Assert.Single(spy.AllInvocations());
        
        spy.Reset();
        Assert.Empty(spy.AllInvocations());
        
        proxy.Property = "TestProperty";
        Assert.Single(spy.AllInvocations());
    }
}