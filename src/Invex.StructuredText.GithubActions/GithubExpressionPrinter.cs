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

    /// <inheritdoc />
    protected override string WrapEmbedded(string printedExpression) =>
        $"${{{{ {printedExpression} }}}}";

    /// <inheritdoc />
    protected override string PrintReference(ReferenceExpr reference) =>
        reference switch
        {
            SecretRef x => $"secrets{Member(x.Name)}",
            VariableRef x => $"vars{Member(x.Name)}",
            InputRef x => $"inputs{Member(x.Name)}",
            MatrixRef x => $"matrix{Member(x.Name)}",
            JobOutputRef x => $"needs{Member(x.Job)}.outputs{Member(x.Output)}",
            StepOutputRef x => $"steps{Member(x.Step)}.outputs{Member(x.Output)}",
            JobResultRef x => $"needs{Member(x.Job)}.result",
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

    // Property dereference (`.name`) is allowed for names made of letters, digits, '_' and '-' that don't start with
    // a digit or '-'; anything else uses index syntax (`['name']`).
    private static string Member(string name) =>
        name.Length > 0 &&
        (char.IsAsciiLetter(name[0]) || name[0] is '_') &&
        name.All(c => char.IsAsciiLetterOrDigit(c) || c is '_' or '-')
            ? $".{name}"
            : $"['{name.Replace("'", "''", StringComparison.Ordinal)}']";
}
