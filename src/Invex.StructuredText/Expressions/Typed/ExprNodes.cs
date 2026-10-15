namespace Invex.StructuredText.Expressions;

// ---------------------------------------------------------------------------------------------------------------
// Literals
// ---------------------------------------------------------------------------------------------------------------

/// <summary>A string literal, rendered quoted and escaped.</summary>
/// <param name="Value">The literal value, unquoted.</param>
[PublicAPI]
public sealed record StringLiteral(string Value) : StringExpr
{
    /// <inheritdoc />
    public override IEnumerable<Expr> Children => [];
}

/// <summary>A numeric literal, rendered with the invariant culture.</summary>
/// <param name="Value">The literal value.</param>
[PublicAPI]
public sealed record NumberLiteral(double Value) : NumberExpr
{
    /// <inheritdoc />
    public override IEnumerable<Expr> Children => [];
}

/// <summary>A boolean literal.</summary>
/// <param name="Value">The literal value.</param>
[PublicAPI]
public sealed record BoolLiteral(bool Value) : BoolExpr
{
    /// <inheritdoc />
    public override IEnumerable<Expr> Children => [];
}

/// <summary>The null literal.</summary>
[PublicAPI]
public sealed record NullLiteral : JsonExpr
{
    /// <inheritdoc />
    public override IEnumerable<Expr> Children => [];
}

// ---------------------------------------------------------------------------------------------------------------
// Raw text
// ---------------------------------------------------------------------------------------------------------------

/// <summary>Implemented by nodes that hold verbatim expression text.</summary>
[PublicAPI]
public interface IRawExpr
{
    /// <summary>The text, written exactly as given.</summary>
    string Text { get; }
}

/// <summary>Verbatim string-valued expression text.</summary>
/// <param name="Text">The text, written exactly as given.</param>
[PublicAPI]
public sealed record RawString(string Text) : StringExpr, IRawExpr
{
    /// <inheritdoc />
    public override IEnumerable<Expr> Children => [];
}

/// <summary>
///     Verbatim expression text, created by <see cref="Tex.Raw" />. It is a condition, and converts implicitly to
///     string, number and JSON expressions.
/// </summary>
/// <param name="Text">The text, written exactly as given.</param>
[PublicAPI]
public sealed record RawExpr(string Text) : BoolExpr, IRawExpr
{
    /// <inheritdoc />
    public override IEnumerable<Expr> Children => [];

    /// <summary>Treats the text as a string expression.</summary>
    /// <param name="raw">The raw text.</param>
    public static implicit operator StringExpr(RawExpr raw) =>
        new RawString(raw.Text);

    /// <summary>Treats the text as a number expression.</summary>
    /// <param name="raw">The raw text.</param>
    public static implicit operator NumberExpr(RawExpr raw) =>
        new RawNumber(raw.Text);

    /// <summary>Treats the text as a JSON expression.</summary>
    /// <param name="raw">The raw text.</param>
    public static implicit operator JsonExpr(RawExpr raw) =>
        new RawJson(raw.Text);
}

/// <summary>Verbatim number-valued expression text.</summary>
/// <param name="Text">The text, written exactly as given.</param>
[PublicAPI]
public sealed record RawNumber(string Text) : NumberExpr, IRawExpr
{
    /// <inheritdoc />
    public override IEnumerable<Expr> Children => [];
}

/// <summary>Verbatim expression text of an object, array or unknown type.</summary>
/// <param name="Text">The text, written exactly as given.</param>
[PublicAPI]
public sealed record RawJson(string Text) : JsonExpr, IRawExpr
{
    /// <inheritdoc />
    public override IEnumerable<Expr> Children => [];
}

// ---------------------------------------------------------------------------------------------------------------
// Context paths (platform catalogues are built from these)
// ---------------------------------------------------------------------------------------------------------------

