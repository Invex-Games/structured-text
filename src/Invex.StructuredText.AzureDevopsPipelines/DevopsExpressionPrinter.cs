namespace Invex.StructuredText.AzureDevopsPipelines;

/// <summary>
///     Renders typed expressions (<see cref="Expr" />) and <see cref="Template" />s to Azure DevOps syntax.
///     Operators are functions (<c>and(...)</c>, <c>or(...)</c>, <c>not(...)</c>, <c>eq(...)</c>, …) and status
///     checks are <c>succeeded()</c>, <c>failed()</c>, <c>canceled()</c> and <c>always()</c>.
/// </summary>
/// <remarks>
///     <para>
///         Inside a <see cref="Template" />, secrets, variables, matrix values and step outputs are written as
///         macros (<c>$(name)</c>), job outputs and <see cref="DevopsRuntimeExpr" /> as runtime expressions
///         (<c>$[ … ]</c>), and everything else as template expressions (<c>${{ … }}</c>).
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
    protected override string RenderEmbedded(Expr expression) =>
        expression switch
        {
            SecretRef x => $"$({x.Name})",
            VariableRef x => $"$({x.Name})",
            MatrixRef x => $"$({x.Name})",
            StepOutputRef x => $"$({x.Step}.{x.Output})",
            JobOutputRef x => $"$[ {Print(x)} ]",
            DevopsRuntimeExpr x => $"$[ {Print(x.Expression)} ]",
            _ => base.RenderEmbedded(expression),
        };

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
            ? (Print(x.Expression, AtomPrecedence), AtomPrecedence)
            : base.PrintOther(expression);
}
