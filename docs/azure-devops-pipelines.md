# Azure DevOps Pipelines

The `Invex.StructuredText.AzureDevopsPipelines` package generates Azure Pipelines YAML from a strongly-typed model
rooted at `DevopsPipeline` (namespace `Invex.StructuredText.AzureDevopsPipelines.DevopsPipelineModel.*`). Read this
page when you generate Azure Pipelines: pipeline shapes, triggers, steps, pools, variables, deployment jobs, and
Azure DevOps expression syntax.

```csharp
var writer = new DevopsPipelineWriter();
writer.Write(pipeline);
var yaml = writer.TextWriter.ToString();
```

## The four pipeline shapes

Azure Pipelines YAML allows four top-level shapes, and `DevopsPipeline` is a union over exactly those four:

| Variant                       | YAML shape                                       |
|-------------------------------|--------------------------------------------------|
| `DevopsPipelineWithSteps`     | `steps:` at the root (implicit single job)       |
| `DevopsPipelineWithJobs`      | `jobs:` at the root (implicit single stage)      |
| `DevopsPipelineWithStages`    | `stages:` — the full multi-stage syntax          |
| `DevopsPipelineWithExtends`   | `extends:` — extend a pipeline template          |

All four share the common pipeline-level properties: `Name` (run number format), `Trigger`, `Pr`, `Schedules`,
`Parameters`, `Pool`, `Variables`, resources, `LockBehavior`, and `AppendCommitMessageToRunName`.

### Steps pipeline

```csharp
var pipeline = new DevopsPipeline.DevopsPipelineWithSteps
{
    Pool = new Pool.PoolSpec { VmImage = "ubuntu-latest" },
    Trigger = new Trigger.BranchList { Branches = ["main"] },
    Steps =
    [
        new Step.Script { ScriptContent = "echo hello" },
    ],
};
```

```yaml
trigger:
  branches:
    include: [ main ]
pool:
  vmImage: ubuntu-latest
steps:

  - script: echo hello
```

### Jobs pipeline

```csharp
var pipeline = new DevopsPipeline.DevopsPipelineWithJobs
{
    Jobs =
    [
        new Job.RegularJob
        {
            JobId = "build",
            DisplayName = "Build",
            Pool = new Pool.PoolSpec { VmImage = "ubuntu-latest" },
            Steps =
            [
                new Step.Script { ScriptContent = "dotnet build" },
            ],
        },
        new Job.RegularJob
        {
            JobId = "test",
            DependsOn = ["build"],
            Condition = Tex.Success(),
            Steps =
            [
                new Step.Script { ScriptContent = "dotnet test" },
            ],
        },
    ],
};
```

### Stages pipeline

```csharp
var pipeline = new DevopsPipeline.DevopsPipelineWithStages
{
    Stages =
    [
        new Stage.StageDefinition
        {
            StageId = "Build",
            Jobs = [ /* Job.RegularJob ... */ ],
        },
        new Stage.StageDefinition
        {
            StageId = "Deploy",
            DependsOn = ["Build"],
            Condition = Tex.Success(),
            Jobs = [ /* deployment jobs ... */ ],
        },
    ],
};
```

### Extends pipeline

```csharp
var pipeline = new DevopsPipeline.DevopsPipelineWithExtends
{
    Extends = new Extends
    {
        Template = "pipeline-template.yml@templates",
        Parameters = new Dictionary<string, Template>
        {
            ["configuration"] = "Release",
        },
    },
};
```

## Triggers

```csharp
// CI trigger: branch list
Trigger = new Trigger.BranchList { Branches = ["main", "release/*"] },

// CI trigger: full control
Trigger = new Trigger.Full
{
    Batch = true,
    Branches = new IncludeExcludeFilters
    {
        Include = ["main"],
        Exclude = ["experimental/*"],
    },
    Paths = new IncludeExcludeFilters { Include = ["src/*"] },
},

// Disable CI triggers entirely
Trigger = new Trigger.None(),
```

`Pr` (pull-request triggers) and `Schedules` (cron triggers) follow the same pattern with their own union/record
types.

## Steps

`Step` is a union over the twelve Azure Pipelines step kinds:

