using Castle.DynamicProxy;
using PurpleKeys.ForFakeSake.Spy;

namespace PurpleKeys.ForFakeSake.UnitTest.SpyInterceptorTests;

public class InterceptActions
{
    private static readonly IFakingSpy Spy;
    
    static InterceptActions()
    {
        var spy = new SpyInterceptor();
        Spy = spy;
        
        var fake = Fake<ITestInterface>.Builder().Build();
        
        var proxy = new ProxyGenerator().CreateInterfaceProxyWithTarget(fake, spy);
        proxy.OverloadAction("Arg1");
        proxy.OverloadAction("Arg1", 2);
    }
    
    [Fact]
    public void SignatureInvocations_OverloadActionWithOneArgumentIsRecorded()
    {
        var actual = Spy.SignatureInvocations<ITestInterface>("Void OverloadAction(System.String)").Single();

        Assert.Equal("OverloadAction", actual.MemberName);
        Assert.Equal("Void OverloadAction(System.String)", actual.MemberSignature);
        Assert.Single(actual.Arguments);
        Assert.Equal("Arg1", actual.Arguments["arg1"]);
        Assert.Null(actual.Result);
        Assert.False(actual.ExpectsReturnValue);
        Assert.Null(actual.ThrewException);
    }
    
    [Fact]
    public void SignatureInvocations_OverloadActionWithTwoArgumentIsRecorded()
    {
        var actual = Spy.SignatureInvocations<ITestInterface>("Void OverloadAction(System.String, Int32)").Single();

        Assert.Equal("OverloadAction", actual.MemberName);
        Assert.Equal("Void OverloadAction(System.String, Int32)", actual.MemberSignature);
        Assert.Equal(2, actual.Arguments.Count);
        Assert.Equal("Arg1", actual.Arguments["arg1"]);
        Assert.Equal(2, (int)actual.Arguments["arg2"]);
        Assert.Null(actual.Result);
        Assert.False(actual.ExpectsReturnValue);
        Assert.Null(actual.ThrewException);
    }

    [Fact]
    public void MemberInvocations_AllInvocationsAreRecordedInOrder()
    {
        var actual = Spy.MemberInvocations<ITestInterface>("OverloadAction").ToArray();
        
        Assert.Equal(2, actual.Length);
        
        Assert.Equal("Void OverloadAction(System.String)", actual[0].MemberSignature);
        Assert.Equal("Void OverloadAction(System.String, Int32)", actual[1].MemberSignature);
    }
}