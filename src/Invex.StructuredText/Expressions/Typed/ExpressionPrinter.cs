namespace Invex.StructuredText.Expressions;

/// <summary>
///     Renders typed expression trees (<see cref="Expr" />) and <see cref="Template" />s to platform syntax.
/// </summary>
/// <remarks>
///     <para>
///         The base class handles operator precedence (adding parentheses only where needed), literal quoting and
///         escaping, casts, raw text and context paths. Platforms override the members that differ: how operators
///         and functions are spelled, how <see cref="ReferenceExpr" /> nodes and <see cref="ResultLiteral" />s are
///         written, and how an embedded expression is delimited inside a <see cref="Template" />.
///     </para>
///     <para>Nodes the printer doesn't recognise cause a <see cref="NotSupportedException" />.</para>
/// </remarks>
[PublicAPI]
public abstract class ExpressionPrinter
{
    /// <summary>Precedence of OR.</summary>
    protected const int OrPrecedence = 1;

    /// <summary>Precedence of AND.</summary>
    protected const int AndPrecedence = 2;

    /// <summary>Precedence of comparisons.</summary>
    protected const int ComparisonPrecedence = 3;

    /// <summary>Precedence of unary NOT.</summary>
    protected const int NotPrecedence = 4;

    /// <summary>Precedence of atoms: literals, paths, calls and access.</summary>
    protected const int AtomPrecedence = 5;

    /// <summary>
    ///     Renders <paramref name="expression" /> as an expression (without template delimiters).
    /// </summary>
    /// <param name="expression">The expression.</param>
    /// <returns>The rendered text.</returns>
    public string Print(Expr expression) =>
        Print(expression, 0);

    /// <summary>
    ///     Renders <paramref name="template" />: literal text as-is, embedded expressions wrapped with
    ///     <see cref="WrapEmbedded" />.
    /// </summary>
    /// <param name="template">The template.</param>
    /// <returns>The rendered text.</returns>
    public string Render(Template template) =>
        string.Concat(template.Parts.Select(part => part.Expression is { } expression
            ? WrapEmbedded(Print(expression))
            : part.Text));

    /// <summary>
    ///     Renders <paramref name="template" />, or returns <see langword="null" /> when it is <see langword="null" />.
    /// </summary>
    /// <param name="template">The template.</param>
    /// <returns>The rendered text, or <see langword="null" />.</returns>
    [return: NotNullIfNotNull(nameof(template))]
    public string? RenderOrNull(Template? template) =>
        template is null
            ? null
            : Render(template);

    /// <summary>
    ///     Renders <paramref name="expression" />, or returns <see langword="null" /> when it is <see langword="null" />.
    /// </summary>
    /// <param name="expression">The expression.</param>
    /// <returns>The rendered text, or <see langword="null" />.</returns>
    [return: NotNullIfNotNull(nameof(expression))]
    public string? PrintOrNull(Expr? expression) =>
        expression is null
            ? null
            : Print(expression);

    /// <summary>
    ///     Wraps a rendered expression that is embedded in literal text, e.g. in expression delimiters.
    /// </summary>
    /// <param name="printedExpression">The rendered expression.</param>
    /// <returns>The wrapped text.</returns>
    protected abstract string WrapEmbedded(string printedExpression);

    /// <summary>
    ///     Renders a portable reference such as a secret or a job output.
    /// </summary>
    /// <param name="reference">The reference.</param>
    /// <returns>The rendered text.</returns>
    protected abstract string PrintReference(ReferenceExpr reference);

    /// <summary>
    ///     Renders a run result literal.
    /// </summary>
    /// <param name="result">The result.</param>
    /// <returns>The rendered text.</returns>
    protected abstract string PrintResult(RunResult result);

    /// <summary>
    ///     Maps a canonical function name (see <see cref="FunctionNames" />) to the platform's name.
    ///     Returns the name unchanged by default.
    /// </summary>
    /// <param name="name">The canonical name.</param>
    /// <returns>The platform's name.</returns>
    protected virtual string FunctionName(string name) =>
        name;

