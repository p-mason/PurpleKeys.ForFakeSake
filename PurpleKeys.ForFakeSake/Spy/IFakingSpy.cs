namespace PurpleKeys.ForFakeSake.Spy;

public interface IFakingSpy
{
    void Reset();
    IEnumerable<SpyInvocation> AllInvocations();
    IEnumerable<SpyInvocation> AllInvocations<T>();
    IEnumerable<SpyInvocation> MemberInvocations<T>(string methodName);
    IEnumerable<SpyInvocation> SignatureInvocations<T>(string methodSignature);
    
}