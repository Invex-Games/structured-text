namespace Invex.StructuredText.GithubActions;

/// <summary>
///     Renders typed expressions (<see cref="Expr" />) and <see cref="Template" />s to GitHub Actions expression
///     syntax: infix operators (<c>==</c>, <c>&amp;&amp;</c>, <c>!</c>), single-quoted string literals, and
///     <c>${{ … }}</c> around expressions embedded in text.
/// </summary>
/// <remarks>
///     Portable references render as context paths: secrets as <c>secrets.NAME</c>, variables as <c>vars.NAME</c>,
///     inputs as <c>inputs.NAME</c>, matrix values as <c>matrix.NAME</c>, job outputs as
///     <c>needs.JOB.outputs.NAME</c>, step outputs as <c>steps.STEP.outputs.NAME</c> and job results as
///     <c>needs.JOB.result</c>. Result literals render as <c>'success'</c>, <c>'failure'</c>, <c>'cancelled'</c> and
///     <c>'skipped'</c>.
/// </remarks>
[PublicAPI]
public sealed class GithubExpressionPrinter : ExpressionPrinter
{
    /// <summary>
    ///     A shared instance. The printer is stateless.
    /// </summary>
    public static GithubExpressionPrinter Instance { get; } = new();

    /// <summary>
    ///     Renders an <c>if:</c> condition. The bare expression is used where possible; a condition whose first
    ///     non-whitespace character means something to YAML (<c>!</c> starts a tag, a quote starts a quoted scalar,
    ///     and so on) is wrapped in <c>${{ }}</c>.
    /// </summary>
    /// <param name="condition">The condition.</param>
    /// <returns>The rendered condition, or <see langword="null" /> when <paramref name="condition" /> is
    ///     <see langword="null" />.</returns>
    [return: NotNullIfNotNull(nameof(condition))]
    public string? PrintCondition(BoolExpr? condition) =>
        PrintOrNull(condition) is { } printed && StartsWithYamlIndicator(printed)
            ? WrapEmbedded(printed)
            : PrintOrNull(condition);

    /// <summary>
    ///     <see langword="true" /> when <paramref name="text" />, after leading whitespace, starts with a character
    ///     that YAML would not read as the start of a plain scalar.
    /// </summary>
    /// <param name="text">The text to test.</param>
    /// <returns>Whether the text needs quoting or wrapping before it can be written as a YAML value.</returns>
    public static bool StartsWithYamlIndicator(string text)
    {
        var trimmed = text.TrimStart();

        return trimmed.Length > 0 &&
               ("!&*'\"[]{}|>%@`#,".Contains(trimmed[0]) ||
                (trimmed[0] is '-' or '?' or ':' && (trimmed.Length == 1 || char.IsWhiteSpace(trimmed[1]))));
    }

    /// <inheritdoc />
    protected override string WrapEmbedded(string printedExpression) =>
        $"${{{{ {printedExpression} }}}}";

    /// <inheritdoc />
    protected override string PrintReference(ReferenceExpr reference) =>
        reference switch
        {
            SecretRef x => $"secrets{PrintMember(x.Name)}",
            VariableRef x => $"vars{PrintMember(x.Name)}",
            InputRef x => $"inputs{PrintMember(x.Name)}",
            MatrixRef x => $"matrix{PrintMember(x.Name)}",
            JobOutputRef x => $"needs{PrintMember(x.Job)}.outputs{PrintMember(x.Output)}",
            StepOutputRef x => $"steps{PrintMember(x.Step)}.outputs{PrintMember(x.Output)}",
            JobResultRef x => $"needs{PrintMember(x.Job)}.result",
            _ => throw new NotSupportedException($"{nameof(GithubExpressionPrinter)} can't render {reference.GetType().Name}."),
        };

    /// <inheritdoc />
    protected override string PrintResult(RunResult result) =>
        result switch
        {
            RunResult.Success => "'success'",
            RunResult.Failure => "'failure'",
            RunResult.Cancelled => "'cancelled'",
            RunResult.Skipped => "'skipped'",
            _ => throw new ArgumentOutOfRangeException(nameof(result)),
        };
}
