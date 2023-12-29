using PurpleKeys.ForFakeSake.Extensions;
using PurpleKeys.ForFakeSake.Spy;

namespace PurpleKeys.ForFakeSake.UnitTest.SpyInterceptorTests;

public class InterceptInFakeCreatedWithBuilder : InterceptIn
{
    private static readonly SpyInterceptor SharedSpy;
    static InterceptInFakeCreatedWithBuilder()
    {
        SharedSpy = new SpyInterceptor();
        var fake = Fake<ITestInterface>
            .Builder()
            .WithSpy(SharedSpy)
            .Build();
        
        fake.Property = "TestProperty";
        var propertyValue = fake.Property;
        fake.OverloadAction("Arg1");
        fake.OverloadAction("Arg1", 2);
        fake.OverloadFunc("Arg1");
        fake.OverloadFunc("Arg1", 2);
    }

    public InterceptInFakeCreatedWithBuilder()
    {
        Spy = SharedSpy;
    }
}