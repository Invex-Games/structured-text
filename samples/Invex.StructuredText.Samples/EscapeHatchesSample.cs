namespace Invex.StructuredText.Samples;

/// <summary>
///     Functions and escape hatches for things the catalogue doesn't cover.
/// </summary>
public static class EscapeHatchesSample
{
    public static string Run()
    {
        // Any property not in the catalogue is reachable with an indexer, then typed with AsString/AsBool/AsNumber.
        var label = GitHubContexts.Event["label"]["name"].AsString();

        // Functions.
        var cacheKey = Fn.Format("{0}-nuget-{1}", GitHubContexts.Runner.Os, Fn.HashFiles("**/*.csproj"));
        var labels = Fn.FromJson(Ref.Variable("RELEASE_LABELS"));

        return Render.Lines(
            ("indexer + AsString", Render.Condition(label.Eq("release"))),
            ("Fn.Format + HashFiles", Render.Value(cacheKey)),
            ("value In array", Render.Condition(label.In(labels))),
            ("ToJson", Render.Value(GitHubContexts.Event.ToJson())),

            // Raw text is written exactly as given. Use it only when nothing typed exists.
            ("BoolExpr.Raw", Render.Condition(BoolExpr.Raw("github.event.forced") && Status.Success())));
    }
}
