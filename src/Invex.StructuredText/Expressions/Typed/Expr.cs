namespace Invex.StructuredText.Expressions;

/// <summary>
///     The base type of the typed expression tree. Every node derives from one of the four result-typed roots:
///     <see cref="BoolExpr" />, <see cref="StringExpr" />, <see cref="NumberExpr" /> or <see cref="JsonExpr" />.
/// </summary>
/// <remarks>
///     Nodes are immutable records. They are rendered to platform syntax by an <see cref="ExpressionPrinter" />;
///     <see cref="ToString" /> uses a neutral infix rendering intended for debugging and test output.
/// </remarks>
[PublicAPI]
public abstract record Expr
{
    /// <summary>
    ///     The direct sub-expressions of this node, in left-to-right order.
    /// </summary>
    public abstract IEnumerable<Expr> Children { get; }

    /// <summary>
    ///     Converts a <see cref="string" /> to a string literal where any expression is accepted,
    ///     e.g. <c>Tex.Format("{0}", "hello")</c>.
    /// </summary>
    /// <param name="value">The literal value, unquoted.</param>
    public static implicit operator Expr(string value) =>
        new StringLiteral(value);

    /// <summary>
    ///     Converts a <see cref="double" /> to a numeric literal where any expression is accepted.
    /// </summary>
    /// <param name="value">The literal value.</param>
    public static implicit operator Expr(double value) =>
        new NumberLiteral(value);

    /// <summary>
    ///     Converts a <see cref="bool" /> to a boolean literal where any expression is accepted.
    /// </summary>
    /// <param name="value">The literal value.</param>
    public static implicit operator Expr(bool value) =>
        new BoolLiteral(value);

    /// <summary>
    ///     Enumerates this node and every node beneath it, depth first, in left-to-right order.
    /// </summary>
    /// <returns>This node followed by all of its descendants.</returns>
    public IEnumerable<Expr> DescendantsAndSelf()
    {
        yield return this;

        foreach (var child in Children)
        foreach (var descendant in child.DescendantsAndSelf())
            yield return descendant;
    }

    /// <summary>
    ///     Renders this expression with a neutral infix syntax, for debugging and test output.
    /// </summary>
    /// <returns>The rendered expression.</returns>
    public sealed override string ToString() =>
        DebugExpressionPrinter.Instance.Print(this);
}

/// <summary>
///     An expression that evaluates to a boolean.
/// </summary>
/// <remarks>
///     <para>
///         Combine conditions with <c>&amp;&amp;</c>, <c>||</c> and <c>!</c> (or <c>&amp;</c> and <c>|</c>).
///         The operators build expression nodes; they never evaluate anything.
///     </para>
///     <para>
///         Because <see langword="true" /> and <see langword="false" /> operators are defined to support
///         <c>&amp;&amp;</c> and <c>||</c>, a <see cref="BoolExpr" /> used directly as a C# condition
///         (e.g. <c>if (expr)</c>) compiles but is always <see langword="false" />.
///     </para>
/// </remarks>
[PublicAPI]
public abstract record BoolExpr : Expr
{
    /// <summary>
    ///     Converts a <see cref="bool" /> to a boolean literal.
    /// </summary>
    /// <param name="value">The literal value.</param>
    public static implicit operator BoolExpr(bool value) =>
        new BoolLiteral(value);

    /// <summary>
    ///     Combines two conditions with logical AND.
    /// </summary>
    /// <param name="left">The left operand.</param>
    /// <param name="right">The right operand.</param>
    public static BoolExpr operator &(BoolExpr left, BoolExpr right) =>
        LogicalExpr.Combine(LogicalOperator.And, left, right);

    /// <summary>
    ///     Combines two conditions with logical OR.
    /// </summary>
    /// <param name="left">The left operand.</param>
    /// <param name="right">The right operand.</param>
    public static BoolExpr operator |(BoolExpr left, BoolExpr right) =>
        LogicalExpr.Combine(LogicalOperator.Or, left, right);

