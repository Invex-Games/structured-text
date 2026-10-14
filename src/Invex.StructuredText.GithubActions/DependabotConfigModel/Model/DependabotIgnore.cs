namespace Invex.StructuredText.GithubActions.DependabotConfigModel.Model;

/// <summary>
///     Ignore rule for Dependabot updates.
/// </summary>
[PublicAPI]
public sealed record DependabotIgnore
{
    /// <summary>
    ///     Use to ignore updates for dependencies with matching names, optionally using * to match zero or more characters.
    /// </summary>
    public required string? DependencyName { get; init; }

    /// <summary>
    ///     Use to ignore types of updates.
    /// </summary>
    public required IReadOnlyList<SemverUpdateType>? UpdateTypes { get; init; }

    /// <summary>
    ///     Use to ignore specific versions or ranges of versions.
    /// </summary>
    public required DependabotVersions? Versions { get; init; }
}

/// <summary>
///     Semantic version update type.
/// </summary>
[PublicAPI]
public enum SemverUpdateType
{
    /// <summary>
    ///     Ignores updates that change the major version (<c>version-update:semver-major</c>).
    /// </summary>
    VersionUpdateSemverMajor,

    /// <summary>
    ///     Ignores updates that change the minor version (<c>version-update:semver-minor</c>).
    /// </summary>
    VersionUpdateSemverMinor,

    /// <summary>
    ///     Ignores updates that change only the patch version (<c>version-update:semver-patch</c>).
    /// </summary>
    VersionUpdateSemverPatch,
}

/// <summary>
///     Versions to ignore. Can be a single version string or a list of version strings.
/// </summary>
[PublicAPI]
[Union]
public partial record DependabotVersions
{
    /// <summary>
    ///     A single version or version range to ignore, written as a one-item <c>versions</c> array.
    /// </summary>
    /// <param name="Version">The version or version range, in the package manager's own syntax.</param>
    public partial record Single(string Version);

    /// <summary>
    ///     A list of versions or version ranges to ignore, written as a <c>versions</c> array.
    /// </summary>
    /// <param name="Versions">The versions or version ranges, in the package manager's own syntax.</param>
    public partial record Multiple(IReadOnlyList<string> Versions);
}
