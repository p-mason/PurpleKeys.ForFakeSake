using PurpleKeys.ForFakeSake.Extensions;

namespace PurpleKeys.ForFakeSake.UnitTest.SetupConditionTests;

public class FunctionWithConditionalSetup
{
    private static readonly ITestInterface Fake;
    
    static FunctionWithConditionalSetup()
    {
        Fake = Fake<ITestInterface>
            .Builder()
            .FakeMethod("Func", FakeFunc<string>.WhenArgs(args => args.Equals("arg1", "BoB")).Returns("SetupValue"))
            .FakeMethod("Func", FakeFunc<string>.WhenArgs(args => args.Equals("arg1", "Dave")).Returns("SetupValue2"))
            .FakeMethodDefault("Func", _ => "Default")
            .Build();
    }
    
    [Fact]
    public void WhenConditionalSetupIsMet_ConditionalReturnValueIsReturned()
    {
        Assert.Equal("SetupValue", Fake.Func("BoB"));
        Assert.Equal("SetupValue2", Fake.Func("Dave"));
    }
    
    [Fact]
    public void WhenNoConditionalSetupIsMet_DefaultReturnValueIsReturned()
    {
        Assert.Equal("Default", Fake.Func("NoSetup"));
    }

    [Fact]
    public void ConditionCanIncludeClosure()
    {
        var fakeOn = false;
        var fake = Fake<ITestInterface>.Builder()
            .FakeMethod("Func", FakeFunc<string>.When(() => fakeOn).Returns("SetupValue"))
            .Build();
        
        Assert.Null(fake.Func("AnyValue"));
        fakeOn = true;
        Assert.Equal("SetupValue", fake.Func("AnyValue"));
    }
}

public class PropertiesWithConditionalSetup
{
    private static readonly ITest Fake;
    private static bool MeetsCondition = false;
    static PropertiesWithConditionalSetup()
    {
        Fake = Fake<ITest>
            .Builder()
            .FakeProperty("Property", FakeProperty<string>.When(() => MeetsCondition).GetReturns("SetupValue"))
            .FakeProperty("Property", FakeProperty<string>.When(() => MeetsCondition).SetExecutes(_ => throw new Exception()))
            .Build();
    }

    [Fact]
    public void PropertyGet_ConditionIsNotMet_ReturnsTypeDefault()
    {
        MeetsCondition = false;
        var actual = Fake.Property;
        Assert.Equal(null, actual);
    }
    
    [Fact]
    public void PropertyGet_ReturnsSetupValue()
    {
        MeetsCondition = true;
        var actual = Fake.Property;
        Assert.Equal("SetupValue", actual);
    }

    [Fact]
    public void PropertySet_ConditionNotMet_DoesNotExecuteSetup()
    {
        MeetsCondition = false;
        Fake.Property = "Test";
    }
    
    [Fact]
    public void PropertySet_ConditionIsMet_ExecutesSetup()
    {
        MeetsCondition = true;
        Assert.Throws<Exception>(() => Fake.Property = "Test");
    }
    
    public interface ITest
    {
        string Property { get; set; }
    }
}