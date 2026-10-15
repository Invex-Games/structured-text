namespace Invex.StructuredText.GithubActions;

/// <summary>
///     Adds the GitHub Actions contexts to <see cref="Tex" />.
/// </summary>
[PublicAPI]
public static class TexGitHubExtensions
{
    extension(Tex)
    {
        /// <summary>
        ///     The GitHub Actions contexts: <c>Tex.GitHub.Ref</c> is <c>github.ref</c>,
        ///     <c>Tex.GitHub.Event.PullRequest.Number</c> is <c>github.event.pull_request.number</c>, and
        ///     <c>Tex.GitHub.Runner.Os</c> is <c>runner.os</c>.
        /// </summary>
        public static GitHubContexts GitHub => GitHubContexts.Instance;
    }
}