/// <summary>Implemented by nodes that reference a dotted context path such as <c>a.b.c</c>.</summary>
[PublicAPI]
public interface IContextPath
{
    /// <summary>The dotted path.</summary>
    string Path { get; }
}

/// <summary>A string-valued context path.</summary>
/// <param name="Path">The dotted path.</param>
[PublicAPI]
public sealed record StringContext(string Path) : StringExpr, IContextPath
{
    /// <inheritdoc />
    public override IEnumerable<Expr> Children => [];
}

/// <summary>A boolean-valued context path.</summary>
/// <param name="Path">The dotted path.</param>
[PublicAPI]
public sealed record BoolContext(string Path) : BoolExpr, IContextPath
{
    /// <inheritdoc />
    public override IEnumerable<Expr> Children => [];
}

/// <summary>A number-valued context path.</summary>
/// <param name="Path">The dotted path.</param>
[PublicAPI]
public sealed record NumberContext(string Path) : NumberExpr, IContextPath
{
    /// <inheritdoc />
    public override IEnumerable<Expr> Children => [];
}

/// <summary>A context path whose value is an object, an array or of unknown type.</summary>
/// <param name="Path">The dotted path.</param>
[PublicAPI]
public sealed record JsonContext(string Path) : JsonExpr, IContextPath
{
    /// <inheritdoc />
    public override IEnumerable<Expr> Children => [];
}

// ---------------------------------------------------------------------------------------------------------------
// Access and casts
// ---------------------------------------------------------------------------------------------------------------

/// <summary>Accesses <paramref name="Property" /> of <paramref name="Target" />.</summary>
/// <param name="Target">The object.</param>
/// <param name="Property">The property name.</param>
[PublicAPI]
public sealed record MemberAccessExpr(JsonExpr Target, string Property) : JsonExpr
{
    /// <inheritdoc />
    public override IEnumerable<Expr> Children => [Target];
}

/// <summary>Accesses element <paramref name="Index" /> of <paramref name="Target" />.</summary>
/// <param name="Target">The array.</param>
/// <param name="Index">The index expression.</param>
[PublicAPI]
public sealed record IndexAccessExpr(JsonExpr Target, Expr Index) : JsonExpr
{
    /// <inheritdoc />
    public override IEnumerable<Expr> Children => [Target, Index];
}

/// <summary>Treats <paramref name="Operand" /> as a string. Rendered transparently.</summary>
/// <param name="Operand">The value.</param>
[PublicAPI]
public sealed record StringCast(Expr Operand) : StringExpr
{
    /// <inheritdoc />
    public override IEnumerable<Expr> Children => [Operand];
}

/// <summary>Treats <paramref name="Operand" /> as a boolean. Rendered transparently.</summary>
/// <param name="Operand">The value.</param>
[PublicAPI]
public sealed record BoolCast(Expr Operand) : BoolExpr
{
    /// <inheritdoc />
    public override IEnumerable<Expr> Children => [Operand];
}

/// <summary>Treats <paramref name="Operand" /> as a number. Rendered transparently.</summary>
/// <param name="Operand">The value.</param>
[PublicAPI]
public sealed record NumberCast(Expr Operand) : NumberExpr
{
    /// <inheritdoc />
    public override IEnumerable<Expr> Children => [Operand];
}

// ---------------------------------------------------------------------------------------------------------------
// Operators
// ---------------------------------------------------------------------------------------------------------------

/// <summary>The comparison operators.</summary>
[PublicAPI]
public enum ComparisonOperator
{
    /// <summary>Equal.</summary>
    Equal,

    /// <summary>Not equal.</summary>
    NotEqual,

    /// <summary>Less than.</summary>
    LessThan,

    /// <summary>Less than or equal.</summary>
    LessThanOrEqual,

    /// <summary>Greater than.</summary>
    GreaterThan,

    /// <summary>Greater than or equal.</summary>
    GreaterThanOrEqual,
}

/// <summary>The logical operators.</summary>
[PublicAPI]
public enum LogicalOperator
{
    /// <summary>Logical AND.</summary>
    And,

