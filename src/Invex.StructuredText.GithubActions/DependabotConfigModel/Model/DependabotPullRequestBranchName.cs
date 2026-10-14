namespace Invex.StructuredText.GithubActions.DependabotConfigModel.Model;

/// <summary>
///     Pull request branch name preferences.
/// </summary>
[PublicAPI]
public sealed record DependabotPullRequestBranchName
{
    /// <summary>
    ///     Change separator for PR branch name.
    /// </summary>
    public required BranchNameSeparator Separator { get; init; }
}

/// <summary>
///     Branch name separator.
/// </summary>
[PublicAPI]
public enum BranchNameSeparator
{
    /// <summary>
    ///     Separates branch name parts with a hyphen (<c>-</c>).
    /// </summary>
    Hyphen,

    /// <summary>
    ///     Separates branch name parts with an underscore (<c>_</c>).
    /// </summary>
    Underscore,

    /// <summary>
    ///     Separates branch name parts with a forward slash (<c>/</c>), the Dependabot default.
    /// </summary>
    Slash,
}