| Variant         | YAML key          | Required property |
|-----------------|-------------------|-------------------|
| `Task`          | `task:`           | `TaskName` (+ `Inputs`) |
| `Script`        | `script:`         | `ScriptContent`   |
| `Bash`          | `bash:`           | `ScriptContent`   |
| `PowerShell`    | `powershell:`     | `ScriptContent`   |
| `Pwsh`          | `pwsh:`           | `ScriptContent`   |
| `Checkout`      | `checkout:`       | `Repository`      |
| `Download`      | `download:`       | `Pipeline`        |
| `DownloadBuild` | `downloadBuild:`  | `Build`           |
| `GetPackage`    | `getPackage:`     | `Package`         |
| `Publish`       | `publish:`        | `PublishPath`     |
| `Template`      | `template:`       | `TemplatePath`    |
| `ReviewApp`     | `reviewApp:`      | `ReviewAppType`   |

All script/task-like steps share the common properties `DisplayName`, `Name`, `Condition`, `ContinueOnError`,
`Enabled`, `Env`, `Target`, `TimeoutInMinutes`, and `RetryCountOnTaskFailure`.

```csharp
Steps =
[
    new Step.Checkout
    {
        Repository = "self",
        FetchDepth = 0,
        Lfs = true,
    },
    new Step.Task
    {
        TaskName = "UseDotNet@2",
        DisplayName = "Install .NET SDK",
        Inputs = new Dictionary<string, Template>
        {
            ["packageType"] = "sdk",
            ["version"] = "10.0.x",
        },
    },
    new Step.Script
    {
        ScriptContent = "dotnet build -c Release",
        DisplayName = "Build",
        Env = new Dictionary<string, Template>
        {
            ["DOTNET_NOLOGO"] = "true",
        },
    },
    new Step.Publish
    {
        PublishPath = Tex.DevOps.Build.ArtifactStagingDirectory,
        Artifact = "drop",
        Condition = Tex.Always(),
    },
],
```

## Pools

```csharp
// Microsoft-hosted image
Pool = new Pool.PoolSpec { VmImage = "ubuntu-latest" },

// Named (self-hosted) pool
Pool = new Pool.PoolName { Name = "Default" },
```

## Variables

`Variables` is a union supporting both YAML syntaxes:

```csharp
// Mapping syntax: name/value pairs
Variables = new Variables.Dictionary
{
    Values = new Dictionary<string, Template>
    {
        ["buildConfiguration"] = "Release",
    },
},

// List syntax: named variables, groups, and templates
Variables = new Variables.VariableList
{
    Values =
    [
        new Variable.Name
        {
            VariableName = "buildConfiguration",
            Value = "Release",
            ReadOnly = true,
        },
        new Variable.Group { GroupName = "shared-secrets" },
        new Variable.Template { TemplatePath = "variables/common.yml" },
    ],
},
```

Reference variables in steps with `Tex.Variable`, which is written in macro syntax:

```csharp
new Step.Script
{
    ScriptContent = $"dotnet build -c {Tex.Variable("buildConfiguration")}",   // dotnet build -c $(buildConfiguration)
}
```

## Deployment jobs and strategies

Deployment jobs (`Job.Deployment`) target an environment and require a `DeploymentStrategy` — a union over `RunOnce`,
`Rolling`, and `Canary`, each with lifecycle hooks (`PreDeploy`, `Deploy`, `RouteTraffic`, `PostRouteTraffic`,
`OnSuccess`, `OnFailure`):

```csharp
new Job.Deployment
{
    DeploymentId = "deploy_prod",
    DisplayName = "Deploy to production",
    Environment = new DeploymentEnvironment.EnvironmentName
    {
        Name = "production",
    },
    Strategy = new DeploymentStrategy.RunOnce
    {
        Deploy = new DeploymentHook
        {
            Steps =
            [
                new Step.Script { ScriptContent = "./deploy.sh" },
            ],
        },
        OnFailure = new DeploymentHook
        {
            Steps =
            [
                new Step.Script { ScriptContent = "./rollback.sh" },
            ],
        },
    },
}
```

Rolling deployments add `MaxParallel`; canary deployments require `Increments` (e.g. `[10, 20, 70]`).

## Resources

`PipelineResources` / `Resources` lets you declare `repositories`, `pipelines`, `builds`, `containers`, `packages`, and
`webhooks` used by the pipeline — each modeled with its own strongly-typed record under
`DevopsPipelineModel.Resources`.

## Templates and parameters

- Job, stage, step, and variable lists each have a `Template` variant for referencing template files with
  `Parameters`.
