using System.Collections.Immutable;
using System.Text.RegularExpressions;
using Castle.DynamicProxy;

namespace PurpleKeys.ForFakeSake.Spy;

/// <summary>
///     Spies on invocations made against a Fake.
/// </summary>
public class SpyInterceptor : IInterceptor, IFakingSpy {
    private readonly List<SpyInvocation> _invocations = new();

    public void Reset() {
        _invocations.Clear();
    }

    public IEnumerable<SpyInvocation> AllInvocations() {
        return _invocations.ToImmutableArray();
    }

    public IEnumerable<SpyInvocation> AllInvocations<T>() {
        return _invocations
            .Where(i => i.Type == typeof(T))
            .ToImmutableArray();
    }

    public IEnumerable<SpyInvocation> MemberInvocations<T>(string memberName) {
        return _invocations
            .Where(i => i.Type == typeof(T) && i.MemberName == memberName)
            .ToImmutableArray();
    }

    public IEnumerable<SpyInvocation> SignatureInvocations<T>(string memberSignature) {
        return _invocations
            .Where(i => i.Type == typeof(T) && i.MemberSignature == memberSignature)
            .ToImmutableArray();
    }

    /// <summary>
    ///     Interceptor method used to spy on all Fake invocations.
    /// </summary>
    /// <param name="invocation">Call to member.</param>
    /// <exception cref="NotSupportedException">Thrown when a invocation can not be spied.</exception>
    public void Intercept(IInvocation invocation) {
        var argsDictionary = invocation.Method
            .GetParameters()
            .Zip(invocation.Arguments)
            .ToDictionary(
                k => k.First.Name ?? throw new NotSupportedException("Parameters Must Have a Name."),
                v => v.Second);

        Exception? threwException = null;
        try {
            invocation.Proceed();
        }
        catch (Exception ex) {
            threwException = ex;
            throw;
        }
        finally {
            var memberName = invocation.Method.Name;
            if (invocation.Method.IsSpecialName && invocation.Method.IsHideBySig) {
                var propertyNameMatch = Regex.Match(invocation.Method.Name, "(?<=^(get|set)_).*");
                if (propertyNameMatch.Success) {
                    memberName = propertyNameMatch.Value;
                }
            }

            var spy = new SpyInvocation(
                invocation.Method.DeclaringType,
                memberName,
                invocation.Method.ToString()!,
                argsDictionary,
                invocation.Method.ReturnType != typeof(void),
                invocation.ReturnValue,
                threwException);

            _invocations.Add(spy);
        }
    }
}
