namespace Invex.StructuredText.GithubActions.GithubActionModel;

/// <summary>
///     The root model of a GitHub Actions workflow file (<c>.github/workflows/*.yml</c>).
///     Serialize it with <see cref="GithubActionWriter" />.
/// </summary>
[PublicAPI]
public sealed record GithubAction
{
    /// <summary>
    ///     The name of the workflow, shown in the GitHub Actions UI.
    /// </summary>
    public string? Name { get; init; }

    /// <summary>
    ///     The name for workflow runs generated from the workflow (<c>run-name</c>).
    ///     Can include expressions, e.g. referencing <c>github.actor</c>.
    /// </summary>
    public TextExpression? RunName { get; init; }

    /// <summary>
    ///     The events that trigger the workflow (<c>on</c>). At least one event is required.
    /// </summary>
    public required IReadOnlyList<On> On { get; init; }

    /// <summary>
    ///     The default <c>GITHUB_TOKEN</c> permissions for all jobs in the workflow.
    ///     Can be overridden per job.
    /// </summary>
    public Permissions? Permissions { get; init; }

    /// <summary>
    ///     Environment variables available to all jobs and steps in the workflow.
    /// </summary>
    public IReadOnlyDictionary<string, TextExpression>? Env { get; init; }

    /// <summary>
    ///     Concurrency settings that limit how many runs of this workflow execute at once.
    /// </summary>
    public Concurrency? Concurrency { get; init; }

    /// <summary>
    ///     The jobs that make up the workflow. At least one job is required.
    /// </summary>
    public required IReadOnlyList<Job> Jobs { get; init; }
}
