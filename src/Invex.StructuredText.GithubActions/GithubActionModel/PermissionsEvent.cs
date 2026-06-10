namespace Invex.StructuredText.GithubActions.GithubActionModel;

/// <summary>
///     Per-scope <c>GITHUB_TOKEN</c> permission levels. Scopes left <c>null</c> are omitted from the YAML
///     and fall back to GitHub's defaults.
/// </summary>
[PublicAPI]
public sealed record PermissionsEvent
{
    /// <summary>
    ///     Permission for GitHub Actions workflows, runs, and artifacts (<c>actions</c>).
    /// </summary>
    public PermissionsLevel? Actions { get; init; }

    /// <summary>
    ///     Permission for artifact attestations (<c>attestations</c>).
    /// </summary>
    public PermissionsLevel? Attestations { get; init; }

    /// <summary>
    ///     Permission for check runs and check suites (<c>checks</c>).
    /// </summary>
    public PermissionsLevel? Checks { get; init; }

    /// <summary>
    ///     Permission for repository contents, commits, branches, tags, and releases (<c>contents</c>).
    /// </summary>
    public PermissionsLevel? Contents { get; init; }

    /// <summary>
    ///     Permission for deployments and deployment statuses (<c>deployments</c>).
    /// </summary>
    public PermissionsLevel? Deployments { get; init; }

    /// <summary>
    ///     Permission to request an OpenID Connect token (<c>id-token</c>).
    /// </summary>
    public PermissionsLevel? IdTokens { get; init; }

    /// <summary>
    ///     Permission for issues and related comments, labels, and milestones (<c>issues</c>).
    /// </summary>
    public PermissionsLevel? Issues { get; init; }

    /// <summary>
    ///     Permission for discussions and related comments (<c>discussions</c>).
    /// </summary>
    public PermissionsLevel? Discussions { get; init; }

    /// <summary>
    ///     Permission for GitHub Packages (<c>packages</c>).
    /// </summary>
    public PermissionsLevel? Packages { get; init; }

    /// <summary>
    ///     Permission for GitHub Pages builds (<c>pages</c>).
    /// </summary>
    public PermissionsLevel? Pages { get; init; }

    /// <summary>
    ///     Permission for pull requests and related comments, labels, and reviews (<c>pull-requests</c>).
    /// </summary>
    public PermissionsLevel? PullRequests { get; init; }

    /// <summary>
    ///     Permission for classic repository projects (<c>repository-projects</c>).
    /// </summary>
    public PermissionsLevel? RepositoryProjects { get; init; }

    /// <summary>
    ///     Permission for code scanning and Dependabot alerts (<c>security-events</c>).
    /// </summary>
    public PermissionsLevel? SecurityEvents { get; init; }

    /// <summary>
    ///     Permission for commit statuses (<c>statuses</c>).
    /// </summary>
    public PermissionsLevel? Statuses { get; init; }

    /// <summary>
    ///     Returns <c>true</c> when every scope is set to <paramref name="permission" />,
    ///     in which case the writer can emit the compact <c>read-all</c> / <c>write-all</c> form.
    /// </summary>
    /// <param name="permission">The level to compare every scope against.</param>
    public bool IsAll(PermissionsLevel permission) =>
        Actions == permission &&
        Attestations == permission &&
        Checks == permission &&
        Contents == permission &&
        Deployments == permission &&
        IdTokens == permission &&
        Issues == permission &&
        Discussions == permission &&
        Packages == permission &&
        Pages == permission &&
        PullRequests == permission &&
        RepositoryProjects == permission &&
        SecurityEvents == permission &&
        Statuses == permission;
}
