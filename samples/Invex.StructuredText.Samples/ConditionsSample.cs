namespace Invex.StructuredText.Samples;

/// <summary>
///     Writing <c>if:</c> conditions with ordinary C# operators.
/// </summary>
public static class ConditionsSample
{
    public static string Run()
    {
        // Contexts are typed: EventName is a StringExpr, RefProtected is a BoolExpr.
        var isPush = Tex.GitHub.EventName.Eq("push");
        var isPullRequest = Tex.GitHub.EventName.Eq("pull_request");
        var onMain = Tex.GitHub.Ref.Eq("refs/heads/main");
        var isTag = Tex.GitHub.Ref.StartsWith("refs/tags/");
        var isDependabot = Tex.GitHub.Actor.Eq("dependabot[bot]");

        return Render.Lines(
            // && and || build AND/OR nodes; parentheses are only added where precedence needs them.
            ("isPush && onMain", Render.Condition(isPush && onMain)),
            ("(isPush || isTag) && !bot", Render.Condition((isPush || isTag) && !isDependabot)),
            ("isPush || isTag && onMain", Render.Condition(isPush || isTag && onMain)),

            // A leading ! is a YAML tag, so the writer wraps those conditions in ${{ }} for you.
            ("!isDependabot", Render.Condition(!isDependabot)),

            // Plain strings become quoted literals, with quotes escaped.
            ("GitHub.Ref.Eq(\"it's\")", Render.Condition(Tex.GitHub.Ref.Eq("it's"))),

            // Numbers compare with Lt/Le/Gt/Ge; booleans are used directly.
            ("PR number > 100", Render.Condition(Tex.GitHub.Event.PullRequest.Number.Gt(100))),
            ("isPullRequest && !draft", Render.Condition(isPullRequest && !Tex.GitHub.Event.PullRequest.Draft)),

            // Status functions combine like any other condition.
            ("Always() && isPush", Render.Condition(Tex.Always() && isPush)),
            ("Tex.Any(...)", Render.Condition(Tex.Any(isPush, isTag, isDependabot))));
    }
}
