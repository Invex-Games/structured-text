namespace Invex.StructuredText.GithubActions.GithubActionModel;

/// <summary>
///     A job within a workflow: a set of steps that execute on the same runner.
/// </summary>
[PublicAPI]
public sealed record Job
{
    /// <summary>
    ///     The id of the job, used as the YAML key under <c>jobs:</c> and referenced by
    ///     <see cref="Needs" /> of other jobs.
    /// </summary>
    public required TextExpression Name { get; init; }

    /// <summary>
    ///     <c>GITHUB_TOKEN</c> permissions for this job, overriding the workflow-level default.
    /// </summary>
    public Permissions? Permissions { get; init; }

    /// <summary>
    ///     Ids of jobs that must complete successfully before this job runs (<c>needs</c>).
    /// </summary>
    public TextExpressionCollection Needs { get; init; } = [];

    /// <summary>
    ///     A condition that must evaluate to true for the job to run (<c>if</c>).
    ///     Written in expression context, so no <c>Evaluate()</c> wrapper is needed.
    /// </summary>
    public TextExpression? If { get; init; }

    /// <summary>
    ///     The runner(s) the job executes on (<c>runs-on</c>).
    /// </summary>
    public required RunsOn RunsOn { get; init; }

    /// <summary>
    ///     Runner image snapshot settings for the job (<c>snapshot</c>).
    /// </summary>
    public Snapshot? Snapshot { get; init; }

    /// <summary>
    ///     The deployment environment the job targets (<c>environment</c>).
    /// </summary>
    public Environment? Environment { get; init; }

    /// <summary>
    ///     Concurrency settings for this job, limiting parallel runs that share the same group.
    /// </summary>
    public Concurrency? Concurrency { get; init; }

    /// <summary>
    ///     Values made available to downstream jobs that depend on this job (<c>outputs</c>).
    ///     Typically populated from step outputs via <see cref="StepOutputExpression" />.
    /// </summary>
    public IReadOnlyDictionary<string, TextExpression>? Outputs { get; init; }

    /// <summary>
    ///     Environment variables available to all steps in the job.
    /// </summary>
    public IReadOnlyDictionary<string, TextExpression>? Env { get; init; }

    /// <summary>
    ///     The maximum number of minutes the job may run before GitHub cancels it (<c>timeout-minutes</c>).
    /// </summary>
    public TextExpression? TimeoutMinutes { get; init; }

    /// <summary>
    ///     The matrix strategy used to run multiple variations of the job (<c>strategy</c>).
    /// </summary>
    public Strategy? Strategy { get; init; }

    /// <summary>
    ///     Whether the workflow run passes when this job fails (<c>continue-on-error</c>).
    /// </summary>
    public TextExpression? ContinueOnError { get; init; }

    /// <summary>
    ///     The container the job's steps run in (<c>container</c>).
    /// </summary>
    public Container? Container { get; init; }

    /// <summary>
    ///     Service containers to host alongside the job (<c>services</c>), keyed by service name.
    /// </summary>
    public IReadOnlyDictionary<string, Container>? Services { get; init; }

    /// <summary>
    ///     The ordered steps that make up the job. At least one step is required.
    /// </summary>
    public required IReadOnlyList<Step> Steps { get; init; }
}
