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
        Template group = $"{Tex.GitHub.Workflow}-{Tex.GitHub.Ref}";
        Template artifactPath = $"{Tex.GitHub.Workspace}/artifacts/{Tex.Matrix("os")}";

        // Plain C# values in the holes are just text.
        var version = 10;
        Template setup = $"Setup .NET {version}.0";

        // Join several templates with a separator.
        var key = Tex.Join("-", [Tex.Matrix("os"), Tex.Matrix("framework"), "release"]);

        // A single expression on its own is a template too.
        Template token = Tex.Secret("NUGET_API_KEY");

        return Render.Lines(
            ("concurrency group", Render.Value(group)),
            ("artifact path", Render.Value(artifactPath)),
            ("step name", Render.Value(setup)),
            ("cache key", Render.Value(key)),
            ("env value", Render.Value(token)));
    }
}