    /// <summary>Logical OR.</summary>
    Or,
}

/// <summary>A comparison between two values.</summary>
/// <param name="Operator">The comparison.</param>
/// <param name="Left">The left operand.</param>
/// <param name="Right">The right operand.</param>
[PublicAPI]
public sealed record ComparisonExpr(ComparisonOperator Operator, Expr Left, Expr Right) : BoolExpr
{
    /// <inheritdoc />
    public override IEnumerable<Expr> Children => [Left, Right];
}

/// <summary>An AND or OR over two or more conditions. Nested nodes with the same operator are flattened.</summary>
/// <param name="Operator">The logical operator.</param>
/// <param name="Operands">The operands, left to right.</param>
[PublicAPI]
public sealed record LogicalExpr(LogicalOperator Operator, IReadOnlyList<BoolExpr> Operands) : BoolExpr
{
    /// <inheritdoc />
    public override IEnumerable<Expr> Children => Operands;

    /// <summary>
    ///     Combines conditions with <paramref name="operator" />, flattening operands that use the same operator.
    /// </summary>
    /// <param name="operator">The logical operator.</param>
    /// <param name="operands">The operands, left to right.</param>
    /// <returns>The single operand when only one is given, otherwise a <see cref="LogicalExpr" />.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="operands" /> is empty.</exception>
    public static BoolExpr Combine(LogicalOperator @operator, params IEnumerable<BoolExpr> operands)
    {
        var flattened = operands
            .SelectMany(x => x is LogicalExpr logical && logical.Operator == @operator
                ? logical.Operands
                : [x])
            .ToList();

        return flattened.Count switch
        {
            0 => throw new ArgumentException("At least one operand is required.", nameof(operands)),
            1 => flattened[0],
            _ => new LogicalExpr(@operator, flattened),
        };
    }
}

/// <summary>The logical negation of <paramref name="Operand" />.</summary>
/// <param name="Operand">The condition to negate.</param>
[PublicAPI]
public sealed record NotExpr(BoolExpr Operand) : BoolExpr
{
    /// <inheritdoc />
    public override IEnumerable<Expr> Children => [Operand];
}

// ---------------------------------------------------------------------------------------------------------------
// Functions
// ---------------------------------------------------------------------------------------------------------------

/// <summary>
///     Canonical names of the functions the core knows about. Printers map them to the platform's own names.
/// </summary>
[PublicAPI]
public static class FunctionNames
{
    /// <summary>Tests whether a string contains a substring.</summary>
    public const string Contains = "contains";

    /// <summary>Tests whether an array contains an element. Arguments: the array, then the element.</summary>
    public const string ContainsValue = "containsValue";

    /// <summary>Tests whether a string starts with a value.</summary>
    public const string StartsWith = "startsWith";

    /// <summary>Tests whether a string ends with a value.</summary>
    public const string EndsWith = "endsWith";

    /// <summary>Replaces <c>{n}</c> placeholders in a string.</summary>
    public const string Format = "format";

    /// <summary>Joins array elements into a string. Arguments: the array, then the optional separator.</summary>
    public const string Join = "join";

    /// <summary>Serializes a value to JSON.</summary>
    public const string ToJson = "toJSON";

    /// <summary>Parses JSON text.</summary>
    public const string FromJson = "fromJSON";

    /// <summary>Hashes the files matching a glob pattern.</summary>
    public const string HashFiles = "hashFiles";

    /// <summary>True when all previous work succeeded.</summary>
    public const string Success = "success";

    /// <summary>Always true, regardless of previous results.</summary>
    public const string Always = "always";

    /// <summary>True when any previous work failed.</summary>
    public const string Failure = "failure";

    /// <summary>True when the run was cancelled.</summary>
    public const string Cancelled = "cancelled";
}

