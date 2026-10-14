namespace Invex.StructuredText.Samples;

/// <summary>
///     Portable references to secrets, variables, inputs, matrix values and other jobs.
/// </summary>
public static class ReferencesSample
{
    public static string Run() =>
        Render.Lines(
            ("Ref.Secret", Render.Value(Ref.Secret("NUGET_API_KEY"))),
            ("Ref.Variable", Render.Value(Ref.Variable("DEPLOY_REGION"))),
            ("Ref.Input", Render.Value(Ref.Input("dry-run"))),
            ("Ref.Matrix", Render.Value(Ref.Matrix("os"))),
            ("Ref.StepOutput", Render.Value(Ref.StepOutput("version", "semver"))),
            ("Ref.JobOutput", Render.Value(Ref.JobOutput("setup", "build-version"))),

            // Job results compare against a RunResult, spelled the platform's way.
            ("JobResult == Success",
                Render.Condition(Ref.JobResult("test").Eq(Ref.Result(RunResult.Success)))),

            // References are strings, so string functions apply.
            ("prerelease version",
                Render.Condition(Ref.JobOutput("setup", "build-version").Contains("-"))));
}