- Pipeline-level `Parameters` define runtime parameters for the pipeline (or template parameters when authoring
  templates).

## Expressions in Azure DevOps

Azure DevOps uses the same expression API as GitHub Actions (see [Expressions](expressions.md)), printed with
`DevopsExpressionPrinter`. Conditions use the function-style syntax:

| C# | Output |
|---|---|
| `a && b` | `and(a, b)` |
| `a \|\| b` | `or(a, b)` |
| `!a` | `not(a)` |
| `a.Eq("x")`, `a.Ne("x")` | `eq(a, 'x')`, `ne(a, 'x')` |
| `n.Lt(1)`, `n.Le(1)`, `n.Gt(1)`, `n.Ge(1)` | `lt(n, 1)`, `le(n, 1)`, `gt(n, 1)`, `ge(n, 1)` |
| `a.Contains("x")` | `contains(a, 'x')` |
| `Tex.Success()`, `Tex.Failure()`, `Tex.Cancelled()`, `Tex.Always()` | `succeeded()`, `failed()`, `canceled()`, `always()` |
| `Tex.JobResult("build").Eq(Tex.Result(RunResult.Success))` | `eq(dependencies.build.result, 'Succeeded')` |

```csharp
Condition = Tex.DevOps.Build.SourceBranch.Eq("refs/heads/main") && !Tex.DevOps.Build.Reason.Eq("PullRequest"),
// condition: and(eq(variables['Build.SourceBranch'], 'refs/heads/main'), not(eq(variables['Build.Reason'], 'PullRequest')))
```

Inside a template, each kind of value is embedded with the syntax Azure DevOps expects for it:

| C# | Embedded as |
|---|---|
| `Tex.Variable("x")`, `Tex.Secret("x")`, `Tex.Matrix("x")` | `$(x)` (macro) |
| `Tex.StepOutput("step", "out")` | `$(step.out)` (macro) |
| `Tex.Input("x")` | `${{ parameters.x }}` (template expression) |
| `Tex.JobOutput("job", "out", "step")` | `$[ dependencies.job.outputs['step.out'] ]` (runtime expression) |
| `Tex.DevOps.Runtime(expression)` | `$[ expression ]` (runtime expression) |
| Any expression that reads a job output or job result | `$[ expression ]` (runtime expression) |
| Any other expression | `${{ expression }}` (template expression) |

> [!IMPORTANT]
> Azure DevOps only evaluates a runtime expression that is the whole value, so a template such as
> `$"v{Tex.JobOutput("Setup", "version", "Info")}"` throws `NotSupportedException`. Assign the runtime value to a
> variable, then interpolate `Tex.Variable("name")`.

`Tex.DevOps` lists the common predefined variables, such as `Tex.DevOps.Build.SourceBranch`,
`Tex.DevOps.Build.BuildId`, `Tex.DevOps.System.AccessToken`, `Tex.DevOps.Agent.Os` and
`Tex.DevOps.PipelineWorkspace`. Use `Tex.Variable("Name")` for any other variable.

```csharp
Variables = new Variables.Dictionary
{
    Values = new Dictionary<string, Template>
    {
        // isMain: $[ eq(variables['Build.SourceBranch'], 'refs/heads/main') ]
        ["isMain"] = Tex.DevOps.Runtime(Tex.DevOps.Build.SourceBranch.Eq("refs/heads/main")),
    },
},
```

> [!NOTE]
> Azure DevOps has no `fromJSON`, so `Tex.FromJson` throws `NotSupportedException` here. `ToJson()` prints
> `convertToJson(...)`, `Join` puts the separator first (`join(',', items)`), and array membership (`value.In(items)`,
> `items.Contains(value)`) prints `containsValue(items, value)`.

## See also

- [Getting Started](getting-started.md): your first pipeline
- [Expressions](expressions.md): the expression API shared with GitHub Actions
- The model closely follows the
  [Azure Pipelines YAML schema](https://learn.microsoft.com/azure/devops/pipelines/yaml-schema/) — property names and
  XML docs mirror the official documentation.
- <xref:Invex.StructuredText.AzureDevopsPipelines.DevopsPipelineModel.Pipeline.DevopsPipeline>,
  <xref:Invex.StructuredText.AzureDevopsPipelines.DevopsPipelineWriter>,
  <xref:Invex.StructuredText.AzureDevopsPipelines.DevopsExpressionPrinter>
