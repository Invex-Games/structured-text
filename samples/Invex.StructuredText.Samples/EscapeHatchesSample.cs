namespace Invex.StructuredText.Samples;

/// <summary>
///     Functions and escape hatches for things the catalogue doesn't cover.
/// </summary>
public static class EscapeHatchesSample
{
    public static string Run()
    {
        // Any property not in the catalogue is reachable with an indexer, then typed with AsString/AsBool/AsNumber.
        var label = Tex.GitHub.Event["label"]["name"].AsString();

        // Functions.
        var cacheKey = Tex.Format("{0}-nuget-{1}", Tex.GitHub.Runner.Os, Tex.HashFiles("**/*.csproj"));
        var labels = Tex.FromJson(Tex.Variable("RELEASE_LABELS"));

        return Render.Lines(
            ("indexer + AsString", Render.Condition(label.Eq("release"))),
            ("Tex.Format + HashFiles", Render.Value(cacheKey)),
            ("value In array", Render.Condition(label.In(labels))),
            ("ToJson", Render.Value(Tex.GitHub.Event.ToJson())),

            // Raw text is written exactly as given. Use it only when nothing typed exists.
            ("BoolExpr.Raw", Render.Condition(Tex.Raw("github.event.forced") && Tex.Success())));
    }
}
