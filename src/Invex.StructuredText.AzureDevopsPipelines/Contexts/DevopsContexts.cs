namespace Invex.StructuredText.AzureDevopsPipelines;

/// <summary>
///     Azure DevOps predefined variables and run-time expressions, reached through <c>Tex.DevOps</c>.
///     Each variable is a <see cref="Tex.Variable" /> reference: <c>$(Build.BuildId)</c> in a template and
///     <c>variables['Build.BuildId']</c> in a condition.
/// </summary>
[PublicAPI]
public sealed class DevopsContexts
{
    private DevopsContexts() { }

    internal static DevopsContexts Instance { get; } = new();

    /// <summary><c>Build.*</c> variables.</summary>
    public DevopsBuildVariables Build { get; } = new();

    /// <summary><c>System.*</c> variables.</summary>
    public DevopsSystemVariables System { get; } = new();

    /// <summary><c>Agent.*</c> variables.</summary>
    public DevopsAgentVariables Agent { get; } = new();

    /// <summary><c>Pipeline.Workspace</c>: the local path on the agent where the pipeline's files live.</summary>
    public StringExpr PipelineWorkspace { get; } = Tex.Variable("Pipeline.Workspace");

    /// <summary>
    ///     Evaluates <paramref name="expression" /> at run time: <c>$[ … ]</c> when embedded in a template.
    /// </summary>
    /// <param name="expression">The expression.</param>
    public DevopsRuntimeExpr Runtime(Expr expression) =>
        new(expression);
}

/// <summary><c>Build.*</c> predefined variables.</summary>
[PublicAPI]
public sealed class DevopsBuildVariables
{
    internal DevopsBuildVariables() { }

    /// <summary><c>Build.BuildId</c>: the ID of the run.</summary>
    public StringExpr BuildId { get; } = Tex.Variable("Build.BuildId");

    /// <summary><c>Build.BuildNumber</c>: the name of the run.</summary>
    public StringExpr BuildNumber { get; } = Tex.Variable("Build.BuildNumber");

    /// <summary><c>Build.Reason</c>: what started the run, e.g. <c>IndividualCI</c> or <c>PullRequest</c>.</summary>
    public StringExpr Reason { get; } = Tex.Variable("Build.Reason");

    /// <summary><c>Build.SourceBranch</c>: the full branch or tag ref, e.g. <c>refs/heads/main</c>.</summary>
    public StringExpr SourceBranch { get; } = Tex.Variable("Build.SourceBranch");

    /// <summary><c>Build.SourceBranchName</c>: the last segment of the branch or tag ref.</summary>
    public StringExpr SourceBranchName { get; } = Tex.Variable("Build.SourceBranchName");

    /// <summary><c>Build.SourceVersion</c>: the commit being built.</summary>
    public StringExpr SourceVersion { get; } = Tex.Variable("Build.SourceVersion");

    /// <summary><c>Build.Repository.Name</c>: the repository name.</summary>
    public StringExpr RepositoryName { get; } = Tex.Variable("Build.Repository.Name");

    /// <summary><c>Build.ArtifactStagingDirectory</c>: a local folder for artifacts before publishing.</summary>
    public StringExpr ArtifactStagingDirectory { get; } = Tex.Variable("Build.ArtifactStagingDirectory");

    /// <summary><c>Build.SourcesDirectory</c>: where the source code is checked out.</summary>
    public StringExpr SourcesDirectory { get; } = Tex.Variable("Build.SourcesDirectory");
}

/// <summary><c>System.*</c> predefined variables.</summary>
[PublicAPI]
public sealed class DevopsSystemVariables
{
    internal DevopsSystemVariables() { }

    /// <summary><c>System.AccessToken</c>: the token the run uses to call Azure DevOps.</summary>
    public StringExpr AccessToken { get; } = Tex.Variable("System.AccessToken");

    /// <summary><c>System.DefaultWorkingDirectory</c>: the default working directory for steps.</summary>
    public StringExpr DefaultWorkingDirectory { get; } = Tex.Variable("System.DefaultWorkingDirectory");

    /// <summary><c>System.PullRequest.PullRequestId</c>: the pull request's ID (pull request runs only).</summary>
    public StringExpr PullRequestId { get; } = Tex.Variable("System.PullRequest.PullRequestId");

    /// <summary><c>System.PullRequest.TargetBranch</c>: the pull request's target branch.</summary>
    public StringExpr PullRequestTargetBranch { get; } = Tex.Variable("System.PullRequest.TargetBranch");

    /// <summary><c>System.TeamProject</c>: the project name.</summary>
    public StringExpr TeamProject { get; } = Tex.Variable("System.TeamProject");
}

/// <summary><c>Agent.*</c> predefined variables.</summary>
[PublicAPI]
public sealed class DevopsAgentVariables
{
    internal DevopsAgentVariables() { }

    /// <summary><c>Agent.OS</c>: <c>Windows_NT</c>, <c>Darwin</c> or <c>Linux</c>.</summary>
    public StringExpr Os { get; } = Tex.Variable("Agent.OS");

    /// <summary><c>Agent.TempDirectory</c>: a temporary folder cleaned after each job.</summary>
    public StringExpr TempDirectory { get; } = Tex.Variable("Agent.TempDirectory");
}

/// <summary>
///     Adds the Azure DevOps predefined variables and run-time expressions to <see cref="Tex" />.
/// </summary>
[PublicAPI]
public static class TexDevopsExtensions
{
    extension(Tex)
    {
        /// <summary>
        ///     Azure DevOps predefined variables (<c>Tex.DevOps.Build.SourceBranch</c>) and run-time expressions
        ///     (<c>Tex.DevOps.Runtime(…)</c>).
        /// </summary>
        public static DevopsContexts DevOps => DevopsContexts.Instance;
    }
}
