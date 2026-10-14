namespace Invex.StructuredText.Expressions;

/// <summary>
///     One part of a <see cref="Template" />: either literal text or an embedded expression.
/// </summary>
/// <param name="Text">The literal text, when this part is text.</param>
/// <param name="Expression">The expression, when this part is an expression.</param>
[PublicAPI]
public readonly record struct TemplatePart(string? Text, Expr? Expression);

/// <summary>
///     A text value made of literal text and embedded expressions, such as a YAML scalar.
/// </summary>
/// <remarks>
///     <para>
///         Build one from a plain <see cref="string" /> (literal text), from any <see cref="Expr" /> (an embedded
///         expression), or from an interpolated string whose holes are expressions:
///         <c>$"{workflow}-{branch}"</c>. A printer wraps each embedded expression in the platform's
///         expression delimiters, so no manual evaluate or concatenate step is needed.
///     </para>
///     <para>Templates are immutable once built.</para>
/// </remarks>
[PublicAPI]
[InterpolatedStringHandler]
public sealed class Template : IEquatable<Template>
{
    private readonly List<TemplatePart> _parts;

    /// <summary>
    ///     Used by the compiler when an interpolated string is converted to a <see cref="Template" />.
    /// </summary>
    /// <param name="literalLength">The total length of the literal parts.</param>
    /// <param name="formattedCount">The number of holes.</param>
    public Template(int literalLength, int formattedCount)
    {
        _ = literalLength;
        _parts = new(formattedCount * 2 + 1);
    }

    private Template(IEnumerable<TemplatePart> parts)
    {
        _parts = [..parts];
    }

    /// <summary>
    ///     The parts of this template, in order.
    /// </summary>
    public IReadOnlyList<TemplatePart> Parts => _parts;

    /// <summary>
    ///     The expressions embedded in this template, in order.
    /// </summary>
    public IEnumerable<Expr> Expressions =>
        _parts
            .Where(x => x.Expression is not null)
            .Select(x => x.Expression!);

    /// <summary>
    ///     <see langword="true" /> when this template is a single embedded expression with no literal text.
    /// </summary>
    public bool IsSingleExpression => _parts is [{ Expression: not null }];

    /// <summary>
    ///     Appends literal text. Called by the compiler for interpolated strings.
    /// </summary>
    /// <param name="value">The literal text.</param>
    public void AppendLiteral(string value) =>
        AppendText(value);

    /// <summary>
    ///     Appends an embedded expression. Called by the compiler for interpolated strings.
    /// </summary>
    /// <param name="expression">The expression.</param>
    public void AppendFormatted(Expr expression) =>
        _parts.Add(new(null, expression));

    /// <summary>
    ///     Appends another template's parts. Called by the compiler for interpolated strings.
    /// </summary>
    /// <param name="template">The template to splice in.</param>
    public void AppendFormatted(Template template)
    {
        foreach (var part in template._parts)
            if (part.Expression is not null)
                _parts.Add(part);
            else
                AppendText(part.Text!);
    }

    /// <summary>
    ///     Appends a string hole as literal text. Called by the compiler for interpolated strings.
    /// </summary>
    /// <param name="value">The text.</param>
    public void AppendFormatted(string? value) =>
        AppendText(value ?? string.Empty);

    /// <summary>
    ///     Appends a value hole as literal text formatted with the invariant culture.
    ///     Called by the compiler for interpolated strings.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <typeparam name="T">The value type.</typeparam>
    public void AppendFormatted<T>(T value)
        where T : IFormattable =>
        AppendText(value.ToString(null, CultureInfo.InvariantCulture));

    /// <summary>
    ///     Creates a template containing only literal text.
    /// </summary>
    /// <param name="text">The text.</param>
    public static implicit operator Template(string text) =>
        new([new(text, null)]);

    /// <summary>
    ///     Creates a template containing a single embedded expression.
    /// </summary>
    /// <param name="expression">The expression.</param>
    public static implicit operator Template(Expr expression) =>
        new([new(null, expression)]);

    /// <summary>
    ///     Creates a template containing the literal text <c>true</c> or <c>false</c>.
    /// </summary>
    /// <param name="value">The value.</param>
    public static implicit operator Template(bool value) =>
        value
            ? "true"
            : "false";

    /// <summary>
    ///     Creates a template containing a number as literal text, formatted with the invariant culture.
    /// </summary>
    /// <param name="value">The value.</param>
    public static implicit operator Template(double value) =>
        value.ToString(CultureInfo.InvariantCulture);

    /// <summary>
    ///     Joins templates with <paramref name="separator" /> between each.
    /// </summary>
    /// <param name="separator">The literal separator.</param>
    /// <param name="templates">The templates to join.</param>
    /// <returns>The joined template.</returns>
    public static Template Join(string separator, IEnumerable<Template> templates)
    {
        var result = new Template([]);
        var first = true;

        foreach (var template in templates)
        {
            if (!first)
                result.AppendText(separator);

            result.AppendFormatted(template);
            first = false;
        }

        return result;
    }

    /// <summary>
    ///     Renders this template using a neutral expression syntax, for debugging and test output.
    /// </summary>
    /// <returns>The rendered text.</returns>
    public override string ToString() =>
        DebugExpressionPrinter.Instance.Render(this);

    /// <inheritdoc />
    public bool Equals(Template? other) =>
        other is not null && _parts.SequenceEqual(other._parts);

    /// <inheritdoc />
    public override bool Equals(object? obj) =>
        obj is Template other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode() =>
        _parts.Aggregate(0, (hash, part) => HashCode.Combine(hash, part));

    private void AppendText(string text)
    {
        if (text.Length is 0)
            return;

        if (_parts is [.., { Text: { } previous }])
            _parts[^1] = new(previous + text, null);
        else
            _parts.Add(new(text, null));
    }
}
