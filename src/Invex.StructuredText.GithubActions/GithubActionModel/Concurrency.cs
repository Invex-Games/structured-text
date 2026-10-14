namespace Invex.StructuredText.GithubActions.GithubActionModel;

/// <summary>
///     Concurrency settings (<c>concurrency</c>) that ensure only one run (or queued run) sharing the same
///     group executes at a time, at either workflow or job level.
/// </summary>
[PublicAPI]
public sealed record Concurrency
{
    /// <summary>
    ///     The concurrency group key. Runs with the same group are serialized.
    ///     Often built from expressions, e.g. <c>$"ci-{GitHub.Ref}"</c>.
    /// </summary>
    public required Template Group { get; init; } = "";

    /// <summary>
    ///     Whether to cancel an in-progress run of the same group when a new run starts
    ///     (<c>cancel-in-progress</c>).
    /// </summary>
    public Template? CancelInProgress { get; init; }
}
