namespace Invex.StructuredText.GithubActions.GithubActionModel;

/// <summary>
///     The access level granted to a <c>GITHUB_TOKEN</c> permission scope.
/// </summary>
[PublicAPI]
public enum PermissionsLevel
{
    /// <summary>No access (<c>none</c>).</summary>
    None,

    /// <summary>Read-only access (<c>read</c>).</summary>
    Read,

    /// <summary>Read and write access (<c>write</c>).</summary>
    Write,
}
