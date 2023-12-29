namespace PurpleKeys.ForFakeSake;

/// <summary>
/// Attribute to determine how to setup for a Fake.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class FakeBuilderHelperAttribute : Attribute
{
    public bool ForPropertyGet { get; set; }
}