namespace Invex.StructuredText.GithubActions.GithubActionModel;

/// <summary>
///     A matrix of variable combinations to run a job with (<c>strategy.matrix</c>).
/// </summary>
[PublicAPI]
public sealed record Matrix
{
    /// <summary>
    ///     The matrix variables: each key is a variable name and each value the list of values to combine.
    ///     The job runs once per combination.
    /// </summary>
    public IReadOnlyDictionary<string, TextExpressionCollection>? Map { get; init; }

    /// <summary>
    ///     Extra combinations to add to (or extend) the generated matrix (<c>include</c>).
    /// </summary>
    public IReadOnlyList<IReadOnlyDictionary<string, TextExpression>>? Include { get; init; }

    /// <summary>
    ///     Combinations to remove from the generated matrix (<c>exclude</c>).
    /// </summary>
    public IReadOnlyList<IReadOnlyDictionary<string, TextExpression>>? Exclude { get; init; }
}
