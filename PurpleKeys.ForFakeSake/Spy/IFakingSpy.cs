namespace PurpleKeys.ForFakeSake.Spy;

/// <summary>
///     Spy on invocations against a Fake.
/// </summary>
public interface IFakingSpy {
    /// <summary>
    ///     Resets recorded invocations.
    /// </summary>
    void Reset();

    /// <summary>
    ///     Returns all invocations recorded by the spy.
    /// </summary>
    /// <returns>Returns all invocations recorded by the spy.</returns>
    IEnumerable<SpyInvocation> AllInvocations();

    /// <summary>
    ///     Returns all invocations recorded by the spy for <typeparamref name="T" />.
    /// </summary>
    /// <returns>Returns all invocations recorded by the spy for <typeparamref name="T" />.</returns>
    IEnumerable<SpyInvocation> AllInvocations<T>();

    /// <summary>
    ///     Returns all invocations recorded by the spy for invocations of a member of <typeparamref name="T" />.
    /// </summary>
    /// <param name="memberName">Name of the member to return all invocations of, including all overloads.</param>
    /// <typeparam name="T">Type declaring the member.</typeparam>
    /// <returns>Returns all invocations recorded by the spy for invocations of a member of <typeparamref name="T" />.</returns>
    IEnumerable<SpyInvocation> MemberInvocations<T>(string memberName);

    /// <summary>
    ///     Returns all invocations recorded by the spy for invocations of a member signature of <typeparamref name="T" />.
    /// </summary>
    /// <param name="memberSignature">signature of the member to query</param>
    /// <typeparam name="T">Type declaring the member.</typeparam>
    /// <returns>
    ///     Returns all invocations recorded by the spy for invocations of a member with the
    ///     signature declared on <typeparamref name="T" />.
    /// </returns>
    IEnumerable<SpyInvocation> SignatureInvocations<T>(string memberSignature);
}
