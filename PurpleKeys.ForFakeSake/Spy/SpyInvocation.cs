namespace PurpleKeys.ForFakeSake.Spy;

public record SpyInvocation(
    Type? Type,
    string MemberName,
    string MemberSignature,
    Dictionary<string, object> Arguments,
    bool ExpectsReturnValue,
    object? Result,
    Exception? ThrewException);