namespace Invex.StructuredText.Expressions;

partial record TextExpression
{
    /// <summary>
    ///     Logically negates this expression.
    ///     The platform formatter decides the rendered syntax (e.g. prefix <c>!</c> or function <c>not(...)</c>).
    /// </summary>
    public NotExpression Not() =>
        new(this);

    /// <summary>
    ///     Logically negates <paramref name="source" />.
    /// </summary>
    public static NotExpression operator !(TextExpression source) =>
        new(source);

    /// <summary>
    ///     Combines this expression with <paramref name="expressions" /> using logical AND.
    ///     The platform formatter decides the rendered syntax (e.g. <c>&amp;&amp;</c> or <c>and(...)</c>).
    /// </summary>
    public AndExpression And(params TextExpression[] expressions) =>
        new([this, ..expressions]);

    /// <summary>
    ///     Combines <paramref name="left" /> and <paramref name="right" /> using logical AND.
    /// </summary>
    public static AndExpression operator &(TextExpression left, TextExpression right) =>
        new(left, right);

    /// <summary>
    ///     Combines this expression with <paramref name="expressions" /> using logical OR.
    ///     The platform formatter decides the rendered syntax (e.g. <c>||</c> or <c>or(...)</c>).
    /// </summary>
    public OrExpression Or(params TextExpression[] expressions) =>
        new([this, ..expressions]);

    /// <summary>
    ///     Combines <paramref name="left" /> and <paramref name="right" /> using logical OR.
    /// </summary>
    public static OrExpression operator |(TextExpression left, TextExpression right) =>
        new(left, right);

    /// <summary>
    ///     Tests this expression for equality with <paramref name="right" />.
    ///     The platform formatter decides the rendered syntax (e.g. <c>==</c> or <c>eq(...)</c>).
    /// </summary>
    public EqualExpression EqualTo(TextExpression right) =>
        new(this, right);

    /// <summary>
    ///     Tests this expression for equality with the verbatim text <paramref name="right" />.
    ///     Use <see cref="EqualToString" /> to compare with a quoted string literal.
    /// </summary>
    public EqualExpression EqualTo(string right) =>
        new(this, new RawExpression(right));

    /// <summary>
    ///     Tests this expression for equality with the string literal <paramref name="right" />,
    ///     e.g. <c>this == 'value'</c>.
    /// </summary>
    public EqualExpression EqualToString(string right) =>
        new(this, new StringExpression(right));

    /// <summary>
    ///     Tests this expression for inequality with <paramref name="right" />.
    ///     The platform formatter decides the rendered syntax (e.g. <c>!=</c> or <c>ne(...)</c>).
    /// </summary>
    public NotEqualExpression NotEqualTo(TextExpression right) =>
        new(this, right);

    /// <summary>
    ///     Tests this expression for inequality with the verbatim text <paramref name="right" />.
    ///     Use <see cref="NotEqualToString" /> to compare with a quoted string literal.
    /// </summary>
    public NotEqualExpression NotEqualTo(string right) =>
        new(this, new RawExpression(right));

    /// <summary>
    ///     Tests this expression for inequality with the string literal <paramref name="right" />,
    ///     e.g. <c>this != 'value'</c>.
    /// </summary>
    public NotEqualExpression NotEqualToString(string right) =>
        new(this, new StringExpression(right));

    /// <summary>
    ///     Tests whether this expression is less than <paramref name="right" />.
    ///     The platform formatter decides the rendered syntax (e.g. <c>&lt;</c> or <c>lt(...)</c>).
    /// </summary>
    public LessThanExpression LessThan(TextExpression right) =>
        new(this, right);

    /// <summary>
    ///     Tests whether this expression is less than the verbatim text <paramref name="right" />.
    /// </summary>
    public LessThanExpression LessThan(string right) =>
        new(this, new RawExpression(right));

    /// <summary>
    ///     Tests whether this expression is less than the string literal <paramref name="right" />.
    /// </summary>
    public LessThanExpression LessThanString(string right) =>
        new(this, new StringExpression(right));

    /// <summary>
    ///     Tests whether this expression is greater than <paramref name="right" />.
    ///     The platform formatter decides the rendered syntax (e.g. <c>&gt;</c> or <c>gt(...)</c>).
    /// </summary>
    public GreaterThanExpression GreaterThan(TextExpression right) =>
        new(this, right);

    /// <summary>
    ///     Tests whether this expression is greater than the verbatim text <paramref name="right" />.
    /// </summary>
    public GreaterThanExpression GreaterThan(string right) =>
        new(this, new RawExpression(right));

    /// <summary>
    ///     Tests whether this expression is greater than the string literal <paramref name="right" />.
    /// </summary>
    public GreaterThanExpression GreaterThanString(string right) =>
        new(this, new StringExpression(right));

    /// <summary>
    ///     Tests whether this expression is less than or equal to <paramref name="right" />.
    ///     The platform formatter decides the rendered syntax (e.g. <c>&lt;=</c> or <c>le(...)</c>).
    /// </summary>
    public LessThanOrEqualToExpression LessThanOrEqualTo(TextExpression right) =>
        new(this, right);

