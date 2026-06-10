namespace Invex.StructuredText.Expressions;

partial record TextExpression
{
    /// <summary>
    ///     Converts a <see cref="bool" /> to a <see cref="BooleanExpression" />.
    /// </summary>
    public static implicit operator TextExpression(bool value) =>
        new BooleanExpression(value);

    /// <summary>
    ///     Converts a <see cref="double" /> to a <see cref="NumberExpression" />.
    /// </summary>
    public static implicit operator TextExpression(double value) =>
        new NumberExpression(value);

    /// <summary>
    ///     Converts a <see cref="float" /> to a <see cref="NumberExpression" />.
    /// </summary>
    public static implicit operator TextExpression(float value) =>
        new NumberExpression(value);

    /// <summary>
    ///     Converts a <see cref="long" /> to a <see cref="NumberExpression" />.
    /// </summary>
    public static implicit operator TextExpression(long value) =>
        new NumberExpression(value);

    /// <summary>
    ///     Converts an <see cref="int" /> to a <see cref="NumberExpression" />.
    /// </summary>
    public static implicit operator TextExpression(int value) =>
        new NumberExpression(value);

    /// <summary>
    ///     Converts a <see cref="short" /> to a <see cref="NumberExpression" />.
    /// </summary>
    public static implicit operator TextExpression(short value) =>
        new NumberExpression(value);

    /// <summary>
    ///     Converts a <see cref="byte" /> to a <see cref="NumberExpression" />.
    /// </summary>
    public static implicit operator TextExpression(byte value) =>
        new NumberExpression(value);

    /// <summary>
    ///     Converts a <see cref="string" /> to a <see cref="RawExpression" /> (verbatim text, not a quoted literal).
    /// </summary>
    public static implicit operator TextExpression(string value) =>
        new RawExpression(value);
}

public static partial class WorkflowExpressionExtensions
{
    extension(TextExpressions)
    {
        /// <summary>
        ///     The boolean literal <c>true</c>.
        /// </summary>
        [PublicAPI]
        public static BooleanExpression True => new(true);

        /// <summary>
        ///     The boolean literal <c>false</c>.
        /// </summary>
        [PublicAPI]
        public static BooleanExpression False => new(false);

        /// <summary>
        ///     The null literal, rendered as empty text.
        /// </summary>
        [PublicAPI]
        public static NullExpression Null => new();

        /// <summary>
        ///     Creates a <see cref="RawExpression" /> — verbatim, uninterpreted text.
        /// </summary>
        [PublicAPI]
        public static RawExpression Raw(string value) =>
            new(value);

        /// <summary>
        ///     Creates a <see cref="StringExpression" /> — a quoted string literal inside an expression.
        /// </summary>
        [PublicAPI]
        public static StringExpression From(string value) =>
            new(value);

        /// <summary>
        ///     Creates a <see cref="BooleanExpression" /> literal.
        /// </summary>
        [PublicAPI]
        public static BooleanExpression From(bool value) =>
            new(value);

        /// <summary>
        ///     Creates a <see cref="NumberExpression" /> literal from any numeric value.
        /// </summary>
        [PublicAPI]
        public static NumberExpression From<T>(T value)
            where T : INumber<T> =>
            new(double.TryParse(value.ToString(), out var result)
                ? result
                : 0);

        /// <summary>
        ///     Builds a <see cref="FormatExpression" /> from an interpolated string whose holes may be
        ///     other <see cref="TextExpression" /> instances. Literal parts become the format string;
        ///     holes become arguments passed to the platform's <c>format()</c> function.
        /// </summary>
        [PublicAPI]
        [OverloadResolutionPriority(1)]
        public static FormatExpression Format(TextExpressionInterpolatedStringHandler handler) =>
            handler.ToFormatExpression();

        /// <summary>
        ///     Builds a <see cref="FormatExpression" /> from plain text with no arguments.
        ///     Braces in <paramref name="text" /> are escaped.
        /// </summary>
        [PublicAPI]
        public static FormatExpression Format(string text)
        {
            var handler = new TextExpressionInterpolatedStringHandler(text.Length, 0);
            handler.AppendLiteral(text);

            return handler.ToFormatExpression();
        }
    }
}

/// <summary>
///     A boolean literal, rendered as <c>true</c> or <c>false</c>.
/// </summary>
/// <param name="Value">The literal value.</param>
[PublicAPI]
public sealed record BooleanExpression(bool Value) : TextExpression, ITextExpression<bool>;

/// <summary>
///     The null literal, rendered as empty text.
/// </summary>
[PublicAPI]
public sealed record NullExpression : TextExpression;

/// <summary>
///     A numeric literal, rendered using the invariant culture.
/// </summary>
/// <param name="Value">The literal value.</param>
[PublicAPI]
public sealed record NumberExpression(double Value) : TextExpression, ITextExpression<double>;

/// <summary>
///     A string literal inside an expression, rendered with quotes (e.g. <c>'value'</c>).
///     Use <see cref="RawExpression" /> for verbatim, unquoted text.
/// </summary>
/// <param name="Value">The literal value, without quotes.</param>
[PublicAPI]
public sealed record StringExpression(string Value) : TextExpression, ITextExpression<string>;
