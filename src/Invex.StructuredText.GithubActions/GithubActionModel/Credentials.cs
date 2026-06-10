namespace Invex.StructuredText.GithubActions.GithubActionModel;

/// <summary>
///     Credentials for a private container registry (<c>credentials</c>), passed to <c>docker login</c>.
///     Typically reference secrets, e.g. <c>${{ secrets.REGISTRY_PASSWORD }}</c>.
/// </summary>
[PublicAPI]
public sealed record Credentials
{
    /// <summary>
    ///     The registry username.
    /// </summary>
    public TextExpression? Username { get; init; }

    /// <summary>
    ///     The registry password or token.
    /// </summary>
    public TextExpression? Password { get; init; }
}
