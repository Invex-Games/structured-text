namespace Invex.StructuredText.Samples;

/// <summary>
///     Writing <c>if:</c> conditions with ordinary C# operators.
/// </summary>
public static class ConditionsSample
{
    public static string Run()
    {
        // Contexts are typed: EventName is a StringExpr, RefProtected is a BoolExpr.
        var isPush = GitHubContexts.EventName.Eq("push");
        var isPullRequest = GitHubContexts.EventName.Eq("pull_request");
        var onMain = GitHubContexts.Ref.Eq("refs/heads/main");
        var isTag = GitHubContexts.Ref.StartsWith("refs/tags/");
        var isDependabot = GitHubContexts.Actor.Eq("dependabot[bot]");

        return Render.Lines(
            // && and || build AND/OR nodes; parentheses are only added where precedence needs them.
            ("isPush && onMain", Render.Condition(isPush && onMain)),
            ("(isPush || isTag) && !bot", Render.Condition((isPush || isTag) && !isDependabot)),
            ("isPush || isTag && onMain", Render.Condition(isPush || isTag && onMain)),

            // A leading ! is a YAML tag, so the writer wraps those conditions in ${{ }} for you.
            ("!isDependabot", Render.Condition(!isDependabot)),

            // Plain strings become quoted literals, with quotes escaped.
            ("Ref.Eq(\"it's\")", Render.Condition(GitHubContexts.Ref.Eq("it's"))),

            // Numbers compare with Lt/Le/Gt/Ge; booleans are used directly.
            ("PR number > 100", Render.Condition(GitHubContexts.Event.PullRequest.Number.Gt(100))),
            ("isPullRequest && !draft", Render.Condition(isPullRequest && !GitHubContexts.Event.PullRequest.Draft)),

            // Status functions combine like any other condition.
            ("Always() && isPush", Render.Condition(Status.Always() && isPush)),
            ("Fn.Any(...)", Render.Condition(Fn.Any(isPush, isTag, isDependabot))));
    }
}
