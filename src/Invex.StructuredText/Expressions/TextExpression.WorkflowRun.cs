namespace Invex.StructuredText.Expressions;

/// <summary>
///     References an output of another job or stage.
///     The platform formatter renders the appropriate context path for the target platform.
/// </summary>
[PublicAPI]
public sealed record TargetOutputExpression : TextExpression, ITextExpression<string>
{
    /// <summary>
    ///     The name (id) of the job or stage that produces the output.
    /// </summary>
    public required string TargetName { get; init; }

    /// <summary>
    ///     The name of the output to read, or <c>null</c> to reference the whole outputs object.
    /// </summary>
    public required string? OutputName { get; init; }
}

/// <summary>
///     References the outcome (result/status) of another job or stage.
///     The platform formatter renders the appropriate context path for the target platform.
///     Compare against a <see cref="TargetOutcomeTypeExpression" />.
/// </summary>
[PublicAPI]
public sealed record TargetOutcomeExpression : TextExpression, ITextExpression<string>
{
    /// <summary>
    ///     The name (id) of the job or stage whose outcome is read.
    /// </summary>
    public required string Target { get; init; }
}

/// <summary>
///     References an output of a step in the current job.
///     The platform formatter renders the appropriate context path for the target platform.
/// </summary>
[PublicAPI]
public sealed record StepOutputExpression : TextExpression, ITextExpression<string>
{
    /// <summary>
    ///     The id/name of the step that produces the output.
    /// </summary>
    public required string StepName { get; init; }

    /// <summary>
    ///     The name of the output to read.
    /// </summary>
    public required string OutputName { get; init; }
}

/// <summary>
///     References the outcome of a step in the current job,
///     rendered as <c>steps.&lt;step&gt;.outcome</c>.
///     Compare against a <see cref="StepOutcomeTypeExpression" />.
/// </summary>
[PublicAPI]
public sealed record StepOutcomeExpression : TextExpression, ITextExpression<string>
{
    /// <summary>
    ///     The id/name of the step whose outcome is read.
    /// </summary>
    public required string StepName { get; init; }
}

/// <summary>
///     A job/stage outcome literal for comparison with <see cref="TargetOutcomeExpression" />.
///     The platform formatter renders the appropriate outcome string for the target platform.
/// </summary>
[PublicAPI]
public sealed record TargetOutcomeTypeExpression : TextExpression, ITextExpression<string>
{
    /// <summary>
    ///     The possible outcomes of a job or stage.
    /// </summary>
    public enum OutcomeType
    {
        /// <summary>The job or stage completed successfully.</summary>
        Success,

        /// <summary>The job or stage failed.</summary>
        Failure,

        /// <summary>The job or stage was cancelled.</summary>
        Cancelled,
    }

    /// <summary>
    ///     The outcome value this literal represents.
    /// </summary>
    public required OutcomeType Type { get; init; }
}

/// <summary>
///     A step outcome literal for comparison with <see cref="StepOutcomeExpression" />.
///     The platform formatter renders the appropriate outcome string for the target platform.
/// </summary>
[PublicAPI]
public sealed record StepOutcomeTypeExpression : TextExpression, ITextExpression<string>
{
    /// <summary>
    ///     The possible outcomes of a step.
    /// </summary>
    public enum OutcomeType
    {
        /// <summary>The step completed successfully.</summary>
        Success,

        /// <summary>The step failed.</summary>
        Failure,

        /// <summary>The step was cancelled.</summary>
        Cancelled,

        /// <summary>The step was skipped.</summary>
        Skipped,
    }

    /// <summary>
    ///     The outcome value this literal represents.
    /// </summary>
    public required OutcomeType Type { get; init; }
}
