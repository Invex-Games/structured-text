# Azure DevOps Pipelines

The `Invex.StructuredText.AzureDevopsPipelines` package generates Azure Pipelines YAML from a strongly-typed model
rooted at `DevopsPipeline` (namespace `Invex.StructuredText.AzureDevopsPipelines.DevopsPipelineModel.*`).

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
    Pool = new Pool.PoolSpec { VmImage = new RawExpression("ubuntu-latest") },
    Trigger = new Trigger.BranchList { Branches = ["main"] },
    Steps =
    [
        new Step.Script { ScriptContent = new RawExpression("echo hello") },
    ],
};
```

```yaml
trigger:
  branches:
    include:
      - main

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
            JobId = new RawExpression("build"),
            DisplayName = new RawExpression("Build"),
            Pool = new Pool.PoolSpec { VmImage = new RawExpression("ubuntu-latest") },
            Steps =
            [
                new Step.Script { ScriptContent = new RawExpression("dotnet build") },
            ],
        },
        new Job.RegularJob
        {
            JobId = new RawExpression("test"),
            DependsOn = ["build"],
            Condition = new RawExpression("succeeded()"),
            Steps =
            [
                new Step.Script { ScriptContent = new RawExpression("dotnet test") },
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
            StageId = new RawExpression("Build"),
            Jobs = [ /* Job.RegularJob ... */ ],
        },
        new Stage.StageDefinition
        {
            StageId = new RawExpression("Deploy"),
            DependsOn = ["Build"],
            Condition = new RawExpression("succeeded()"),
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
        Template = new RawExpression("pipeline-template.yml@templates"),
        Parameters = new Dictionary<string, TextExpression>
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
        Repository = new RawExpression("self"),
        FetchDepth = 0,
        Lfs = true,
    },
    new Step.Task
    {
        TaskName = new RawExpression("UseDotNet@2"),
        DisplayName = new RawExpression("Install .NET SDK"),
        Inputs = new Dictionary<string, TextExpression>
        {
            ["packageType"] = "sdk",
            ["version"] = "10.0.x",
        },
    },
    new Step.Script
    {
        ScriptContent = new RawExpression("dotnet build -c Release"),
        DisplayName = new RawExpression("Build"),
        Env = new Dictionary<string, TextExpression>
        {
            ["DOTNET_NOLOGO"] = "true",
        },
    },
    new Step.Publish
    {
        PublishPath = new RawExpression("$(Build.ArtifactStagingDirectory)"),
        Artifact = new RawExpression("drop"),
        Condition = new RawExpression("succeededOrFailed()"),
    },
],
```

## Pools

```csharp
// Microsoft-hosted image
Pool = new Pool.PoolSpec { VmImage = new RawExpression("ubuntu-latest") },

// Named (self-hosted) pool
Pool = new Pool.PoolName { Name = new RawExpression("Default") },
```

## Variables

`Variables` is a union supporting both YAML syntaxes:

```csharp
// Mapping syntax: name/value pairs
Variables = new Variables.Dictionary
{
    Values = new Dictionary<string, TextExpression>
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
            VariableName = new RawExpression("buildConfiguration"),
            Value = new RawExpression("Release"),
            ReadOnly = true,
        },
        new Variable.Group { GroupName = new RawExpression("shared-secrets") },
        new Variable.Template { TemplatePath = new RawExpression("variables/common.yml") },
    ],
},
```

Reference variables in steps via macro syntax:

```csharp
var configuration = new DevopsMacroExpression(new RawExpression("buildConfiguration"));
// renders as $(buildConfiguration)

new Step.Script
{
    ScriptContent = TextExpressions.Concat(
    [
        new RawExpression("dotnet build -c "),
        configuration,
    ]),
}
```

## Deployment jobs and strategies

Deployment jobs (`Job.Deployment`) target an environment and require a `DeploymentStrategy` — a union over `RunOnce`,
`Rolling`, and `Canary`, each with lifecycle hooks (`PreDeploy`, `Deploy`, `RouteTraffic`, `PostRouteTraffic`,
`OnSuccess`, `OnFailure`):

```csharp
new Job.Deployment
{
    DeploymentId = new RawExpression("deploy_prod"),
    DisplayName = new RawExpression("Deploy to production"),
    Environment = new DeploymentEnvironment.EnvironmentName
    {
        Name = new RawExpression("production"),
    },
    Strategy = new DeploymentStrategy.RunOnce
    {
        Deploy = new DeploymentHook
        {
            Steps =
            [
                new Step.Script { ScriptContent = new RawExpression("./deploy.sh") },
            ],
        },
        OnFailure = new DeploymentHook
        {
            Steps =
            [
                new Step.Script { ScriptContent = new RawExpression("./rollback.sh") },
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

The `DevopsExpressionFormatter` renders the shared expression tree using Azure DevOps function-style syntax:

| C#                                              | Output                          |
|--------------------------------------------------|---------------------------------|
| `a & b`                                          | `and(a, b)`                     |
| `a \| b`                                         | `or(a, b)`                      |
| `!a`                                             | `not(a)`                        |
| `a.EqualToString("x")`                           | `eq(a, 'x')`                    |
| `a.Contains("x")`                                | `contains(a, 'x')`              |
| `expr.Evaluate()`                                | `${{ expr }}` (compile-time)    |
| `new DevopsRuntimeExpression(expr)`              | `$[ expr ]` (runtime)           |
| `new DevopsMacroExpression(variable)`            | `$(variable)` (macro)           |

See [Expressions](expressions.md) for the full system.

## See also

- [Getting Started](getting-started.md)
- [Expressions](expressions.md)
- The model closely follows the
  [Azure Pipelines YAML schema](https://learn.microsoft.com/azure/devops/pipelines/yaml-schema/) — property names and
  XML docs mirror the official documentation.

