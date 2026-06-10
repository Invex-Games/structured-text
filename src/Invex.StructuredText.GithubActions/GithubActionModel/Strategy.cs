namespace Invex.StructuredText.GithubActions.GithubActionModel;

/// <summary>
///     The execution strategy for a job (<c>strategy</c>), defining a matrix of variations to run.
/// </summary>
[PublicAPI]
public sealed record Strategy
{
    /// <summary>
    ///     The matrix of variable combinations to run the job with.
    /// </summary>
    public required Matrix Matrix { get; init; }

    /// <summary>
    ///     Whether to cancel all in-progress matrix jobs when any of them fails (<c>fail-fast</c>).
    ///     GitHub defaults to true when omitted.
    /// </summary>
    public TextExpression? FailFast { get; init; }

    /// <summary>
    ///     The maximum number of matrix jobs that may run simultaneously (<c>max-parallel</c>).
    /// </summary>
    public TextExpression? MaxParallel { get; init; }
}
