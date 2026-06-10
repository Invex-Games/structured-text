namespace Invex.StructuredText.AzureDevopsPipelines;

/// <summary>
///     Wraps an expression to be evaluated at runtime using Azure DevOps runtime expression syntax: $[ expression ]
/// </summary>
/// <remarks>
///     Runtime expressions are evaluated when the value is read during the run, unlike compile-time
///     template expressions (<see cref="TextExpression.Evaluate" /> → <c>${{ }}</c>) which are expanded
///     when the pipeline is compiled. Typically used for variable values and job/stage conditions, e.g.
///     <c>new DevopsRuntimeExpression(expr)</c> renders as <c>$[ ... ]</c>.
/// </remarks>
/// <param name="Expression">The expression to evaluate at runtime.</param>
[PublicAPI]
[SuppressMessage("Design", "CA1067:Override Object.Equals(object) when implementing IEquatable<T>")]
public sealed record DevopsRuntimeExpression(TextExpression Expression) : TextExpression;
