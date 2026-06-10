namespace Invex.StructuredText.Expressions;

partial record TextExpression
{
    /// <summary>
    ///     Tests whether this expression contains <paramref name="pattern" />, rendered as <c>contains(this, pattern)</c>.
    /// </summary>
    public ContainsExpression Contains(TextExpression pattern) =>
        new(this, pattern);

    /// <summary>
    ///     Tests whether this expression contains the string literal <paramref name="pattern" />,
    ///     rendered as <c>contains(this, 'pattern')</c>.
    /// </summary>
    public ContainsExpression Contains(string pattern) =>
        new(this, new StringExpression(pattern));

    /// <summary>
    ///     Tests whether this expression is contained in <paramref name="collection" /> (reversed
    ///     <see cref="Contains(TextExpression)" />), rendered as <c>contains(collection, this)</c>.
    /// </summary>
    public ContainsExpression ContainedIn(TextExpression collection) =>
        new(collection, this);

    /// <summary>
    ///     Tests whether this expression is contained in the string literal <paramref name="collection" />,
    ///     rendered as <c>contains('collection', this)</c>.
    /// </summary>
    public ContainsExpression ContainedIn(string collection) =>
        new(new StringExpression(collection), this);

    /// <summary>
    ///     Returns the first non-empty value of this expression and <paramref name="source" />,
    ///     rendered as <c>coalesce(this, ...)</c>.
    /// </summary>
    public CoalesceExpression Coalesce(params TextExpression[] source) =>
        new([this, ..source]);

    /// <summary>
    ///     Returns the first non-empty value of this expression and the string literals in <paramref name="source" />.
    /// </summary>
    public CoalesceExpression Coalesce(params string[] source) =>
        new([this, ..source.Select(x => new StringExpression(x))]);

    /// <summary>
    ///     Tests whether this expression starts with <paramref name="pattern" />,
    ///     rendered as <c>startsWith(this, pattern)</c>.
    /// </summary>
    public StartsWithExpression StartsWith(TextExpression pattern) =>
        new(this, pattern);

    /// <summary>
    ///     Tests whether this expression starts with the string literal <paramref name="pattern" />.
    /// </summary>
    public StartsWithExpression StartsWith(string pattern) =>
        new(this, new StringExpression(pattern));

    /// <summary>
    ///     Tests whether this expression is the start of <paramref name="pattern" /> (reversed
    ///     <see cref="StartsWith(TextExpression)" />), rendered as <c>startsWith(pattern, this)</c>.
    /// </summary>
    public StartsWithExpression IsStartOf(TextExpression pattern) =>
        new(pattern, this);

    /// <summary>
    ///     Tests whether this expression is the start of the string literal <paramref name="pattern" />.
    /// </summary>
    public StartsWithExpression IsStartOf(string pattern) =>
        new(new StringExpression(pattern), this);

    /// <summary>
    ///     Tests whether this expression ends with <paramref name="pattern" />,
    ///     rendered as <c>endsWith(this, pattern)</c>.
    /// </summary>
    public EndsWithExpression EndsWith(TextExpression pattern) =>
        new(this, pattern);

    /// <summary>
    ///     Tests whether this expression ends with the string literal <paramref name="pattern" />.
    /// </summary>
    public EndsWithExpression EndsWith(string pattern) =>
        new(this, new StringExpression(pattern));

    /// <summary>
    ///     Tests whether this expression is the end of <paramref name="pattern" /> (reversed
    ///     <see cref="EndsWith(TextExpression)" />), rendered as <c>endsWith(pattern, this)</c>.
    /// </summary>
    public EndsWithExpression IsEndOf(TextExpression pattern) =>
        new(pattern, this);

    /// <summary>
    ///     Tests whether this expression is the end of the string literal <paramref name="pattern" />.
    /// </summary>
    public EndsWithExpression IsEndOfString(string pattern) =>
        new(new StringExpression(pattern), this);

    /// <summary>
    ///     Uses this expression as a format string with <paramref name="arguments" /> as replacement values,
    ///     rendered as <c>format(this, args...)</c>.
    /// </summary>
    public FormatExpression Format(params TextExpression[] arguments) =>
        new(this, arguments);

    /// <summary>
    ///     Uses this expression as a format string with string-literal <paramref name="arguments" />.
    /// </summary>
    public FormatExpression FormatString(params string[] arguments) =>
        new(this,
            arguments
                .Select(x => new StringExpression(x))
                .ToArray<TextExpression>());

    /// <summary>
    ///     Uses <paramref name="source" /> as a format string with <paramref name="argument" /> as a replacement value.
    /// </summary>
    public static FormatExpression operator +(TextExpression source, TextExpression argument) =>
        new(source, argument);

    /// <summary>
    ///     Uses <paramref name="source" /> as a format string with <paramref name="arguments" /> as replacement values.
    /// </summary>
    public static FormatExpression operator +(TextExpression source, TextExpression[] arguments) =>
        new(source, arguments);

    /// <summary>
    ///     Uses <paramref name="source" /> as a format string with the string literal <paramref name="argument" />.
    /// </summary>
    public static FormatExpression operator +(TextExpression source, string argument) =>
        new(source, new StringExpression(argument));

    /// <summary>
    ///     Uses <paramref name="source" /> as a format string with string-literal <paramref name="arguments" />.
    /// </summary>
    public static FormatExpression operator +(TextExpression source, string[] arguments) =>
        new(source,
            arguments
                .Select(x => new StringExpression(x))
                .ToArray<TextExpression>());

