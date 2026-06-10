# GitHub Actions Workflows

The `Invex.StructuredText.GithubActions` package generates GitHub Actions workflow YAML from a strongly-typed model
rooted at `GithubAction` (namespace `Invex.StructuredText.GithubActions.GithubActionModel`).

```csharp
var writer = new GithubActionWriter();
writer.Write(workflow);
var yaml = writer.TextWriter.ToString();
```

## The model at a glance

```text
GithubAction
├── Name, RunName
├── On            (list of trigger events — discriminated union)
├── Permissions   (All | Exact)
├── Env           (dictionary of expressions)
├── Concurrency
└── Jobs
    ├── Name, Needs, If, RunsOn, Environment, Concurrency
    ├── Permissions, Outputs, Env, TimeoutMinutes, ContinueOnError
    ├── Strategy (Matrix), Container, Services, Snapshot
    └── Steps     (UsesStep | RunStep)
```

Many model types are **discriminated unions** (generated with [Dunet](https://github.com/domn1995/dunet)): you pick a
variant by constructing the appropriate nested record, e.g. `new On.Push { ... }` or `new Step.RunStep { ... }`.

## Triggers (`On`)

`On` is a union over all GitHub webhook events. Common variants:

```csharp
On =
[
    // Push to branches/tags/paths
    new On.Push
    {
        Branches = ["main"],
        BranchesIgnore = null,
        Tags = ["v*"],
        TagsIgnore = null,
        Paths = null,
        PathsIgnore = null,
    },

    // Pull requests
    new On.PullRequest([On.PullRequest.PullRequestType.opened, On.PullRequest.PullRequestType.synchronized])
    {
        Branches = ["main"],
        BranchesIgnore = null,
        Tags = null,
        TagsIgnore = null,
        Paths = null,
        PathsIgnore = null,
    },

    // Cron schedules
    new On.Schedule(["0 6 * * 1"]),

    // Manual dispatch with typed inputs
    new On.WorkflowDispatch(
    [
        new WorkflowDispatchInput.Boolean
        {
            Name = "dry-run",
            Description = "Skip publishing",
            Required = false,
            Default = "false",
        },
        new WorkflowDispatchInput.Choice
        {
            Name = "configuration",
            Description = null,
            Required = true,
            Default = "Release",
            Options = ["Debug", "Release"],
        },
    ]),

    // Reusable workflows / external triggers
    new On.WorkflowCall(),
    new On.RepositoryDispatch(["deploy"]),
]
```

Other supported events include `Release`, `IssueComment`, `Issues`, `Label`, `MergeGroup`, `Discussion`, `CheckRun`,
`CheckSuite`, `WorkflowRun`, `Watch`, `Create`, `Delete`, `Fork`, `PageBuild`, `Public`, `Status`, and more — each with
its schema-accurate activity-type enum.

## Jobs

```csharp
new Job
{
    Name = new RawExpression("test"),
    RunsOn = new() { Labels = [new RawExpression("ubuntu-latest")] },
    Needs = ["build"],                                   // job dependencies
    If = new RawExpression("github.event_name").EqualToString("push"),
    TimeoutMinutes = 30,
    Env = new Dictionary<string, TextExpression>
    {
        ["DOTNET_NOLOGO"] = "true",
    },
    Outputs = new Dictionary<string, TextExpression>
    {
        ["version"] = new StepOutputExpression
        {
            StepName = "gitversion",
            OutputName = "semVer",
        }.Evaluate(),
    },
    Steps = [ /* ... */ ],
}
```

### Matrix strategies

```csharp
Strategy = new Strategy
{
    Matrix = new Matrix
    {
        Map = new Dictionary<string, TextExpressionCollection>
        {
            ["os"] = new[] { "ubuntu-latest", "windows-latest", "macos-latest" },
            ["dotnet"] = new[] { "8.0.x", "9.0.x" },
        },
        Include = null,
        Exclude =
        [
            new Dictionary<string, TextExpression>
            {
                ["os"] = "macos-latest",
                ["dotnet"] = "8.0.x",
            },
        ],
    },
    FailFast = false,
    MaxParallel = 4,
},
RunsOn = new() { Labels = [new RawExpression("matrix.os").Evaluate()] },
```

### Permissions

```csharp
// Top-level or per-job: the same level for every scope (read-all / write-all)
Permissions = new Permissions.All(PermissionsLevel.Read),

// Or exact, per-scope
Permissions = new Permissions.Exact(new PermissionsEvent
{
    Contents = PermissionsLevel.Read,
    Packages = PermissionsLevel.Write,
}),
```

### Concurrency, environments, containers

```csharp
Concurrency = new Concurrency
{
    Group = TextExpressions.Format($"ci-{new RawExpression("github.ref")}"),
    CancelInProgress = true,
},

Environment = new Environment
{
    Name = new RawExpression("production"),
},

Container = new Container
{
    Image = new RawExpression("mcr.microsoft.com/dotnet/sdk:10.0"),
},
Services = new Dictionary<string, Container>
{
    ["postgres"] = new() { Image = new RawExpression("postgres:16") },
},
```

## Steps

`Step` is a union of two variants. Shared properties (`Id`, `Name`, `If`, `Env`, `With`, `WorkingDirectory`,
`ContinueOnError`, `TimeoutMinutes`) live on the base record.

### `UsesStep` — run an action

```csharp
new Step.UsesStep
{
    Name = new RawExpression("Setup .NET"),
    Uses = new RawExpression("actions/setup-dotnet@v4"),
    With = new Dictionary<string, TextExpressionCollection>
    {
        ["dotnet-version"] = new[] { "8.0.x", "9.0.x" },
    },
}
```

### `RunStep` — run shell commands

`Run` is a `TextExpressionCollection`; multiple entries produce a multi-line `run: |` block.

```csharp
new Step.RunStep
{
    Id = "pack",
    Name = new RawExpression("Pack"),
    Run =
    [
        "dotnet pack --configuration Release",
        "dotnet nuget push *.nupkg",
    ],
    Shell = new RawExpression("bash"),
    Env = new Dictionary<string, TextExpression>
    {
        ["NUGET_API_KEY"] = new RawExpression("secrets.NUGET_API_KEY").Evaluate(),
    },
}
```

### Conditional steps

```csharp
new Step.RunStep
{
    Name = new RawExpression("Publish (main only)"),
    If = new RawExpression("github.ref").EqualToString("refs/heads/main")
        & new RawExpression("success()"),
    Run = ["dotnet nuget push *.nupkg"],
}
```

`If` is rendered inside expression context, producing
`if: github.ref == 'refs/heads/main' && success()`.

## Referencing outputs between steps and jobs

```csharp
// In job "release", consume the version produced by job "build":
var version = new TargetOutputExpression
{
    TargetName = "build",
    OutputName = "version",
};

new Job
{
    Name = new RawExpression("release"),
    Needs = ["build"],
    RunsOn = new() { Labels = [new RawExpression("ubuntu-latest")] },
    Env = new Dictionary<string, TextExpression>
    {
        ["VERSION"] = version.Evaluate(),   // ${{ needs.build.outputs.version }}
    },
    Steps = [ /* ... */ ],
}
```

## Complete example

```csharp
var workflow = new GithubAction
{
    Name = "CI",
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
    ],
    Permissions = new Permissions.Exact(new PermissionsEvent
    {
        Contents = PermissionsLevel.Read,
    }),
    Concurrency = new Concurrency
    {
        Group = TextExpressions.Format($"ci-{new RawExpression("github.ref")}"),
        CancelInProgress = true,
    },
    Jobs =
    [
        new Job
        {
            Name = new RawExpression("build"),
            RunsOn = new() { Labels = [new RawExpression("ubuntu-latest")] },
            Steps =
            [
                new Step.UsesStep { Uses = new RawExpression("actions/checkout@v4") },
                new Step.UsesStep
                {
                    Uses = new RawExpression("actions/setup-dotnet@v4"),
                    With = new Dictionary<string, TextExpressionCollection>
                    {
                        ["dotnet-version"] = "10.0.x",
                    },
                },
                new Step.RunStep { Run = ["dotnet build -c Release"] },
                new Step.RunStep { Run = ["dotnet test -c Release --no-build"] },
            ],
        },
    ],
};

var writer = new GithubActionWriter();
writer.Write(workflow);
File.WriteAllText(".github/workflows/ci.yml", writer.TextWriter.ToString());
```

## See also

- [Expressions](expressions.md) — the expression system used throughout the model
- [Dependabot](dependabot.md) — generating `dependabot.yml` with the same package


