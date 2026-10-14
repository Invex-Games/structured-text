namespace Invex.StructuredText.Expressions;

/// <summary>
///     Portable references to values every supported platform provides. Each platform renders them in its own syntax.
/// </summary>
[PublicAPI]
public static class Ref
{
    /// <summary>A secret.</summary>
    /// <param name="name">The secret name.</param>
    public static StringExpr Secret(string name) =>
        new SecretRef(name);

    /// <summary>A configuration variable defined outside the workflow.</summary>
    /// <param name="name">The variable name.</param>
    public static StringExpr Variable(string name) =>
        new VariableRef(name);

    /// <summary>An input (parameter) supplied when the run was started.</summary>
    /// <param name="name">The input name.</param>
    public static StringExpr Input(string name) =>
        new InputRef(name);

    /// <summary>The current value of a matrix dimension.</summary>
    /// <param name="name">The dimension name.</param>
    public static StringExpr Matrix(string name) =>
        new MatrixRef(name);

    /// <summary>An output of another job that the current job depends on.</summary>
    /// <param name="job">The producing job's id.</param>
    /// <param name="output">The output name.</param>
    /// <param name="step">The id of the step that set the output, for platforms that need it.</param>
    public static StringExpr JobOutput(string job, string output, string? step = null) =>
        new JobOutputRef(job, output, step);

    /// <summary>An output of an earlier step in the current job.</summary>
    /// <param name="step">The step id.</param>
    /// <param name="output">The output name.</param>
    public static StringExpr StepOutput(string step, string output) =>
        new StepOutputRef(step, output);

    /// <summary>The result of another job that the current job depends on.</summary>
    /// <param name="job">The job id.</param>
    public static StringExpr JobResult(string job) =>
        new JobResultRef(job);

    /// <summary>A result value to compare with <see cref="JobResult" />, spelled the platform's way.</summary>
    /// <param name="result">The result.</param>
    public static StringExpr Result(RunResult result) =>
        new ResultLiteral(result);
}

/// <summary>
///     Status checks for conditions, such as "only if everything so far succeeded".
/// </summary>
[PublicAPI]
public static class Status
{
    /// <summary>True when all previous work succeeded. Usually the platform default.</summary>
    public static BoolExpr Success() =>
        new BoolFunction(FunctionNames.Success);

    /// <summary>Always true, even after failures or cancellation.</summary>
    public static BoolExpr Always() =>
        new BoolFunction(FunctionNames.Always);

    /// <summary>True when any previous work failed.</summary>
    public static BoolExpr Failure() =>
        new BoolFunction(FunctionNames.Failure);

    /// <summary>True when the run was cancelled.</summary>
    public static BoolExpr Cancelled() =>
        new BoolFunction(FunctionNames.Cancelled);
}

/// <summary>
///     General-purpose expression functions.
/// </summary>
[PublicAPI]
public static class Fn
{
    /// <summary>True when every condition is true. Equivalent to chaining <c>&amp;&amp;</c>.</summary>
    /// <param name="conditions">One or more conditions.</param>
    public static BoolExpr All(params IEnumerable<BoolExpr> conditions) =>
        LogicalExpr.Combine(LogicalOperator.And, conditions);

    /// <summary>True when any condition is true. Equivalent to chaining <c>||</c>.</summary>
    /// <param name="conditions">One or more conditions.</param>
    public static BoolExpr Any(params IEnumerable<BoolExpr> conditions) =>
        LogicalExpr.Combine(LogicalOperator.Or, conditions);

    /// <summary>Replaces <c>{0}</c>, <c>{1}</c>, … in <paramref name="format" /> with <paramref name="arguments" />.</summary>
    /// <param name="format">The format string.</param>
    /// <param name="arguments">The replacement values.</param>
    public static StringExpr Format(StringExpr format, params IEnumerable<Expr> arguments) =>
        new StringFunction(FunctionNames.Format, [format, ..arguments]);

    /// <summary>Hashes the files matching the glob <paramref name="patterns" />.</summary>
    /// <param name="patterns">One or more glob patterns.</param>
    public static StringExpr HashFiles(params IEnumerable<StringExpr> patterns) =>
        new StringFunction(FunctionNames.HashFiles, [..patterns]);

    /// <summary>Parses JSON text.</summary>
    /// <param name="json">The JSON text.</param>
    public static JsonExpr FromJson(StringExpr json) =>
        new JsonFunction(FunctionNames.FromJson, json);

    /// <summary>The null literal.</summary>
    public static JsonExpr Null => new NullLiteral();
}
