namespace Invex.StructuredText.GithubActions.GithubActionModel;

/// <summary>
///     The runner selection for a job (<c>runs-on</c>).
/// </summary>
[PublicAPI]
public sealed record RunsOn
{
    /// <summary>
    ///     The runner labels the job requires, e.g. <c>ubuntu-latest</c> or self-hosted runner labels.
    ///     A single label is written inline; multiple labels are written as a list.
    /// </summary>
    public required TextExpressionCollection Labels { get; init; }

    /// <summary>
    ///     The runner group to select runners from (<c>group</c>), for organization-level runner groups.
    /// </summary>
    public TextExpression? Group { get; init; }
}
