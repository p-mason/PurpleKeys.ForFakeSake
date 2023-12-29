using Castle.DynamicProxy;
using PurpleKeys.ForFakeSake.Spy;

namespace PurpleKeys.ForFakeSake.UnitTest.SpyInterceptorTests;

public class InterceptFunctions
{
    private static readonly SpyInterceptor Spy = new();
    private static string _resultFromFuncOverload1;
    private static string _resultFromFuncOverload2;
    
    static InterceptFunctions()
    {
        var fake = Fake<ITestInterface>.Builder()
            .AllFunctionOverloads("OverloadFunc", args => string.Join(',', args.Select(i =>$"{i.Key}:{i.Value}")))
            .Build();
        
        var proxy = new ProxyGenerator().CreateInterfaceProxyWithTarget(fake, Spy);
        _resultFromFuncOverload1 = proxy.OverloadFunc("Arg1");
        _resultFromFuncOverload2 = proxy.OverloadFunc("Arg1", 2);
    }
    
    [Fact]
    public void SignatureInvocations_OverloadActionWithOneArgumentIsRecorded()
    {
        var actual = Spy.SignatureInvocations<ITestInterface>("System.String OverloadFunc(System.String)").Single();

        Assert.Equal("OverloadFunc", actual.MemberName);
        Assert.Equal("System.String OverloadFunc(System.String)", actual.MemberSignature);
        Assert.Single(actual.Arguments);
        Assert.Equal("Arg1", actual.Arguments["arg1"]);
        Assert.Equal("arg1:Arg1", actual.Result);
        Assert.Equal(_resultFromFuncOverload1, actual.Result);
        Assert.True(actual.ExpectsReturnValue);
        Assert.Null(actual.ThrewException);
    }
    
    [Fact]
    public void SignatureInvocations_OverloadActionWithTwoArgumentIsRecorded()
    {
        var actual = Spy.SignatureInvocations<ITestInterface>("System.String OverloadFunc(System.String, Int32)").Single();

        Assert.Equal("OverloadFunc", actual.MemberName);
        Assert.Equal("System.String OverloadFunc(System.String, Int32)", actual.MemberSignature);
        Assert.Equal("Arg1", actual.Arguments["arg1"]);
        Assert.Equal(2, actual.Arguments["arg2"]);
        Assert.Equal("arg1:Arg1,arg2:2", actual.Result);
        Assert.Equal(_resultFromFuncOverload2, actual.Result);
        Assert.True(actual.ExpectsReturnValue);
        Assert.Null(actual.ThrewException);
    }

    [Fact]
    public void MemberInvocations_AllInvocatiosAreRecordedInOrder()
    {
        var actual = Spy.MemberInvocations<ITestInterface>("OverloadFunc").ToArray();
        
        Assert.Equal(2, actual.Length);
        
        Assert.Equal("System.String OverloadFunc(System.String)", actual[0].MemberSignature);
        Assert.Equal("System.String OverloadFunc(System.String, Int32)", actual[1].MemberSignature);
    }
}