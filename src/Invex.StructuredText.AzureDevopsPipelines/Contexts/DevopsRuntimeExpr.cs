namespace Invex.StructuredText.AzureDevopsPipelines;

/// <summary>
///     Marks an expression to be evaluated at run time. Embedded in a <see cref="Template" />, it renders as
///     <c>$[ … ]</c> instead of the compile-time <c>${{ … }}</c>. Create one with <c>Tex.DevOps.Runtime(…)</c>.
/// </summary>
/// <param name="Expression">The expression to evaluate at run time.</param>
[PublicAPI]
public sealed record DevopsRuntimeExpr(Expr Expression) : JsonExpr
{
    /// <inheritdoc />
    public override IEnumerable<Expr> Children => [Expression];
}
