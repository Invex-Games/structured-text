namespace Invex.StructuredText.GithubActions.GithubActionModel;

/// <summary>
///     <c>GITHUB_TOKEN</c> permissions at workflow or job level (<c>permissions</c>).
///     This is a discriminated union: use <see cref="All" /> to grant one level to every scope
///     (<c>read-all</c> / <c>write-all</c>), or <see cref="Exact" /> to specify levels per scope.
/// </summary>
[PublicAPI]
[Union]
public partial record Permissions
{
    /// <summary>
    ///     Grants the same <paramref name="Level" /> to every permission scope,
    ///     rendered as <c>read-all</c> or <c>write-all</c>.
    /// </summary>
    /// <param name="Level">The level to grant across all scopes.</param>
    [PublicAPI]
    public sealed partial record All(PermissionsLevel Level);

    /// <summary>
    ///     Grants explicit levels for individual permission scopes.
    /// </summary>
    /// <param name="Permissions">The per-scope permission levels.</param>
    [PublicAPI]
    public sealed partial record Exact(PermissionsEvent Permissions);
}
