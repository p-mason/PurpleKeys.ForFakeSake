// ReSharper disable AccessToModifiedClosure

using PurpleKeys.ForFakeSake.Extensions;

namespace PurpleKeys.ForFakeSake.UnitTest;

public class FakingInterfaceProperties
{
    [Fact]
    public void PropertiesReturnDefaultValues()
    {
        var fake = Fake<ITestInterface>.Empty();
        
        Assert.IsAssignableFrom<ITestInterface>(fake);
        Assert.Null(fake.ReferenceProperty);
        Assert.Equal(0, fake.ValueProperty);
    }

    [Fact]
    public void PropertiesWithoutSetupBehaveAsSimpleProperties()
    {
        var fake = Fake<ITestInterface>.Empty();
        fake.ValueProperty = 5;
        
        Assert.Equal(5, fake.ValueProperty);
    }

    [Fact]
    public void StubPropertiesAlwaysReturnsSetupValue()
    {
        var fake = Fake<ITestInterface>.Builder()
            .StubProperties(t => t.ReferenceProperty == "AlwaysReturnsThis")
            .Build();
        
        Assert.Equal("AlwaysReturnsThis", fake.ReferenceProperty);
        fake.ReferenceProperty = "Something Different";
        Assert.Equal("AlwaysReturnsThis", fake.ReferenceProperty);
    }

    [Fact]
    public void WritingToThePropertyAlwaysThrowsAnException()
    {
        var fake = Fake<ITestInterface>.Builder()
            .PropertySetExecutes(nameof(ITestInterface.ReferenceProperty), (_) => throw new InvalidDataException("Test"))
            .Build();

        Assert.Throws<InvalidDataException>(() => fake.ReferenceProperty = "Something");
    }

    [Fact]
    public void ManyProperties()
    {
        var fake = Fake<ITestInterface>.Builder()
            .StubProperties(i =>
                i.ReferenceProperty == "Test" &
                i.ValueProperty == 1 &
                i.ValueProperty2 == 2
            ).Build();
        
        Assert.Equal("Test", fake.ReferenceProperty);
        Assert.Equal(1, fake.ValueProperty);
        Assert.Equal(2, fake.ValueProperty2);
    }
    
    [Fact]
    public void PropertiesReturnResultOfFunctionSupportingCaptures()
    {
        var baseNumber = 1;
        var fake = Fake<ITestInterface>.Builder()
            .StubProperties((i, builder) =>
                i.ValueProperty == builder.Execute(() => baseNumber + 1) &
                i.ValueProperty2 == builder.Execute(() => baseNumber + 2) &
                i.ReferenceProperty == builder.Execute(() => $"BaseNumber:{baseNumber}")
            ).Build();
        
        Assert.Equal(2, fake.ValueProperty);
        Assert.Equal(3, fake.ValueProperty2);
        Assert.Equal("BaseNumber:1", fake.ReferenceProperty);
        baseNumber = 2;
        Assert.Equal(3, fake.ValueProperty);
        Assert.Equal(4, fake.ValueProperty2);
        Assert.Equal("BaseNumber:2", fake.ReferenceProperty);
    }
    
    // ReSharper disable once MemberCanBePrivate.Global
    public interface ITestInterface
    {
        string ReferenceProperty { get; set; }
        int ValueProperty { get; set; }
        int ValueProperty2 { get; set; }
    }
}