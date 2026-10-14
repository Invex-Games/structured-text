namespace Invex.StructuredText.GithubActions.DependabotConfigModel.Model;

/// <summary>
///     Schedule day for Dependabot updates.
/// </summary>
[PublicAPI]
public enum ScheduleDay
{
    /// <summary>
    ///     Runs updates on Monday (<c>monday</c>).
    /// </summary>
    Monday,

    /// <summary>
    ///     Runs updates on Tuesday (<c>tuesday</c>).
    /// </summary>
    Tuesday,

    /// <summary>
    ///     Runs updates on Wednesday (<c>wednesday</c>).
    /// </summary>
    Wednesday,

    /// <summary>
    ///     Runs updates on Thursday (<c>thursday</c>).
    /// </summary>
    Thursday,

    /// <summary>
    ///     Runs updates on Friday (<c>friday</c>).
    /// </summary>
    Friday,

    /// <summary>
    ///     Runs updates on Saturday (<c>saturday</c>).
    /// </summary>
    Saturday,

    /// <summary>
    ///     Runs updates on Sunday (<c>sunday</c>).
    /// </summary>
    Sunday,
}