/// <summary>A function call that returns a boolean.</summary>
/// <param name="Name">The canonical function name (see <see cref="FunctionNames" />).</param>
/// <param name="Arguments">The arguments.</param>
[PublicAPI]
public sealed record BoolFunction(string Name, params IReadOnlyList<Expr> Arguments) : BoolExpr
{
    /// <inheritdoc />
    public override IEnumerable<Expr> Children => Arguments;
}

/// <summary>A function call that returns a string.</summary>
/// <param name="Name">The canonical function name (see <see cref="FunctionNames" />).</param>
/// <param name="Arguments">The arguments.</param>
[PublicAPI]
public sealed record StringFunction(string Name, params IReadOnlyList<Expr> Arguments) : StringExpr
{
    /// <inheritdoc />
    public override IEnumerable<Expr> Children => Arguments;
}

/// <summary>A function call that returns an object, an array or a value of unknown type.</summary>
/// <param name="Name">The canonical function name (see <see cref="FunctionNames" />).</param>
/// <param name="Arguments">The arguments.</param>
[PublicAPI]
public sealed record JsonFunction(string Name, params IReadOnlyList<Expr> Arguments) : JsonExpr
{
    /// <inheritdoc />
    public override IEnumerable<Expr> Children => Arguments;
}

// ---------------------------------------------------------------------------------------------------------------
// Portable references
// ---------------------------------------------------------------------------------------------------------------

/// <summary>
///     A reference to a value every supported platform provides (a secret, variable, input, matrix value or output).
///     Each platform's printer decides the syntax.
/// </summary>
[PublicAPI]
public abstract record ReferenceExpr : StringExpr
{
    /// <inheritdoc />
    public override IEnumerable<Expr> Children => [];
}

/// <summary>A secret.</summary>
/// <param name="Name">The secret name.</param>
[PublicAPI]
public sealed record SecretRef(string Name) : ReferenceExpr;

/// <summary>A configuration variable defined outside the workflow.</summary>
/// <param name="Name">The variable name.</param>
[PublicAPI]
public sealed record VariableRef(string Name) : ReferenceExpr;

/// <summary>An input (parameter) supplied when the run was started.</summary>
/// <param name="Name">The input name.</param>
[PublicAPI]
public sealed record InputRef(string Name) : ReferenceExpr;

/// <summary>The current value of a matrix dimension.</summary>
/// <param name="Name">The dimension name.</param>
[PublicAPI]
public sealed record MatrixRef(string Name) : ReferenceExpr;

/// <summary>An output of another job that the current job depends on.</summary>
/// <param name="Job">The producing job's id.</param>
/// <param name="Output">The output name.</param>
/// <param name="Step">The id of the step in that job that set the output, for platforms that need it.</param>
[PublicAPI]
public sealed record JobOutputRef(string Job, string Output, string? Step = null) : ReferenceExpr;

/// <summary>An output of an earlier step in the current job.</summary>
/// <param name="Step">The step id.</param>
/// <param name="Output">The output name.</param>
[PublicAPI]
public sealed record StepOutputRef(string Step, string Output) : ReferenceExpr;

/// <summary>The result of another job that the current job depends on. Compare with <see cref="ResultLiteral" />.</summary>
/// <param name="Job">The job id.</param>
[PublicAPI]
public sealed record JobResultRef(string Job) : ReferenceExpr;

/// <summary>The possible results of a job or step.</summary>
[PublicAPI]
public enum RunResult
{
    /// <summary>Completed successfully.</summary>
    Success,

    /// <summary>Failed.</summary>
    Failure,

    /// <summary>Was cancelled.</summary>
    Cancelled,

    /// <summary>Was skipped.</summary>
    Skipped,
}

/// <summary>A result value, rendered with the platform's own spelling.</summary>
/// <param name="Result">The result.</param>
[PublicAPI]
public sealed record ResultLiteral(RunResult Result) : StringExpr
{
    /// <inheritdoc />
    public override IEnumerable<Expr> Children => [];
}
