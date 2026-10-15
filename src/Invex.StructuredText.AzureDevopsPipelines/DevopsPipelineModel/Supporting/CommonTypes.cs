namespace Invex.StructuredText.AzureDevopsPipelines.DevopsPipelineModel.Supporting;

/// <summary>
///     Include and exclude filters for branches, paths, or tags.
/// </summary>
[PublicAPI]
public sealed record IncludeExcludeFilters
{
    /// <summary>
    ///     List of items to include.
    /// </summary>
    public IReadOnlyList<ExprTemplate>? Include { get; init; }

    /// <summary>
    ///     List of items to exclude.
    /// </summary>
    public IReadOnlyList<ExprTemplate>? Exclude { get; init; }
}

/// <summary>
///     Workspace options on the agent.
/// </summary>
[PublicAPI]
public sealed record Workspace
{
    /// <summary>
    ///     What to clean up before the job runs.
    ///     Valid values: "outputs" | "resources" | "all"
    /// </summary>
    public ExprTemplate? Clean { get; init; }
}

/// <summary>
///     Extends a template.
/// </summary>
[PublicAPI]
public sealed record Extends
{
    /// <summary>
    ///     The template referenced by the pipeline to extend.
    /// </summary>
    public required ExprTemplate Template { get; init; }

    /// <summary>
    ///     Parameters used in the extend.
    /// </summary>
    public IReadOnlyDictionary<string, ExprTemplate>? Parameters { get; init; }
}

/// <summary>
///     Pipeline template parameter definition.
/// </summary>
[PublicAPI]
public sealed record Parameter
{
    /// <summary>
    ///     Parameter name.
    /// </summary>
    public required ExprTemplate Name { get; init; }

    /// <summary>
    ///     Parameter display name.
    /// </summary>
    public ExprTemplate? DisplayName { get; init; }

    /// <summary>
    ///     Parameter type.
    ///     Valid values: "string" | "number" | "boolean" | "object" | "step" | "stepList" |
    ///     "job" | "jobList" | "deployment" | "deploymentList" | "stage" | "stageList"
    /// </summary>
    public ExprTemplate? Type { get; init; }

    /// <summary>
    ///     Default value for the parameter.
    /// </summary>
    public ExprTemplate? Default { get; init; }

    /// <summary>
    ///     Allowed values for the parameter.
    /// </summary>
    public IReadOnlyList<ExprTemplate>? Values { get; init; }
}

/// <summary>
///     Scheduled trigger (cron).
/// </summary>
[PublicAPI]
public sealed record Schedule
{
    /// <summary>
    ///     Cron expression for the schedule.
    /// </summary>
    public required ExprTemplate Cron { get; init; }

    /// <summary>
    ///     Display name for the schedule.
    /// </summary>
    public ExprTemplate? DisplayName { get; init; }

    /// <summary>
    ///     Branches to include or exclude for the scheduled trigger.
    /// </summary>
    public IncludeExcludeFilters? Branches { get; init; }

    /// <summary>
    ///     Whether to run the schedule if the code hasn't changed.
    /// </summary>
    public ExprTemplate? Always { get; init; }
}
