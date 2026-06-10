namespace Invex.StructuredText.Expressions;

/// <summary>
///     An interpolated string handler that creates a <see cref="FormatExpression" /> from an interpolated string.
///     Literal parts are combined into a format string with placeholders, and <see cref="TextExpression" /> holes
///     become arguments.
/// </summary>
[InterpolatedStringHandler]
[PublicAPI]
public ref struct TextExpressionInterpolatedStringHandler(int literalLength, int formattedCount)
{
    private readonly StringBuilder _formatBuilder = new(literalLength + formattedCount * 3);
    private readonly List<TextExpression> _arguments = new(formattedCount);
    private int _argumentIndex = 0;

    /// <summary>
    ///     Appends a literal part of the interpolated string to the format string, escaping braces.
    /// </summary>
    /// <param name="value">The literal text.</param>
    public void AppendLiteral(string value) =>
        _formatBuilder.Append(value
            .Replace("{", "{{")
            .Replace("}", "}}"));

    /// <summary>
    ///     Appends an expression hole: a placeholder (<c>{n}</c>) is added to the format string and
    ///     <paramref name="expression" /> becomes the corresponding argument.
    /// </summary>
    /// <param name="expression">The expression to use as a format argument.</param>
    public void AppendFormatted(TextExpression expression)
    {
        _formatBuilder.Append('{');
        _formatBuilder.Append(_argumentIndex++);
        _formatBuilder.Append('}');
        _arguments.Add(expression);
    }

    /// <summary>
    ///     Appends a string hole as a <see cref="StringExpression" /> argument.
    /// </summary>
    /// <param name="value">The string literal to use as a format argument.</param>
    public void AppendFormatted(string value)
    {
        _formatBuilder.Append('{');
        _formatBuilder.Append(_argumentIndex++);
        _formatBuilder.Append('}');
        _arguments.Add(new StringExpression(value));
    }

    /// <summary>
    ///     Appends a numeric hole as a <see cref="NumberExpression" /> argument.
    /// </summary>
    /// <param name="value">The numeric value to use as a format argument.</param>
    public void AppendFormatted<T>(T value)
        where T : INumber<T>
    {
        _formatBuilder.Append('{');
        _formatBuilder.Append(_argumentIndex++);
        _formatBuilder.Append('}');

        _arguments.Add(new NumberExpression(double.TryParse(value.ToString(), out var result)
            ? result
            : 0));
    }

    /// <summary>
    ///     Appends a boolean hole as a <see cref="BooleanExpression" /> argument.
    /// </summary>
    /// <param name="value">The boolean value to use as a format argument.</param>
    public void AppendFormatted(bool value)
    {
        _formatBuilder.Append('{');
        _formatBuilder.Append(_argumentIndex++);
        _formatBuilder.Append('}');
        _arguments.Add(new BooleanExpression(value));
    }

    /// <summary>
    ///     Produces the final <see cref="FormatExpression" /> from the accumulated format string and arguments.
    /// </summary>
    public FormatExpression ToFormatExpression() =>
        new(new StringExpression(_formatBuilder.ToString()), [.. _arguments]);
}
