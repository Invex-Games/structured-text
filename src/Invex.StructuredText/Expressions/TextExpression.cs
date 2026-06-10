namespace Invex.StructuredText.Expressions;

/// <summary>
///     Marker interface that documents the result type an expression is expected to resolve to.
///     The type parameter is a compile-time hint only; it has no effect on formatting.
/// </summary>
/// <typeparam name="TResult">The type the expression resolves to (e.g. <see cref="string" /> or <see cref="bool" />).</typeparam>
[PublicAPI]
public interface ITextExpression<TResult>;

/// <summary>
///     The base type of the platform-agnostic expression tree.
///     Expressions are immutable records composed via fluent methods and operators, then rendered to
///     platform-specific text by a <see cref="TextExpressionFormatter" />.
/// </summary>
/// <remarks>
///     Construction is ergonomic thanks to implicit conversions from <see cref="string" />, <see cref="bool" />,
///     and numeric types, plus fluent builders for functions (<see cref="Contains(string)" />, …) and
///     logic operators (<see cref="EqualTo(TextExpression)" />, <c>&amp;</c>, <c>|</c>, <c>!</c>, …).
/// </remarks>
[PublicAPI]
public abstract partial record TextExpression
{
    /// <summary>
    ///     Creates an index access on this expression, e.g. <c>source[0]</c>.
    /// </summary>
    /// <param name="index">The zero-based index to access.</param>
    public IndexAccessExpression this[int index] => new(this, new NumberExpression(index));

    /// <summary>
    ///     Creates a property access on this expression, e.g. <c>source.property</c>.
    /// </summary>
    /// <param name="property">The property name to access.</param>
    public PropertyAccessExpression this[string property] => new(this, new RawExpression(property));

    /// <summary>
    ///     Marks this expression for evaluation in the target platform's expression context.
    /// </summary>
    public EvaluateExpression Evaluate() =>
        new(this);

    /// <summary>
    ///     Reinterprets this expression as having a different result type.
    ///     The underlying expression is unchanged; only the compile-time type marker is affected.
    /// </summary>
    public CastExpression<TTo> Cast<TTo>() =>
        new(this);
}

/// <summary>
///     Wraps an expression to reinterpret its result type at compile time.
///     The formatter should treat this transparently — it simply delegates to the inner expression.
/// </summary>
[PublicAPI]
public sealed record CastExpression<TTo>(TextExpression Inner) : TextExpression, ITextExpression<TTo>;

/// <summary>
///     A typed workflow expression that documents the expected result type of the expression.
///     The type parameter serves as a compile-time hint for what the expression resolves to.
/// </summary>
[PublicAPI]
public readonly record struct WorkflowExpression<T>(TextExpression Value)
{
    /// <summary>
    ///     Wraps an untyped expression in a typed <see cref="WorkflowExpression{T}" />.
    /// </summary>
    public static implicit operator WorkflowExpression<T>(TextExpression expression) =>
        new(expression);

    /// <summary>
    ///     Unwraps the underlying <see cref="TextExpression" />.
    /// </summary>
    public static implicit operator TextExpression(WorkflowExpression<T> typed) =>
        typed.Value;

    /// <summary>
    ///     Wraps an untyped expression in a nullable typed <see cref="WorkflowExpression{T}" />.
    /// </summary>
    [return: NotNullIfNotNull(nameof(expression))]
    public static implicit operator WorkflowExpression<T>?(TextExpression? expression) =>
        expression is not null
            ? new(expression)
            : null;

    /// <summary>
    ///     Unwraps the underlying <see cref="TextExpression" /> from a nullable typed wrapper.
    /// </summary>
    [return: NotNullIfNotNull(nameof(typed))]
    public static implicit operator TextExpression?(WorkflowExpression<T>? typed) =>
        typed?.Value;
}

/// <summary>
///     Verbatim, uninterpreted text. The formatter writes <see cref="Value" /> exactly as-is.
///     Use this for plain YAML values and context path references;
///     use <see cref="StringExpression" /> for quoted string literals inside expressions.
/// </summary>
/// <param name="Value">The text to write verbatim.</param>
[PublicAPI]
public sealed record RawExpression(string Value) : TextExpression, ITextExpression<string>;

/// <summary>
///     Marks <paramref name="Expression" /> for evaluation in the target platform's expression context.
/// </summary>
/// <param name="Expression">The expression to evaluate.</param>
[PublicAPI]
public sealed record EvaluateExpression(TextExpression Expression) : TextExpression, ITextExpression<string>;

/// <summary>
///     Accesses an element of <paramref name="Array" /> by <paramref name="Index" />, rendered as <c>array[index]</c>.
/// </summary>
/// <param name="Array">The expression being indexed.</param>
/// <param name="Index">The index expression.</param>
[PublicAPI]
public sealed record IndexAccessExpression(TextExpression Array, TextExpression Index)
    : TextExpression, ITextExpression<string>;

/// <summary>
///     Accesses <paramref name="Property" /> of <paramref name="Object" />, rendered as <c>object.property</c>.
/// </summary>
/// <param name="Object">The expression whose property is accessed.</param>
/// <param name="Property">The property name expression.</param>
[PublicAPI]
public sealed record PropertyAccessExpression(TextExpression Object, TextExpression Property)
    : TextExpression, ITextExpression<string>;

/// <summary>
///     Concatenates the rendered text of <paramref name="Values" /> with no separator.
///     Handled directly by <see cref="TextExpressionFormatter" />, so it works on every platform.
/// </summary>
/// <param name="Values">The expressions to concatenate, in order.</param>
[PublicAPI]
public sealed record ConcatExpression(IEnumerable<TextExpression> Values) : TextExpression, ITextExpression<string>;

/// <summary>
///     Extension members that augment <see cref="TextExpressions" /> and <see cref="TextExpression" />
///     with factory helpers for building expression trees.
/// </summary>
[PublicAPI]
public static partial class WorkflowExpressionExtensions
{
    private static IEnumerable<TextExpression> Join(TextExpression separator, IEnumerable<TextExpression> expressions)
    {
        var list = expressions.ToList();

        yield return list[0];

        foreach (var expression in list.Skip(1))
        {
            yield return separator;
            yield return expression;
        }
    }

    extension(TextExpressions)
    {
        /// <summary>
        ///     Concatenates <paramref name="expressions" /> with no separator.
        /// </summary>
        public static ConcatExpression Concat(IEnumerable<TextExpression> expressions) =>
            new(expressions);

        /// <summary>
        ///     Concatenates <paramref name="expressions" /> with <paramref name="separator" /> between each element.
        /// </summary>
        public static ConcatExpression ConcatWithSeparator(
            TextExpression separator,
            IEnumerable<TextExpression> expressions) =>
            new(Join(separator, expressions));
    }
}
