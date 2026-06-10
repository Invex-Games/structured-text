namespace Invex.StructuredText.AzureDevopsPipelines;

/// <summary>
///     Wraps a variable name to be expanded using Azure DevOps macro syntax: $(variableName)
/// </summary>
/// <remarks>
///     Macro expressions are expanded at runtime before a task executes. Use this for pipeline and
///     predefined variables, e.g. <c>new DevopsMacroExpression(new RawExpression("Build.BuildId"))</c>
///     renders as <c>$(Build.BuildId)</c>.
/// </remarks>
/// <param name="Variable">The variable name to expand.</param>
[PublicAPI]
[SuppressMessage("Design", "CA1067:Override Object.Equals(object) when implementing IEquatable<T>")]
public sealed record DevopsMacroExpression(TextExpression Variable) : TextExpression;
