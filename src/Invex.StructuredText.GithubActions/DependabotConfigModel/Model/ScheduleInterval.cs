namespace Invex.StructuredText.GithubActions.DependabotConfigModel.Model;

/// <summary>
///     Schedule interval for Dependabot updates.
/// </summary>
[PublicAPI]
public enum ScheduleInterval
{
    /// <summary>
    ///     Checks for updates every weekday, Monday to Friday (<c>daily</c>).
    /// </summary>
    Daily,

    /// <summary>
    ///     Checks for updates once a week, on Monday unless <see cref="DependabotSchedule.Day" /> is set
    ///     (<c>weekly</c>).
    /// </summary>
    Weekly,

    /// <summary>
    ///     Checks for updates on the first day of each month (<c>monthly</c>).
    /// </summary>
    Monthly,

    /// <summary>
    ///     Checks for updates on the first day of January, April, July and October (<c>quarterly</c>).
    /// </summary>
    Quarterly,

    /// <summary>
    ///     Checks for updates every six months, on the first day of January and July (<c>semiannually</c>).
    /// </summary>
    Semiannually,

    /// <summary>
    ///     Checks for updates on the first day of January each year (<c>yearly</c>).
    /// </summary>
    Yearly,

    /// <summary>
    ///     Checks for updates on the cron expression in <see cref="DependabotSchedule.Cronjob" /> (<c>cron</c>).
    /// </summary>
    Cron,
}
