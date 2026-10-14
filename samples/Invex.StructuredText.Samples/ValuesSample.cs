namespace Invex.StructuredText.Samples;

/// <summary>
///     Building YAML values (names, env vars, inputs) with interpolated strings.
/// </summary>
public static class ValuesSample
{
    public static string Run()
    {
        // A Template is literal text with embedded expressions. Interpolation builds one directly,
        // and each expression is wrapped in ${{ }} when written.
        Template group = $"{GitHubContexts.Workflow}-{GitHubContexts.Ref}";
        Template artifactPath = $"{GitHubContexts.Workspace}/artifacts/{Ref.Matrix("os")}";

        // Plain C# values in the holes are just text.
        var version = 10;
        Template setup = $"Setup .NET {version}.0";

        // Join several templates with a separator.
        var key = Template.Join("-", [Ref.Matrix("os"), Ref.Matrix("framework"), "release"]);

        // A single expression on its own is a template too.
        Template token = Ref.Secret("NUGET_API_KEY");

        return Render.Lines(
            ("concurrency group", Render.Value(group)),
            ("artifact path", Render.Value(artifactPath)),
            ("step name", Render.Value(setup)),
            ("cache key", Render.Value(key)),
            ("env value", Render.Value(token)));
    }
}
