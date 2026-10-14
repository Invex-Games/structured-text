namespace Invex.StructuredText.GithubActions.DependabotConfigModel.Model;

/// <summary>
///     Group configuration for dependencies.
/// </summary>
[PublicAPI]
[Union]
public partial record DependabotGroup
{
    /// <summary>
    ///     Use to specify whether the rules in the group apply to version updates or security updates.
    /// </summary>
    public GroupAppliesTo? AppliesTo { get; init; }

    /// <summary>
    ///     Specify a dependency type to be included in the group.
    /// </summary>
    public virtual GroupDependencyType? DependencyType { get; init; }

    /// <summary>
    ///     Define strings of characters that match with a dependency name to include those dependencies in the group.
    /// </summary>
    public virtual IReadOnlyList<string> Patterns { get; init; } = [];

    /// <summary>
    ///     Exclude certain dependencies from the group.
    /// </summary>
    public virtual IReadOnlyList<string> ExcludePatterns { get; init; } = [];

    /// <summary>
    ///     Specify the semantic versioning level to include in the group.
    /// </summary>
    public virtual IReadOnlyList<GroupUpdateType> UpdateTypes { get; init; } = [];

    /// <summary>
    ///     Configure how dependencies are grouped within this group.
    /// </summary>
    public virtual GroupBy? GroupBy { get; init; }

    /// <summary>
    ///     A group that selects dependencies by <see cref="DependabotGroup.DependencyType" />.
    /// </summary>
    public sealed partial record FromType
    {
        /// <summary>
        ///     Gets the dependency type to include in the group (<c>dependency-type</c>).
        /// </summary>
        public override required GroupDependencyType? DependencyType { get; init; }
    }

    /// <summary>
    ///     A group that selects dependencies whose names match <see cref="DependabotGroup.Patterns" />.
    /// </summary>
    public sealed partial record FromPatterns
    {
        /// <summary>
        ///     Gets the dependency name patterns to include in the group (<c>patterns</c>); <c>*</c> matches zero or
        ///     more characters.
        /// </summary>
        public override required IReadOnlyList<string> Patterns { get; init; }
    }

    /// <summary>
    ///     A group defined by the dependencies it excludes, via <see cref="DependabotGroup.ExcludePatterns" />.
    /// </summary>
    public sealed partial record FromExcludePatterns
    {
        /// <summary>
        ///     Gets the dependency name patterns to exclude from the group (<c>exclude-patterns</c>).
        /// </summary>
        public override required IReadOnlyList<string> ExcludePatterns { get; init; }
    }

    /// <summary>
    ///     A group that selects updates by semantic versioning level, via <see cref="DependabotGroup.UpdateTypes" />.
    /// </summary>
    public sealed partial record FromUpdateTypes
    {
        /// <summary>
        ///     Gets the semantic versioning levels to include in the group (<c>update-types</c>).
        /// </summary>
        public override required IReadOnlyList<GroupUpdateType> UpdateTypes { get; init; }
    }

    /// <summary>
    ///     A group whose updates are grouped by <see cref="DependabotGroup.GroupBy" />.
    /// </summary>
    public sealed partial record FromGroupBy
    {
        /// <summary>
        ///     Gets how dependencies are grouped within the group (<c>group-by</c>).
        /// </summary>
        public override required GroupBy? GroupBy { get; init; }
    }
}

/// <summary>
///     Specifies whether the rules in the group apply to version updates or security updates.
/// </summary>
[PublicAPI]
public enum GroupAppliesTo
{
    /// <summary>
    ///     The group's rules apply to version updates (<c>version-updates</c>).
    /// </summary>
    VersionUpdates,

    /// <summary>
    ///     The group's rules apply to security updates (<c>security-updates</c>).
    /// </summary>
    SecurityUpdates,
}

/// <summary>
///     Dependency type for groups.
/// </summary>
[PublicAPI]
public enum GroupDependencyType
{
    /// <summary>
    ///     Includes only development dependencies in the group (<c>development</c>).
    /// </summary>
    Development,

    /// <summary>
    ///     Includes only production dependencies in the group (<c>production</c>).
    /// </summary>
    Production,
}

/// <summary>
///     Semantic versioning update type.
/// </summary>
[PublicAPI]
public enum GroupUpdateType
{
    /// <summary>
    ///     Includes updates that change the major version (<c>major</c>).
    /// </summary>
    Major,

    /// <summary>
    ///     Includes updates that change the minor version (<c>minor</c>).
    /// </summary>
    Minor,

    /// <summary>
    ///     Includes updates that change only the patch version (<c>patch</c>).
    /// </summary>
    Patch,
}

/// <summary>
///     Configure how dependencies are grouped.
/// </summary>
[PublicAPI]
public enum GroupBy
{
    /// <summary>
    ///     Groups updates for the same dependency across directories into one pull request (<c>dependency-name</c>).
    /// </summary>
    DependencyName,
}
