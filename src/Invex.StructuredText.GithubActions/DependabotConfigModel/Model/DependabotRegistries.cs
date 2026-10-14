namespace Invex.StructuredText.GithubActions.DependabotConfigModel.Model;

/// <summary>
///     Registries to use for this update configuration. Can be a list of registry names or "*" for all.
/// </summary>
[PublicAPI]
[Union]
public partial record DependabotRegistries
{
    /// <summary>
    ///     Uses every registry defined in <see cref="DependabotConfig.Registries" />, written as <c>registries:
    ///     "*"</c>.
    /// </summary>
    public partial record All;

    /// <summary>
    ///     Uses only the named registries, written as a <c>registries</c> array.
    /// </summary>
    /// <param name="Names">The names of registries defined in <see cref="DependabotConfig.Registries" />.</param>
    public partial record Named(params IReadOnlyList<string> Names);
}