    /// <summary>
    ///     Joins the elements of this (array-valued) expression with <paramref name="separator" />,
    ///     rendered as <c>join(this, separator)</c>.
    /// </summary>
    public JoinExpression Join(TextExpression separator) =>
        new(this, separator);

    /// <summary>
    ///     Joins the elements of this (array-valued) expression with the string literal <paramref name="separator" />.
    /// </summary>
    public JoinExpression JoinString(string separator) =>
        new(this, new StringExpression(separator));

    /// <summary>
    ///     Converts this expression to its JSON representation,
    ///     rendered as the platform's JSON serialization function.
    /// </summary>
    public ToJsonExpression ToJson() =>
        new(this);

    /// <summary>
    ///     Hashes the files matched by this (glob pattern) expression, rendered as <c>hashFiles(this)</c>.
    /// </summary>
    public HashFilesExpression HashFiles() =>
        new(this);
}

/// <summary>
///     Tests whether <paramref name="Source" /> contains <paramref name="Pattern" />,
///     rendered as <c>contains(source, pattern)</c>.
/// </summary>
/// <param name="Source">The value or collection to search.</param>
/// <param name="Pattern">The item or substring to search for.</param>
[PublicAPI]
public sealed record ContainsExpression(TextExpression Source, TextExpression Pattern)
    : TextExpression, ITextExpression<bool>;

/// <summary>
///     Returns the first non-empty value of <paramref name="Source" />,
///     rendered as <c>coalesce(...)</c>.
/// </summary>
/// <param name="Source">The candidate expressions, evaluated in order.</param>
[PublicAPI]
public sealed record CoalesceExpression(params TextExpression[] Source) : TextExpression
{
    protected override bool PrintMembers(StringBuilder builder)
    {
        if (base.PrintMembers(builder))
            builder.Append(", ");

        builder.Append("[ ");

        for (var i = 0; i < Source.Length; i++)
        {
            if (i < Source.Length - 1)
                builder.Append(", ");

            builder.Append(Source[i]);
        }

        builder.Append(" ]");

        return true;
    }
}

/// <summary>
///     Tests whether <paramref name="Source" /> starts with <paramref name="Pattern" />,
///     rendered as <c>startsWith(source, pattern)</c>.
/// </summary>
/// <param name="Source">The value to test.</param>
/// <param name="Pattern">The prefix to test for.</param>
[PublicAPI]
public sealed record StartsWithExpression(TextExpression Source, TextExpression Pattern)
    : TextExpression, ITextExpression<bool>;

/// <summary>
///     Tests whether <paramref name="Source" /> ends with <paramref name="Pattern" />,
///     rendered as <c>endsWith(source, pattern)</c>.
/// </summary>
/// <param name="Source">The value to test.</param>
/// <param name="Pattern">The suffix to test for.</param>
[PublicAPI]
public sealed record EndsWithExpression(TextExpression Source, TextExpression Pattern)
    : TextExpression, ITextExpression<bool>;

/// <summary>
///     Replaces placeholders (<c>{0}</c>, <c>{1}</c>, …) in <paramref name="Source" /> with
///     <paramref name="Arguments" />,
///     rendered as <c>format(source, args...)</c>.
///     Usually built via <c>TextExpressions.Format($"...")</c> string interpolation.
/// </summary>
/// <param name="Source">The format string expression.</param>
/// <param name="Arguments">The replacement values.</param>
[PublicAPI]
public sealed record FormatExpression(TextExpression Source, params TextExpression[] Arguments)
    : TextExpression, ITextExpression<string>
{
    protected override bool PrintMembers(StringBuilder builder)
    {
        if (base.PrintMembers(builder))
            builder.Append(", ");

        builder.Append(Source);
        builder.Append(", ");

        builder.Append("[ ");

        for (var i = 0; i < Arguments.Length; i++)
        {
            if (i < Arguments.Length - 1)
                builder.Append(", ");

            builder.Append(Arguments[i]);
        }

        builder.Append(" ]");

        return true;
    }
}

/// <summary>
///     Joins the elements of the array-valued <paramref name="Source" /> with
///     <paramref name="OptionalSeparator" /> (comma when omitted),
///     rendered as <c>join(source, separator)</c>.
/// </summary>
/// <param name="Source">The array-valued expression to join.</param>
/// <param name="OptionalSeparator">The separator, or <c>null</c> for the platform default.</param>
[PublicAPI]
public sealed record JoinExpression(TextExpression Source, TextExpression? OptionalSeparator)
    : TextExpression, ITextExpression<string>;

/// <summary>
///     Converts <paramref name="Source" /> to its JSON representation,
///     rendered as the platform's JSON serialization function.
/// </summary>
/// <param name="Source">The value to convert.</param>
[PublicAPI]
public sealed record ToJsonExpression(TextExpression Source) : TextExpression, ITextExpression<string>;

/// <summary>
///     Returns a hash of the files matched by the glob pattern(s) in <paramref name="Source" />,
///     rendered as <c>hashFiles(source)</c>. Commonly used for cache keys.
/// </summary>
/// <param name="Source">The glob pattern expression.</param>
[PublicAPI]
public sealed record HashFilesExpression(TextExpression Source) : TextExpression, ITextExpression<string>;
