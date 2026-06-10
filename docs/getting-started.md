# Getting Started

## Requirements

- .NET 8.0, .NET 9.0, or .NET 10.0

## Installation

Install the package for the platform you want to target. Each platform package automatically brings in the
`Invex.StructuredText` core library.

```shell
# GitHub Actions workflows + Dependabot configs
dotnet add package Invex.StructuredText.GithubActions

# Azure DevOps Pipelines
dotnet add package Invex.StructuredText.AzureDevopsPipelines

# Just the core writer + expression system
dotnet add package Invex.StructuredText
```

## The general pattern

Every generator in this library follows the same three-step pattern:

1. **Build a model** — construct an object graph (`GithubAction`, `DependabotConfig`, or `DevopsPipeline`) using
   records with `init` properties and collection expressions.
2. **Write it** — pass the model to the corresponding writer (`GithubActionWriter`, `DependabotConfigWriter`, or
   `DevopsPipelineWriter`).
3. **Get the text** — call `writer.TextWriter.ToString()` and save it wherever you need.

```csharp
var writer = new GithubActionWriter();
writer.Write(myWorkflow);

var yaml = writer.TextWriter.ToString();
File.WriteAllText(".github/workflows/ci.yml", yaml);
```

## Your first GitHub Actions workflow

```csharp
using Invex.StructuredText.Expressions;
using Invex.StructuredText.GithubActions;
using Invex.StructuredText.GithubActions.GithubActionModel;

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
    Jobs =
    [
        new Job
        {
            Name = new RawExpression("build"),
            RunsOn = new() { Labels = [new RawExpression("ubuntu-latest")] },
            Steps =
            [
                new Step.UsesStep
                {
                    Name = new RawExpression("Checkout"),
                    Uses = new RawExpression("actions/checkout@v4"),
                },
                new Step.RunStep
                {
                    Name = new RawExpression("Build"),
                    Run = [new RawExpression("dotnet build --configuration Release")],
                },
            ],
        },
    ],
};

var writer = new GithubActionWriter();
writer.Write(workflow);

Console.WriteLine(writer.TextWriter.ToString());
```

Output:

```yaml
name: CI

on:
  push:
    branches:
      - main

jobs:

  build:
    runs-on: ubuntu-latest
    steps:
      - name: Checkout
        uses: actions/checkout@v4

      - name: Build
        run: dotnet build --configuration Release
```

> [!TIP]
> Many string-valued properties accept `TextExpression` and have implicit conversions from `string`, so you can often
> write `Branches = ["main"]` instead of building `RawExpression` instances by hand. See
> [Expressions](expressions.md) for details.

## Your first Azure DevOps pipeline

```csharp
using Invex.StructuredText.AzureDevopsPipelines;
using Invex.StructuredText.AzureDevopsPipelines.DevopsPipelineModel.Pipeline;
using Invex.StructuredText.AzureDevopsPipelines.DevopsPipelineModel.Steps;
using Invex.StructuredText.AzureDevopsPipelines.DevopsPipelineModel.Triggers;
using Invex.StructuredText.Expressions;

var pipeline = new DevopsPipeline.DevopsPipelineWithSteps
{
    Trigger = new Trigger.BranchList { Branches = ["main"] },
    Steps =
    [
        new Step.Script
        {
            ScriptContent = new RawExpression("echo Hello, world!"),
            DisplayName = new RawExpression("Say hello"),
        },
    ],
};

var writer = new DevopsPipelineWriter();
writer.Write(pipeline);

Console.WriteLine(writer.TextWriter.ToString());
```

Output:

```yaml
trigger:
  branches:
    include:
      - main

steps:

  - script: echo Hello, world!
    displayName: Say hello
```

## Your first Dependabot config

```csharp
using Invex.StructuredText.GithubActions;
using Invex.StructuredText.GithubActions.DependabotConfigModel.Model;

var config = new DependabotConfig
{
    Updates =
    [
        new DependabotUpdate
        {
            PackageEcosystem = "nuget",
            Directory = "/",
            Schedule = new() { Interval = ScheduleInterval.Weekly },
        },
    ],
};

var writer = new DependabotConfigWriter();
writer.WriteConfig(config);

File.WriteAllText(".github/dependabot.yml", writer.TextWriter.ToString());
```

## A common workflow: generate-and-commit

A typical setup is a small console project in your repository (e.g. `_build/` or `_atom/`) that regenerates all
workflow files. Run it locally or in CI, then commit the output. Because generation is deterministic, the diff shows
exactly what changed.

```csharp
// Program.cs in a build-tool project
var writer = new GithubActionWriter();
writer.Write(BuildCiWorkflow());
File.WriteAllText("../.github/workflows/ci.yml", writer.TextWriter.ToString());

writer = new GithubActionWriter(); // fresh writer per file
writer.Write(BuildReleaseWorkflow());
File.WriteAllText("../.github/workflows/release.yml", writer.TextWriter.ToString());
```

> [!NOTE]
> Writers accumulate text in an internal `StructuredTextWriter`. Use a fresh writer per output file, or call
> `writer.TextWriter.Reset()` between writes.

## Next steps

- [Expressions](expressions.md) — conditions, interpolation, and platform-specific syntax
- [GitHub Actions](github-actions.md) — the full workflow model
- [Azure DevOps Pipelines](azure-devops-pipelines.md) — stages, jobs, deployment strategies, resources
- [Dependabot](dependabot.md) — registries, groups, schedules