    /// <summary>
    ///     Logically negates a condition.
    /// </summary>
    /// <param name="operand">The condition to negate.</param>
    public static BoolExpr operator !(BoolExpr operand) =>
        operand is NotExpr not
            ? not.Operand
            : new NotExpr(operand);

    /// <summary>
    ///     Always returns <see langword="false" />. Exists only so that <c>&amp;&amp;</c> builds an AND node.
    /// </summary>
    /// <param name="operand">Ignored.</param>
    public static bool operator true(BoolExpr operand) =>
        false;

    /// <summary>
    ///     Always returns <see langword="false" />. Exists only so that <c>||</c> builds an OR node.
    /// </summary>
    /// <param name="operand">Ignored.</param>
    public static bool operator false(BoolExpr operand) =>
        false;


    /// <summary>
    ///     Tests this condition for equality with <paramref name="other" />.
    /// </summary>
    /// <param name="other">The value to compare with.</param>
    public BoolExpr Eq(BoolExpr other) =>
        new ComparisonExpr(ComparisonOperator.Equal, this, other);

    /// <summary>
    ///     Tests this condition for inequality with <paramref name="other" />.
    /// </summary>
    /// <param name="other">The value to compare with.</param>
    public BoolExpr Ne(BoolExpr other) =>
        new ComparisonExpr(ComparisonOperator.NotEqual, this, other);
}

/// <summary>
///     An expression that evaluates to a string.
/// </summary>
/// <remarks>
///     A plain <see cref="string" /> converts implicitly to a quoted, escaped string literal.
///     Use <see cref="Tex.Raw" /> for verbatim expression text.
/// </remarks>
[PublicAPI]
public abstract record StringExpr : Expr
{
    /// <summary>
    ///     Converts a <see cref="string" /> to a string literal.
    /// </summary>
    /// <param name="value">The literal value, unquoted.</param>
    public static implicit operator StringExpr(string value) =>
        new StringLiteral(value);


    /// <summary>
    ///     Tests this value for equality with <paramref name="other" />.
    /// </summary>
    /// <param name="other">The value to compare with.</param>
    public BoolExpr Eq(StringExpr other) =>
        new ComparisonExpr(ComparisonOperator.Equal, this, other);

    /// <summary>
    ///     Tests this value for inequality with <paramref name="other" />.
    /// </summary>
    /// <param name="other">The value to compare with.</param>
    public BoolExpr Ne(StringExpr other) =>
        new ComparisonExpr(ComparisonOperator.NotEqual, this, other);

    /// <summary>
    ///     Tests whether this value contains <paramref name="value" />.
    /// </summary>
    /// <param name="value">The substring to search for.</param>
    public BoolExpr Contains(StringExpr value) =>
        new BoolFunction(FunctionNames.Contains, this, value);

    /// <summary>
    ///     Tests whether this value starts with <paramref name="value" />.
    /// </summary>
    /// <param name="value">The prefix to test for.</param>
    public BoolExpr StartsWith(StringExpr value) =>
        new BoolFunction(FunctionNames.StartsWith, this, value);

    /// <summary>
    ///     Tests whether this value ends with <paramref name="value" />.
    /// </summary>
    /// <param name="value">The suffix to test for.</param>
    public BoolExpr EndsWith(StringExpr value) =>
        new BoolFunction(FunctionNames.EndsWith, this, value);

    /// <summary>
    ///     Tests whether this value is one of the elements of <paramref name="collection" />.
    /// </summary>
    /// <param name="collection">The array-valued expression to search.</param>
    public BoolExpr In(JsonExpr collection) =>
        new BoolFunction(FunctionNames.Contains, collection, this);
}

/// <summary>
///     An expression that evaluates to a number.
/// </summary>
[PublicAPI]
public abstract record NumberExpr : Expr
{
    /// <summary>
    ///     Converts a <see cref="double" /> to a numeric literal.
    /// </summary>
    /// <param name="value">The literal value.</param>
    public static implicit operator NumberExpr(double value) =>
        new NumberLiteral(value);


    /// <summary>Tests this value for equality with <paramref name="other" />.</summary>
    /// <param name="other">The value to compare with.</param>
    public BoolExpr Eq(NumberExpr other) =>
        new ComparisonExpr(ComparisonOperator.Equal, this, other);

