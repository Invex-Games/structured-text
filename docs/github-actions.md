# GitHub Actions Workflows

The `Invex.StructuredText.GithubActions` package generates GitHub Actions workflow YAML from a strongly-typed model
rooted at `GithubAction` (namespace `Invex.StructuredText.GithubActions.GithubActionModel`). Read this page when you
want to go beyond the first example: triggers, jobs, steps, matrices, permissions, and passing outputs between jobs.

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
    new On.PullRequest([On.PullRequest.PullRequestType.opened, On.PullRequest.PullRequestType.synchronize])
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
    Name = "test",
    RunsOn = new() { Labels = ["ubuntu-latest"] },
    Needs = ["build"],                                   // job dependencies
    If = Tex.GitHub.EventName.Eq("push"),
    TimeoutMinutes = 30,
    Env = new Dictionary<string, Template>
    {
        ["DOTNET_NOLOGO"] = "true",
    },
    Outputs = new Dictionary<string, Template>
    {
        ["version"] = Tex.StepOutput("gitversion", "semVer"),
    },
    Steps = [ /* ... */ ],
}
```

`Name`, `TimeoutMinutes`, `Env` and `Outputs` values are templates, so they accept plain text, a number, an expression
or an interpolated string. `If` is a `BoolExpr` condition. See [Expressions](expressions.md).

### Matrix strategies

```csharp
Strategy = new Strategy
{
    Matrix = new Matrix
    {
        Map = new Dictionary<string, IReadOnlyList<Template>>
        {
            ["os"] = ["ubuntu-latest", "windows-latest", "macos-latest"],
            ["configuration"] = ["Debug", "Release"],
        },
        Exclude =
        [
            new Dictionary<string, Template>
            {
                ["os"] = "macos-latest",
                ["configuration"] = "Debug",
            },
        ],
    },
    FailFast = false,
    MaxParallel = 4,
},
RunsOn = new() { Labels = [Tex.Matrix("os")] },          // ${{ matrix.os }}
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
    Group = $"ci-{Tex.GitHub.Ref}",                     // ci-${{ github.ref }}
    CancelInProgress = Tex.GitHub.EventName.Eq("pull_request"),
},

Environment = new Environment { Name = "production" },

Container = new Container { Image = "mcr.microsoft.com/dotnet/sdk:10.0" },
Services = new Dictionary<string, Container>
{
    ["postgres"] = new() { Image = "postgres:16" },
},
```

## Steps

`Step` is a union of two variants. Shared properties (`Id`, `Name`, `If`, `Env`, `With`, `WorkingDirectory`,
`ContinueOnError`, `TimeoutMinutes`) live on the base record.

### `UsesStep`: run an action

```csharp
new Step.UsesStep
{
    Name = "Setup .NET",
    Uses = "actions/setup-dotnet@v5",
    With = new Dictionary<string, Template>
    {
        ["dotnet-version"] = "10.0.x",
    },
}
```

### `RunStep`: run shell commands

`Run` is a list of templates; more than one entry produces a multi-line `run: |` block.

```csharp
new Step.RunStep
{
    Id = "pack",
    Name = "Pack",
    Run =
    [
        $"dotnet pack --configuration Release -p:Version={Tex.JobOutput("setup", "version")}",
        "dotnet nuget push *.nupkg --api-key \"$NUGET_API_KEY\"",
    ],
    Shell = "bash",
    Env = new Dictionary<string, Template>
    {
        ["NUGET_API_KEY"] = Tex.Secret("NUGET_API_KEY"),  // ${{ secrets.NUGET_API_KEY }}
    },
}
```

> [!TIP]
> Pass secrets to commands through `Env` rather than interpolating them into `Run`, so they never appear in the
> generated script text.

### Conditional steps

```csharp
new Step.RunStep
{
    Name = "Publish (main only)",
    If = Tex.GitHub.Ref.Eq("refs/heads/main") && Tex.Success(),
    Run = ["dotnet nuget push *.nupkg"],
}
```

This renders `if: github.ref == 'refs/heads/main' && success()`. Conditions are written without `${{ }}` unless they
start with `!`, which YAML would read as a tag.

## Referencing outputs between steps and jobs

A step output is published as a job output, and a later job reads it with `Tex.JobOutput`:

```csharp
new Job
{
    Name = "build",
    RunsOn = new() { Labels = ["ubuntu-latest"] },
    Outputs = new Dictionary<string, Template>
    {
        ["version"] = Tex.StepOutput("gitversion", "semVer"),   // ${{ steps.gitversion.outputs.semVer }}
    },
    Steps = [ /* a step with Id = "gitversion" that writes semVer to $GITHUB_OUTPUT */ ],
},
new Job
{
    Name = "release",
    Needs = ["build"],
    If = !Tex.JobOutput("build", "version").Contains("-"),     // stable versions only
    RunsOn = new() { Labels = ["ubuntu-latest"] },
    Env = new Dictionary<string, Template>
    {
        ["VERSION"] = Tex.JobOutput("build", "version"),      // ${{ needs.build.outputs.version }}
    },
    Steps = [ /* ... */ ],
}
```

The `release` job's condition becomes `if: ${{ !contains(needs.build.outputs.version, '-') }}`.

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
        Group = $"ci-{Tex.GitHub.Ref}",
        CancelInProgress = true,
    },
    Jobs =
    [
        new Job
        {
            Name = "build",
            RunsOn = new() { Labels = ["ubuntu-latest"] },
            Steps =
            [
                new Step.UsesStep { Uses = "actions/checkout@v6" },
                new Step.UsesStep
                {
                    Uses = "actions/setup-dotnet@v5",
                    With = new Dictionary<string, Template> { ["dotnet-version"] = "10.0.x" },
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

The [samples](https://github.com/Invex-Games/structured-text/blob/main/samples/README.md) include a larger workflow
with a version job, a test matrix, a stable-only publish job and failure reporting, together with its output.

## See also

- [Expressions](expressions.md): the expression system used throughout the model
- [Dependabot Configuration](dependabot.md): generating `dependabot.yml` with the same package
- [StructuredTextWriter](structured-text-writer.md): the writer behind `GithubActionWriter.TextWriter`
- <xref:Invex.StructuredText.GithubActions.GithubActionModel.GithubAction>, <xref:Invex.StructuredText.GithubActions.GithubActionWriter>,
  <xref:Invex.StructuredText.GithubActions.GithubExpressionPrinter>
