using Castle.DynamicProxy;
using PurpleKeys.ForFakeSake.Spy;

namespace PurpleKeys.ForFakeSake.UnitTest.SpyInterceptorTests;

public class InterceptProperty
{
    private static ITestInterface _proxy;
    private static IFakingSpy _spy;
    
    static InterceptProperty()
    {
        var spy = new SpyInterceptor();
        _spy = spy;
        
        var fake = Fake<ITestInterface>.Builder()
            .StubProperties(p => p.Property == "TestProperty")
            .Build();
        
        _proxy = new ProxyGenerator().CreateInterfaceProxyWithTarget(fake, spy);
        _proxy.Property = "TestWrite";
        var propertyRead = _proxy.Property;
    }

    [Fact]
    public void SignatureInvocations_PropertyReadIsRecorded()
    {
        var actual = _spy.SignatureInvocations<ITestInterface>("System.String get_Property()").Single();

        Assert.Equal("Property", actual.MemberName);
        Assert.Equal("System.String get_Property()", actual.MemberSignature);
        Assert.Empty(actual.Arguments);
        Assert.Equal("TestProperty", actual.Result);
        Assert.True(actual.ExpectsReturnValue);
        Assert.Null(actual.ThrewException);
    }
    
    [Fact]
    public void SignatureInvocations_PropertyWriteIsRecorded()
    {
        var actual = _spy.SignatureInvocations<ITestInterface>("Void set_Property(System.String)").Single();

        Assert.Equal("Property", actual.MemberName);
        Assert.Equal("Void set_Property(System.String)", actual.MemberSignature);
        Assert.Equal("TestWrite", actual.Arguments["value"]);
        Assert.False(actual.ExpectsReturnValue);
        Assert.Null(actual.ThrewException);
    }
    
    [Fact]
    public void MemberInvocations_PropertyReadAndWriteAreRecordedInOrder()
    {
        var actual = _spy.MemberInvocations<ITestInterface>("Property").ToArray();

        Assert.Equal(2, actual.Length);
        
        // Assert Write
        Assert.Equal("Property", actual[0].MemberName);
        Assert.Equal("Void set_Property(System.String)", actual[0].MemberSignature);
        Assert.Equal("TestWrite", actual[0].Arguments["value"]);
        
        // Assert Read
        Assert.Equal("Property", actual[1].MemberName);
        Assert.Equal("System.String get_Property()", actual[1].MemberSignature);
        Assert.Empty(actual[1].Arguments);
    }
    
    [Fact]
    public void AllInvocations_IncludesAllInvocations()
    {
        Assert.Equal(2, _spy.AllInvocations().Count());
    }
}