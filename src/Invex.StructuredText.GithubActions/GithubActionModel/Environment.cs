namespace Invex.StructuredText.GithubActions.GithubActionModel;

/// <summary>
///     The deployment environment a job targets (<c>environment</c>).
///     Environments can carry protection rules and secrets configured in the repository settings.
/// </summary>
[PublicAPI]
public sealed record Environment
{
    /// <summary>
    ///     The name of the environment, e.g. <c>production</c>.
    /// </summary>
    public required Template Name { get; init; }

    /// <summary>
    ///     The URL shown for the deployment in the GitHub UI (<c>url</c>).
    /// </summary>
    public Template? UrlValue { get; init; }
}
