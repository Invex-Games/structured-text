namespace Invex.StructuredText.Samples;

/// <summary>
///     A complete build, test and publish workflow.
/// </summary>
public static class CiWorkflowSample
{
    public static string Run()
    {
        var version = Tex.JobOutput("setup", "version");
        var isRelease = !version.Contains("-");

        return Render.Workflow(new()
        {
            Name = "Build",
            On =
            [
                new On.Push
                {
                    Branches = ["main"],
                    BranchesIgnore = null,
                    Tags = null,
                    TagsIgnore = null,
                    Paths = null,
                    PathsIgnore = null,
                },
                new On.PullRequest([])
                {
                    Branches = ["main"],
                    BranchesIgnore = null,
                    Tags = null,
                    TagsIgnore = null,
                    Paths = null,
                    PathsIgnore = null,
                },
            ],
            Permissions = new Permissions.All(PermissionsLevel.Read),
            Concurrency = new()
            {
                Group = $"{Tex.GitHub.Workflow}-{Tex.GitHub.Ref}",
                CancelInProgress = Tex.GitHub.EventName.Eq("pull_request"),
            },
            Jobs =
            [
                new()
                {
                    Name = "setup",
                    RunsOn = new() { Labels = ["ubuntu-latest"] },
                    Outputs = new Dictionary<string, Template>
                    {
                        ["version"] = Tex.StepOutput("version", "semver"),
                    },
                    Steps =
                    [
                        new Step.UsesStep
                        {
                            Name = "Checkout",
                            Uses = "actions/checkout@v6",
                            // GitVersion needs the full history.
                            With = new Dictionary<string, Template> { ["fetch-depth"] = 0 },
                        },
                        new Step.RunStep
                        {
                            Id = "version",
                            Name = "Compute version",
                            // Assign first so a GitVersion failure fails the step (bash runs with -e).
                            Run =
                            [
                                "dotnet tool install --global GitVersion.Tool",
                                "semver=$(dotnet-gitversion /showvariable SemVer)",
                                "echo \"semver=$semver\" >> \"$GITHUB_OUTPUT\"",
                            ],
                        },
                    ],
                },
                new()
                {
                    Name = "test",
                    Needs = ["setup"],
                    RunsOn = new() { Labels = [Tex.Matrix("os")] },
                    Strategy = new()
                    {
                        Matrix = new()
                        {
                            Map = new Dictionary<string, IReadOnlyList<Template>>
                            {
                                ["os"] = ["ubuntu-latest", "windows-latest", "macos-latest"],
                            },
                        },
                    },
                    Steps =
                    [
                        new Step.UsesStep { Name = "Checkout", Uses = "actions/checkout@v6" },
                        new Step.RunStep
                        {
                            Name = $"Test on {Tex.Matrix("os")}",
                            Run = [$"dotnet test -p:Version={version}"],
                        },
                    ],
                },
                new()
                {
                    Name = "publish",
                    Needs = ["setup", "test"],
                    // Publish stable versions only, and never from a pull request.
                    If = isRelease && Tex.GitHub.EventName.Ne("pull_request"),
                    RunsOn = new() { Labels = ["ubuntu-latest"] },
                    Steps =
                    [
                        new Step.UsesStep { Name = "Checkout", Uses = "actions/checkout@v6" },
                        new Step.RunStep
                        {
                            Name = $"Publish {version}",
                            Env = new Dictionary<string, Template>
                            {
                                ["NUGET_API_KEY"] = Tex.Secret("NUGET_API_KEY"),
                            },
                            Run =
                            [
                                $"dotnet pack -c Release -p:Version={version}",
                                "dotnet nuget push \"**/*.nupkg\" --api-key \"$NUGET_API_KEY\" --source nuget.org",
                            ],
                        },
                    ],
                },
                new()
                {
                    Name = "notify",
                    Needs = ["test"],
                    // Runs even when tests fail, but only reports failures.
                    If = Tex.Always() && Tex.JobResult("test").Eq(Tex.Result(RunResult.Failure)),
                    RunsOn = new() { Labels = ["ubuntu-latest"] },
                    Steps =
                    [
                        new Step.RunStep
                        {
                            Name = "Report failure",
                            Run = [$"echo \"Tests failed on {Tex.GitHub.Ref} ({Tex.GitHub.Sha})\""],
                        },
                    ],
                },
            ],
        });
    }
}
