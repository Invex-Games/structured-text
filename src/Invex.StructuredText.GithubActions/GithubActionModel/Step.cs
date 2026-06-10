namespace Invex.StructuredText.GithubActions.GithubActionModel;

/// <summary>
///     A single step within a job. This is a discriminated union:
///     construct a <see cref="UsesStep" /> to run an action or a <see cref="RunStep" /> to run shell commands.
///     The properties on this base record are shared by both variants.
/// </summary>
[PublicAPI]
[Union]
public partial record Step
{
    /// <summary>
    ///     A unique identifier for the step (<c>id</c>), used to reference its outputs and outcome
    ///     via <see cref="StepOutputExpression" /> / <see cref="StepOutcomeExpression" />.
    /// </summary>
    public string? Id { get; init; }

    /// <summary>
    ///     A condition that must evaluate to true for the step to run (<c>if</c>).
    ///     Written in expression context, so no <c>Evaluate()</c> wrapper is needed.
    /// </summary>
    public TextExpression? If { get; init; }

    /// <summary>
    ///     The display name of the step (<c>name</c>).
    /// </summary>
    public TextExpression? Name { get; init; }

    /// <summary>
    ///     The working directory the step runs in (<c>working-directory</c>).
    /// </summary>
    public TextExpression? WorkingDirectory { get; init; }

    /// <summary>
    ///     Input parameters for the step (<c>with</c>), keyed by input name.
    ///     Multi-element collections are written as multi-line values.
    /// </summary>
    public IReadOnlyDictionary<string, TextExpressionCollection>? With { get; init; }

    /// <summary>
    ///     Environment variables available to the step (<c>env</c>).
    /// </summary>
    public IReadOnlyDictionary<string, TextExpression>? Env { get; init; }

    /// <summary>
    ///     Whether the job passes when this step fails (<c>continue-on-error</c>).
    /// </summary>
    public TextExpression? ContinueOnError { get; init; }

    /// <summary>
    ///     The maximum number of minutes the step may run before being terminated (<c>timeout-minutes</c>).
    /// </summary>
    public TextExpression? TimeoutMinutes { get; init; }

    /// <summary>
    ///     A step that runs a reusable action (<c>uses</c>), e.g. <c>actions/checkout@v4</c>.
    ///     Provide inputs via <see cref="Step.With" />.
    /// </summary>
    [PublicAPI]
    public partial record UsesStep
    {
        /// <summary>
        ///     The action reference to run, e.g. <c>actions/setup-dotnet@v4</c> or a local path.
        /// </summary>
        public required TextExpression Uses { get; init; }
    }

    /// <summary>
    ///     A step that runs shell commands (<c>run</c>).
    /// </summary>
    [PublicAPI]
    public partial record RunStep
    {
        /// <summary>
        ///     The command lines to run. A single entry produces <c>run: command</c>;
        ///     multiple entries produce a multi-line <c>run: |</c> block.
        /// </summary>
        public required TextExpressionCollection Run { get; init; }

        /// <summary>
        ///     The shell to run the commands with (<c>shell</c>), e.g. <c>bash</c> or <c>pwsh</c>.
        /// </summary>
        public TextExpression? Shell { get; init; }
    }
}