    /// <summary>
    ///     Quotes and escapes a string literal. Uses single quotes, doubling embedded single quotes, by default.
    /// </summary>
    /// <param name="value">The unquoted value.</param>
    /// <returns>The literal.</returns>
    protected virtual string QuoteString(string value) =>
        $"'{value.Replace("'", "''", StringComparison.Ordinal)}'";

    /// <summary>
    ///     Renders a comparison. Uses infix operators (<c>==</c>, <c>!=</c>, <c>&lt;</c>, …) by default.
    /// </summary>
    /// <param name="comparison">The comparison.</param>
    /// <returns>The rendered text and its precedence.</returns>
    protected virtual (string Text, int Precedence) PrintComparison(ComparisonExpr comparison)
    {
        var op = comparison.Operator switch
        {
            ComparisonOperator.Equal => "==",
            ComparisonOperator.NotEqual => "!=",
            ComparisonOperator.LessThan => "<",
            ComparisonOperator.LessThanOrEqual => "<=",
            ComparisonOperator.GreaterThan => ">",
            ComparisonOperator.GreaterThanOrEqual => ">=",
            _ => throw new ArgumentOutOfRangeException(nameof(comparison)),
        };

        return ($"{Print(comparison.Left, ComparisonPrecedence + 1)} {op} {Print(comparison.Right, ComparisonPrecedence + 1)}",
            ComparisonPrecedence);
    }

    /// <summary>
    ///     Renders an AND or OR. Uses infix <c>&amp;&amp;</c> and <c>||</c> by default.
    /// </summary>
    /// <param name="logical">The logical expression.</param>
    /// <returns>The rendered text and its precedence.</returns>
    protected virtual (string Text, int Precedence) PrintLogical(LogicalExpr logical)
    {
        var (op, precedence) = logical.Operator switch
        {
            LogicalOperator.And => (" && ", AndPrecedence),
            LogicalOperator.Or => (" || ", OrPrecedence),
            _ => throw new ArgumentOutOfRangeException(nameof(logical)),
        };

        return (string.Join(op, logical.Operands.Select(x => Print(x, precedence + 1))), precedence);
    }

    /// <summary>
    ///     Renders a negation. Uses prefix <c>!</c> by default.
    /// </summary>
    /// <param name="not">The negation.</param>
    /// <returns>The rendered text and its precedence.</returns>
    protected virtual (string Text, int Precedence) PrintNot(NotExpr not) =>
        ($"!{Print(not.Operand, NotPrecedence)}", NotPrecedence);

    /// <summary>
    ///     Renders a node the base class doesn't know, such as a platform-specific node.
    ///     Throws <see cref="NotSupportedException" /> by default.
    /// </summary>
    /// <param name="expression">The node.</param>
    /// <returns>The rendered text and its precedence.</returns>
    protected virtual (string Text, int Precedence) PrintOther(Expr expression) =>
        throw new NotSupportedException($"{GetType().Name} can't render {expression.GetType().Name}: {expression}");

    /// <summary>
    ///     Precedence given to raw text that isn't a plain dotted path. It is the lowest, so raw text is
    ///     parenthesized whenever it is nested inside an operator, and written verbatim when it stands alone.
    /// </summary>
    protected const int RawPrecedence = 0;

    /// <summary>
    ///     <see langword="true" /> when <paramref name="name" /> can be written with property dereference
    ///     (<c>.name</c>): letters, digits, <c>_</c> and <c>-</c>, not starting with a digit or <c>-</c>.
    /// </summary>
    /// <param name="name">The property name.</param>
    /// <returns>Whether dot syntax is valid for the name.</returns>
    public static bool IsDotAccessible(string name) =>
        name.Length > 0 &&
        (char.IsAsciiLetter(name[0]) || name[0] is '_') &&
        name.All(c => char.IsAsciiLetterOrDigit(c) || c is '_' or '-');

