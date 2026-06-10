namespace Invex.StructuredText.Expressions;

/// <summary>
///     Renders a <see cref="TextExpression" /> tree into platform-specific text.
/// </summary>
[PublicAPI]
public interface ITextExpressionFormatter
{
    /// <summary>
    ///     Renders <paramref name="expression" /> to text, or returns <c>null</c> when the expression is <c>null</c>.
    /// </summary>
    /// <param name="expression">The expression tree to render.</param>
    [return: NotNullIfNotNull("expression")]
    string? Format(TextExpression? expression);

    /// <summary>
    ///     Renders the expression wrapped in a typed <see cref="WorkflowExpression{T}" />,
    ///     or returns <c>null</c> when the wrapper has no value.
    /// </summary>
    /// <param name="expression">The typed expression to render.</param>
    string? Format<T>(WorkflowExpression<T>? expression) =>
        expression.HasValue
            ? Format(expression.Value.Value)
            : null;
}

/// <summary>
///     Base class for platform-specific expression formatters.
/// </summary>
/// <remarks>
///     <see cref="Format" /> is implemented as a rewrite loop: <see cref="RawExpression" /> yields its text,
///     <see cref="ConcatExpression" /> concatenates its formatted parts, and <see cref="CastExpression{TTo}" />
///     is unwrapped transparently. Every other node is passed to <see cref="Resolve" />, which must rewrite it
///     into a simpler expression; the loop repeats until everything bottoms out in raw text.
/// </remarks>
[PublicAPI]
public abstract class TextExpressionFormatter : ITextExpressionFormatter
{
    /// <summary>
    ///     Renders <paramref name="expression" /> to platform-specific text,
    ///     or returns <c>null</c> when the expression is <c>null</c>.
    /// </summary>
    /// <param name="expression">The expression tree to render.</param>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when <see cref="Resolve" /> cannot handle an expression node.
    /// </exception>
    [return: NotNullIfNotNull("expression")]
    public string? Format(TextExpression? expression)
    {
        while (true)
        {
            switch (expression)
            {
                case null:
                    return null;

                case RawExpression raw:
                    return raw.Value;

                case ConcatExpression concat:
                    return string.Concat(concat.Values.Select(Format));
            }

            // Unwrap CastExpression transparently — it only affects compile-time typing
            if (expression.GetType() is { IsGenericType: true } type &&
                type.GetGenericTypeDefinition() == typeof(CastExpression<>))
            {
                var inner = (TextExpression)type.GetProperty(nameof(CastExpression<>.Inner))!.GetValue(expression)!;

                expression = inner;

                continue;
            }

            if (Resolve(expression) is { } result)
            {
                expression = result;

                continue;
            }

            throw new InvalidOperationException($"No writer found to handle expression {expression}");
        }
    }

    /// <summary>
    ///     Rewrites <paramref name="expression" /> into a simpler expression — typically a
    ///     <see cref="RawExpression" /> containing the platform-specific rendering of the node.
    ///     Implementations may call <see cref="Format" /> recursively to render sub-expressions.
    /// </summary>
    /// <param name="expression">The node to rewrite. Never a raw, concat, or cast expression.</param>
    /// <returns>The rewritten expression, or <c>null</c> if the node is not supported.</returns>
    protected abstract TextExpression? Resolve(TextExpression expression);
}
