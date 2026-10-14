namespace Invex.StructuredText.GithubActions.GithubActionModel;

/// <summary>
///     A Docker container that a job runs in (<c>container</c>) or that hosts a service (<c>services</c>).
/// </summary>
[PublicAPI]
public sealed record Container
{
    /// <summary>
    ///     The Docker image to use, e.g. <c>node:20</c> or a full registry path.
    /// </summary>
    public required Template Image { get; init; }

    /// <summary>
    ///     Credentials for the container registry hosting the image, when it is private.
    /// </summary>
    public Credentials? Credentials { get; init; }

    /// <summary>
    ///     Environment variables to set inside the container.
    /// </summary>
    public IReadOnlyDictionary<string, Template>? Env { get; init; }

    /// <summary>
    ///     Ports to expose on the container.
    /// </summary>
    public IReadOnlyList<Template>? Ports { get; init; }

    /// <summary>
    ///     Volumes for the container to use, as source/destination pairs or named volumes.
    /// </summary>
    public IReadOnlyList<Template>? Volumes { get; init; }

    /// <summary>
    ///     Additional options passed to <c>docker create</c> (<c>options</c>).
    /// </summary>
    public Template? Options { get; init; }
}