    /// <summary>
    ///     Renders a property access suffix: <c>.name</c> where valid, otherwise <c>['name']</c> with quotes escaped.
    /// </summary>
    /// <param name="name">The property name.</param>
    /// <returns>The suffix to append to the target.</returns>
    protected virtual string PrintMember(string name) =>
        IsDotAccessible(name)
            ? $".{name}"
            : $"[{QuoteString(name)}]";

    /// <summary>
    ///     Renders <paramref name="expression" />, adding parentheses when its precedence is below
    ///     <paramref name="minimumPrecedence" />.
    /// </summary>
    /// <param name="expression">The expression.</param>
    /// <param name="minimumPrecedence">The lowest precedence that may appear without parentheses.</param>
    /// <returns>The rendered text.</returns>
    protected string Print(Expr expression, int minimumPrecedence)
    {
        var (text, precedence) = PrintNode(expression);

        return precedence < minimumPrecedence
            ? $"({text})"
            : text;
    }

    private (string Text, int Precedence) PrintNode(Expr expression) =>
        expression switch
        {
            StringLiteral x => (QuoteString(x.Value), AtomPrecedence),
            NumberLiteral x => (x.Value.ToString(CultureInfo.InvariantCulture), AtomPrecedence),
            BoolLiteral x => (x.Value
                ? "true"
                : "false", AtomPrecedence),
            NullLiteral => ("null", AtomPrecedence),
            IRawExpr x => (x.Text, IsPlainPath(x.Text)
                ? AtomPrecedence
                : RawPrecedence),
            IContextPath x => (x.Path, AtomPrecedence),
            StringCast x => PrintNode(x.Operand),
            BoolCast x => PrintNode(x.Operand),
            NumberCast x => PrintNode(x.Operand),
            MemberAccessExpr x => ($"{Print(x.Target, AtomPrecedence)}{PrintMember(x.Property)}", AtomPrecedence),
            IndexAccessExpr x => ($"{Print(x.Target, AtomPrecedence)}[{Print(x.Index)}]", AtomPrecedence),
            ComparisonExpr x => PrintComparison(x),
            LogicalExpr x => PrintLogical(x),
            NotExpr x => PrintNot(x),
            BoolFunction x => (PrintCall(x.Name, x.Arguments), AtomPrecedence),
            StringFunction x => (PrintCall(x.Name, x.Arguments), AtomPrecedence),
            JsonFunction x => (PrintCall(x.Name, x.Arguments), AtomPrecedence),
            ReferenceExpr x => (PrintReference(x), AtomPrecedence),
            ResultLiteral x => (PrintResult(x.Result), AtomPrecedence),
            _ => PrintOther(expression),
        };

    // A dotted path such as github.event.forced is an atom; any other raw text might contain operators.
    private static bool IsPlainPath(string text) =>
        text.Length > 0 && text.Split('.').All(IsDotAccessible);

    private string PrintCall(string name, IReadOnlyList<Expr> arguments) =>
        $"{FunctionName(name)}({string.Join(", ", arguments.Select(x => Print(x)))})";
}

/// <summary>
///     A neutral printer used by <see cref="Expr.ToString" /> and <see cref="Template.ToString" />.
/// </summary>
internal sealed class DebugExpressionPrinter : ExpressionPrinter
{
    public static DebugExpressionPrinter Instance { get; } = new();

    protected override string WrapEmbedded(string printedExpression) =>
        $"{{{{ {printedExpression} }}}}";

    protected override string PrintReference(ReferenceExpr reference) =>
        reference switch
        {
            SecretRef x => $"secret({x.Name})",
            VariableRef x => $"variable({x.Name})",
            InputRef x => $"input({x.Name})",
            MatrixRef x => $"matrix({x.Name})",
            JobOutputRef x => $"jobOutput({x.Job}, {x.Output})",
            StepOutputRef x => $"stepOutput({x.Step}, {x.Output})",
            JobResultRef x => $"jobResult({x.Job})",
            _ => reference.GetType().Name,
        };

    protected override string PrintResult(RunResult result) =>
        result.ToString();

    protected override (string Text, int Precedence) PrintOther(Expr expression) =>
        (expression.GetType().Name, AtomPrecedence);
}
