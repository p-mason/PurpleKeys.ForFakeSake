namespace PurpleKeys.ForFakeSake;

/// <summary>
///     Fluid Helper for creating Fakes for Properties
/// </summary>
/// <typeparam name="TReturn">Type of the property.</typeparam>
public static class FakeProperty<TReturn> {
    /// <summary>
    ///     Creates a FakeCondition that applies to a Fake Property setup.
    /// </summary>
    /// <param name="condition">Predicate that must be met for a setup to be executed.</param>
    /// <returns>Instance of a FakeCondition.</returns>
    public static FakeCondition When(Func<bool> condition) {
        return new FakeCondition(_ => condition());
    }
}