    /// <summary>Tests this value for inequality with <paramref name="other" />.</summary>
    /// <param name="other">The value to compare with.</param>
    public BoolExpr Ne(NumberExpr other) =>
        new ComparisonExpr(ComparisonOperator.NotEqual, this, other);

    /// <summary>Tests whether this value is less than <paramref name="other" />.</summary>
    /// <param name="other">The value to compare with.</param>
    public BoolExpr Lt(NumberExpr other) =>
        new ComparisonExpr(ComparisonOperator.LessThan, this, other);

    /// <summary>Tests whether this value is less than or equal to <paramref name="other" />.</summary>
    /// <param name="other">The value to compare with.</param>
    public BoolExpr Le(NumberExpr other) =>
        new ComparisonExpr(ComparisonOperator.LessThanOrEqual, this, other);

    /// <summary>Tests whether this value is greater than <paramref name="other" />.</summary>
    /// <param name="other">The value to compare with.</param>
    public BoolExpr Gt(NumberExpr other) =>
        new ComparisonExpr(ComparisonOperator.GreaterThan, this, other);

    /// <summary>Tests whether this value is greater than or equal to <paramref name="other" />.</summary>
    /// <param name="other">The value to compare with.</param>
    public BoolExpr Ge(NumberExpr other) =>
        new ComparisonExpr(ComparisonOperator.GreaterThanOrEqual, this, other);
}

/// <summary>
///     An expression whose value is an object, an array, or of a type not known statically.
/// </summary>
/// <remarks>
///     Navigate with the indexers and convert with <see cref="AsString" />, <see cref="AsBool" /> or
///     <see cref="AsNumber" /> when the value's type is known.
/// </remarks>
[PublicAPI]
public abstract record JsonExpr : Expr
{
    /// <summary>
    ///     Accesses a property of this object. The name is a single key: it may contain dots or spaces, and the
    ///     printer uses <c>['name']</c> syntax where <c>.name</c> isn't valid.
    /// </summary>
    /// <param name="property">The property name.</param>
    public JsonExpr this[string property] =>
        this is IContextPath context && ExpressionPrinter.IsDotAccessible(property)
            ? new JsonContext($"{context.Path}.{property}")
            : new MemberAccessExpr(this, property);

    /// <summary>
    ///     Accesses an element of this array.
    /// </summary>
    /// <param name="index">The zero-based index.</param>
    public JsonExpr this[int index] =>
        new IndexAccessExpr(this, new NumberLiteral(index));


    /// <summary>
    ///     Treats this value as a string.
    /// </summary>
    public StringExpr AsString() =>
        this is IContextPath context
            ? new StringContext(context.Path)
            : new StringCast(this);

    /// <summary>
    ///     Treats this value as a boolean.
    /// </summary>
    public BoolExpr AsBool() =>
        this is IContextPath context
            ? new BoolContext(context.Path)
            : new BoolCast(this);

    /// <summary>
    ///     Treats this value as a number.
    /// </summary>
    public NumberExpr AsNumber() =>
        this is IContextPath context
            ? new NumberContext(context.Path)
            : new NumberCast(this);

    /// <summary>
    ///     Tests whether this array or string contains <paramref name="value" />.
    /// </summary>
    /// <param name="value">The element or substring to search for.</param>
    public BoolExpr Contains(Expr value) =>
        new BoolFunction(FunctionNames.Contains, this, value);

    /// <summary>
    ///     Joins the elements of this array with <paramref name="separator" />.
    /// </summary>
    /// <param name="separator">The separator; the platform default when <see langword="null" />.</param>
    public StringExpr Join(StringExpr? separator = null) =>
        separator is null
            ? new StringFunction(FunctionNames.Join, this)
            : new StringFunction(FunctionNames.Join, this, separator);

    /// <summary>
    ///     Serializes this value to JSON text.
    /// </summary>
    public StringExpr ToJson() =>
        new StringFunction(FunctionNames.ToJson, this);
}
