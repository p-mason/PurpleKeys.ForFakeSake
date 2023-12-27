namespace PurpleKeys.ForFakeSake;

[AttributeUsage(AttributeTargets.Method)]
public class FakeBuilderHelperAttribute : Attribute
{
    public bool ForPropertyGet { get; set; }
}