    /// <summary>
    ///     Tests whether this expression is less than or equal to the verbatim text <paramref name="right" />.
    /// </summary>
    public LessThanOrEqualToExpression LessThanOrEqualTo(string right) =>
        new(this, new RawExpression(right));

    /// <summary>
    ///     Tests whether this expression is less than or equal to the string literal <paramref name="right" />.
    /// </summary>
    public LessThanOrEqualToExpression LessThanOrEqualToString(string right) =>
        new(this, new StringExpression(right));

    /// <summary>
    ///     Tests whether this expression is greater than or equal to <paramref name="right" />.
    ///     The platform formatter decides the rendered syntax (e.g. <c>&gt;=</c> or <c>ge(...)</c>).
    /// </summary>
    public GreaterThanOrEqualToExpression GreaterThanOrEqualTo(TextExpression right) =>
        new(this, right);

    /// <summary>
    ///     Tests whether this expression is greater than or equal to the verbatim text <paramref name="right" />.
    /// </summary>
    public GreaterThanOrEqualToExpression GreaterThanOrEqualTo(string right) =>
        new(this, new RawExpression(right));

    /// <summary>
    ///     Tests whether this expression is greater than or equal to the string literal <paramref name="right" />.
    /// </summary>
    public GreaterThanOrEqualToExpression GreaterThanOrEqualToString(string right) =>
        new(this, new StringExpression(right));
}

/// <summary>
///     Logical negation of <paramref name="Source" />.
///     The platform formatter decides the rendered syntax (e.g. prefix <c>!</c> or <c>not(source)</c>).
/// </summary>
/// <param name="Source">The expression to negate.</param>
[PublicAPI]
public sealed record NotExpression(TextExpression Source) : TextExpression, ITextExpression<bool>;

/// <summary>
///     Logical AND over <paramref name="Source" />.
///     The platform formatter decides the rendered syntax (e.g. <c>a &amp;&amp; b</c> or <c>and(a, b)</c>).
/// </summary>
/// <param name="Source">The operands, combined left to right.</param>
[PublicAPI]
public sealed record AndExpression(params TextExpression[] Source) : TextExpression, ITextExpression<bool>
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
///     Logical OR over <paramref name="Source" />.
///     The platform formatter decides the rendered syntax (e.g. <c>a || b</c> or <c>or(a, b)</c>).
/// </summary>
/// <param name="Source">The operands, combined left to right.</param>
[PublicAPI]
public sealed record OrExpression(params TextExpression[] Source) : TextExpression, ITextExpression<bool>
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
///     Equality comparison. The platform formatter decides the rendered syntax (e.g. <c>left == right</c> or <c>eq(left, right)</c>).
/// </summary>
/// <param name="Left">The left operand.</param>
/// <param name="Right">The right operand.</param>
[PublicAPI]
public sealed record EqualExpression(TextExpression Left, TextExpression Right) : TextExpression, ITextExpression<bool>;

/// <summary>
///     Inequality comparison. The platform formatter decides the rendered syntax (e.g. <c>left != right</c> or <c>ne(left, right)</c>).
/// </summary>
/// <param name="Left">The left operand.</param>
/// <param name="Right">The right operand.</param>
[PublicAPI]
public sealed record NotEqualExpression(TextExpression Left, TextExpression Right)
    : TextExpression, ITextExpression<bool>;

/// <summary>
///     Less-than comparison. The platform formatter decides the rendered syntax (e.g. <c>left &lt; right</c> or <c>lt(left, right)</c>).
/// </summary>
/// <param name="Left">The left operand.</param>
/// <param name="Right">The right operand.</param>
[PublicAPI]
public sealed record LessThanExpression(TextExpression Left, TextExpression Right)
    : TextExpression, ITextExpression<bool>;

/// <summary>
///     Greater-than comparison. The platform formatter decides the rendered syntax (e.g. <c>left &gt; right</c> or <c>gt(left, right)</c>).
/// </summary>
/// <param name="Left">The left operand.</param>
/// <param name="Right">The right operand.</param>
[PublicAPI]
public sealed record GreaterThanExpression(TextExpression Left, TextExpression Right)
    : TextExpression, ITextExpression<bool>;

/// <summary>
///     Less-than-or-equal comparison. The platform formatter decides the rendered syntax (e.g. <c>left &lt;= right</c> or <c>le(left, right)</c>).
/// </summary>
/// <param name="Left">The left operand.</param>
/// <param name="Right">The right operand.</param>
[PublicAPI]
public sealed record LessThanOrEqualToExpression(TextExpression Left, TextExpression Right)
    : TextExpression, ITextExpression<bool>;

/// <summary>
///     Greater-than-or-equal comparison. The platform formatter decides the rendered syntax (e.g. <c>left &gt;= right</c> or <c>ge(left, right)</c>).
/// </summary>
/// <param name="Left">The left operand.</param>
/// <param name="Right">The right operand.</param>
[PublicAPI]
public sealed record GreaterThanOrEqualToExpression(TextExpression Left, TextExpression Right)
    : TextExpression, ITextExpression<bool>;
