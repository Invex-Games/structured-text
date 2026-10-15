namespace Invex.StructuredText.AzureDevopsPipelines;

/// <summary>
///     Renders typed expressions (<see cref="Expr" />) and <see cref="Template" />s to Azure DevOps syntax.
///     Operators are functions (<c>and(...)</c>, <c>or(...)</c>, <c>not(...)</c>, <c>eq(...)</c>, …) and status
///     checks are <c>succeeded()</c>, <c>failed()</c>, <c>canceled()</c> and <c>always()</c>.
/// </summary>
/// <remarks>
///     <para>
///         Inside a <see cref="Template" />, secrets, variables, matrix values and step outputs are written as
///         macros (<c>$(name)</c>). Expressions that depend on other jobs (job outputs, job results) or contain a
///         <see cref="DevopsRuntimeExpr" /> are runtime expressions (<c>$[ … ]</c>), and everything else is a
///         template expression (<c>${{ … }}</c>).
///     </para>
///     <para>
///         Azure DevOps only evaluates a runtime expression that is the whole value, so a template that combines
///         one with text or other expressions throws <see cref="NotSupportedException" />. Assign the runtime value
///         to a variable and reference it with <see cref="Tex.Variable" /> instead.
///     </para>
///     <para>
///         Inside an expression, secrets, variables and matrix values are <c>variables['name']</c>, inputs are
///         <c>parameters.name</c>, job outputs are <c>dependencies.JOB.outputs['STEP.NAME']</c> and job results are
///         <c>dependencies.JOB.result</c>.
///     </para>
/// </remarks>
[PublicAPI]
public sealed class DevopsExpressionPrinter : ExpressionPrinter
{
    /// <summary>
    ///     A shared instance. The printer is stateless.
    /// </summary>
    public static DevopsExpressionPrinter Instance { get; } = new();

    /// <inheritdoc />
    protected override string WrapEmbedded(string printedExpression) =>
        $"${{{{ {printedExpression} }}}}";

    /// <inheritdoc />
    public override string Render(Template template)
    {
        if (template.Parts.Count > 1 && template.Expressions.FirstOrDefault(IsRuntime) is { } runtime)
            throw new NotSupportedException(
                $"Azure DevOps only evaluates a runtime expression ($[ … ]) that is the whole value, but '{runtime}' " +
                "is combined with other text. Assign it to a variable and reference that with Tex.Variable instead.");

        return base.Render(template);
    }

    /// <inheritdoc />
    protected override string RenderEmbedded(Expr expression) =>
        expression switch
        {
            SecretRef x => $"$({x.Name})",
            VariableRef x => $"$({x.Name})",
            MatrixRef x => $"$({x.Name})",
            StepOutputRef x => $"$({x.Step}.{x.Output})",
            _ when IsRuntime(expression) => $"$[ {Print(expression)} ]",
            _ => base.RenderEmbedded(expression),
        };

    // Job outputs and results only exist at run time, so anything that reads them can't be a template expression.
    private static bool IsRuntime(Expr expression) =>
        expression.DescendantsAndSelf().Any(x => x is JobOutputRef or JobResultRef or DevopsRuntimeExpr);

    /// <inheritdoc />
    protected override string PrintReference(ReferenceExpr reference) =>
        reference switch
        {
            SecretRef x => $"variables[{QuoteString(x.Name)}]",
            VariableRef x => $"variables[{QuoteString(x.Name)}]",
            MatrixRef x => $"variables[{QuoteString(x.Name)}]",
            InputRef x => $"parameters{PrintMember(x.Name)}",
            JobOutputRef x => $"dependencies{PrintMember(x.Job)}.outputs[{QuoteString($"{x.Step ?? x.Job}.{x.Output}")}]",
            StepOutputRef x => $"variables[{QuoteString($"{x.Step}.{x.Output}")}]",
            JobResultRef x => $"dependencies{PrintMember(x.Job)}.result",
            _ => throw new NotSupportedException($"{nameof(DevopsExpressionPrinter)} can't render {reference.GetType().Name}."),
        };

    /// <inheritdoc />
    protected override string PrintResult(RunResult result) =>
        result switch
        {
            RunResult.Success => "'Succeeded'",
            RunResult.Failure => "'Failed'",
            RunResult.Cancelled => "'Canceled'",
            RunResult.Skipped => "'Skipped'",
            _ => throw new ArgumentOutOfRangeException(nameof(result)),
        };

    /// <inheritdoc />
    protected override string FunctionName(string name) =>
        name switch
        {
            FunctionNames.Success => "succeeded",
            FunctionNames.Failure => "failed",
            FunctionNames.Cancelled => "canceled",
            FunctionNames.ToJson => "convertToJson",
            FunctionNames.FromJson => throw new NotSupportedException("Azure DevOps expressions have no fromJSON function."),
            _ => name,
        };

    /// <inheritdoc />
    /// <remarks>
    ///     Azure DevOps's <c>join</c> takes the separator first and requires it, so the arguments are swapped and a
    ///     missing separator becomes <c>','</c>.
    /// </remarks>
    protected override string PrintCall(string name, IReadOnlyList<Expr> arguments) =>
        name is FunctionNames.Join
            ? base.PrintCall(name, [arguments.Count > 1 ? arguments[1] : new StringLiteral(","), arguments[0]])
            : base.PrintCall(name, arguments);

    /// <inheritdoc />
    protected override (string Text, int Precedence) PrintComparison(ComparisonExpr comparison)
    {
        var name = comparison.Operator switch
        {
            ComparisonOperator.Equal => "eq",
            ComparisonOperator.NotEqual => "ne",
            ComparisonOperator.LessThan => "lt",
            ComparisonOperator.LessThanOrEqual => "le",
            ComparisonOperator.GreaterThan => "gt",
            ComparisonOperator.GreaterThanOrEqual => "ge",
            _ => throw new ArgumentOutOfRangeException(nameof(comparison)),
        };

        return ($"{name}({Print(comparison.Left)}, {Print(comparison.Right)})", AtomPrecedence);
    }

    /// <inheritdoc />
    protected override (string Text, int Precedence) PrintLogical(LogicalExpr logical) =>
        ($"{(logical.Operator is LogicalOperator.And ? "and" : "or")}({string.Join(", ", logical.Operands.Select(x => Print(x)))})",
            AtomPrecedence);

    /// <inheritdoc />
    protected override (string Text, int Precedence) PrintNot(NotExpr not) =>
        ($"not({Print(not.Operand)})", AtomPrecedence);

    /// <inheritdoc />
    protected override (string Text, int Precedence) PrintOther(Expr expression) =>
        expression is DevopsRuntimeExpr x
            ? (Print(x.Expression), AtomPrecedence)
            : base.PrintOther(expression);
}
