using PurpleKeys.ForFakeSake.Spy;

namespace PurpleKeys.ForFakeSake.UnitTest.SpyInterceptorTests;

public abstract class InterceptIn
{
    protected SpyInterceptor Spy;
    
    [Fact]
    public void EmptyMock_SpiesOn_PropertyRead()
    {
        var spiedInvocation = Spy.SignatureInvocations<ITestInterface>("System.String get_Property()").Single();
        Assert.NotNull(spiedInvocation);
    }
    
    [Fact]
    public void EmptyMock_SpiesOn_PropertyWrite()
    {
        var spiedInvocation = Spy.SignatureInvocations<ITestInterface>("Void set_Property(System.String)").Single();
        Assert.NotNull(spiedInvocation);
    }
    
    [Fact]
    public void EmptyMock_SpiesOn_OverloadedAction()
    {
        var spiedInvocation = Spy.SignatureInvocations<ITestInterface>("Void OverloadAction(System.String)").Single();
        Assert.NotNull(spiedInvocation);
        
        spiedInvocation = Spy.SignatureInvocations<ITestInterface>("Void OverloadAction(System.String, Int32)").Single();
        Assert.NotNull(spiedInvocation);
    }
}