namespace Invex.StructuredText.Samples;

/// <summary>
///     Portable references to secrets, variables, inputs, matrix values and other jobs.
/// </summary>
public static class ReferencesSample
{
    public static string Run() =>
        Render.Lines(
            ("Tex.Secret", Render.Value(Tex.Secret("NUGET_API_KEY"))),
            ("Tex.Variable", Render.Value(Tex.Variable("DEPLOY_REGION"))),
            ("Tex.Input", Render.Value(Tex.Input("dry-run"))),
            ("Tex.Matrix", Render.Value(Tex.Matrix("os"))),
            ("Tex.StepOutput", Render.Value(Tex.StepOutput("version", "semver"))),
            ("Tex.JobOutput", Render.Value(Tex.JobOutput("setup", "build-version"))),

            // Job results compare against a RunResult, spelled the platform's way.
            ("JobResult == Success",
                Render.Condition(Tex.JobResult("test").Eq(Tex.Result(RunResult.Success)))),

            // References are strings, so string functions apply.
            ("prerelease version",
                Render.Condition(Tex.JobOutput("setup", "build-version").Contains("-"))));
}
