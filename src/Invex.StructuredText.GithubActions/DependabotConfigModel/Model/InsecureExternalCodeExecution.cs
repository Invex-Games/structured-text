namespace Invex.StructuredText.GithubActions.DependabotConfigModel.Model;

/// <summary>
///     Allow or deny code execution in manifest files.
/// </summary>
[PublicAPI]
public enum InsecureExternalCodeExecution
{
    /// <summary>
    ///     Allows Dependabot to execute code in manifest files during updates (<c>allow</c>).
    /// </summary>
    Allow,

    /// <summary>
    ///     Prevents Dependabot from executing code in manifest files during updates (<c>deny</c>).
    /// </summary>
    Deny,
}
