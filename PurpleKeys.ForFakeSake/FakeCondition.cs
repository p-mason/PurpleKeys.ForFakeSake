namespace PurpleKeys.ForFakeSake;

/// <summary>
///     Represents a condition for a FakeSetup that needs to be satisfied for the Fake to be applied
/// </summary>
public class FakeCondition {
    /// <summary>
    ///     Condition that is always satisfied.
    /// </summary>
    public static readonly FakeCondition Satisfied = new(_ => true);

    /// <summary>
    ///     Condition that is never satisfied.
    /// </summary>
    public static readonly FakeCondition NotSatisfied = new(_ => false);

    /// <summary>
    ///     Creates an instance of Fake Condition
    /// </summary>
    /// <param name="condition">Predicate to evaluate if a FakeSetup should be used or not.</param>
    public FakeCondition(Func<IDictionary<string, object>, bool> condition) {
        Condition = condition;
    }

    /// <summary>
    ///     Predicate to evaluation if the condition is met or not.
    /// </summary>
    public Func<IDictionary<string, object>, bool> Condition { get; }
}
