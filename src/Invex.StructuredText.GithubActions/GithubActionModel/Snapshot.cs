namespace Invex.StructuredText.GithubActions.GithubActionModel;

/// <summary>
///     Runner image snapshot settings for a job (<c>snapshot</c>),
///     used to capture the runner state as a custom image after the job completes.
/// </summary>
[PublicAPI]
public sealed record Snapshot
{
    /// <summary>
    ///     The name of the image to create from the snapshot.
    /// </summary>
    public required TextExpression ImageName { get; init; }

    /// <summary>
    ///     The version to tag the snapshot image with.
    /// </summary>
    public TextExpression? Version { get; init; }
}
