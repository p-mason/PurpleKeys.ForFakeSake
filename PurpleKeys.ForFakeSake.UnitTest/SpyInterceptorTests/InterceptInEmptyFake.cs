using PurpleKeys.ForFakeSake.Spy;

namespace PurpleKeys.ForFakeSake.UnitTest.SpyInterceptorTests;

public class InterceptInEmptyFake : InterceptIn
{
    private static readonly SpyInterceptor SharedSpy;
    static InterceptInEmptyFake()
    {
        SharedSpy = new SpyInterceptor();
        var fake = Fake<ITestInterface>.EmptyWithSpy(SharedSpy);
        
        fake.Property = "TestProperty";
        var propertyValue = fake.Property;
        fake.OverloadAction("Arg1");
        fake.OverloadAction("Arg1", 2);
        fake.OverloadFunc("Arg1");
        fake.OverloadFunc("Arg1", 2);
    }

    public InterceptInEmptyFake()
    {
        Spy = SharedSpy;
    }
}