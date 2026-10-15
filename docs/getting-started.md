# Getting Started

This page shows how to install the packages and generate your first GitHub Actions workflow, Azure DevOps pipeline,
and Dependabot config. Start here if you have not used the library before.

## Requirements

- .NET 10.0

## Installation

Install the package for the platform you want to target. Each platform package automatically brings in the
`Invex.StructuredText` core library.

```shell
# GitHub Actions workflows + Dependabot configs
dotnet add package Invex.StructuredText.GithubActions

# Azure DevOps Pipelines
dotnet add package Invex.StructuredText.AzureDevopsPipelines

# Just the core writer and expression API
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
            Name = "build",
            RunsOn = new() { Labels = ["ubuntu-latest"] },
            Steps =
            [
                new Step.UsesStep { Name = "Checkout", Uses = "actions/checkout@v6" },
                new Step.RunStep
                {
                    Name = "Build",
                    If = Tex.GitHub.EventName.Eq("push"),
                    Run = [$"dotnet build -c Release -p:Version=1.0.{Tex.GitHub.RunNumber}"],
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
    branches: [ main ]

jobs:

  build:
    runs-on: ubuntu-latest
    steps:

      - name: Checkout
        uses: actions/checkout@v6

      - name: Build
        if: github.event_name == 'push'
        run: 'dotnet build -c Release -p:Version=1.0.${{ github.run_number }}'
```

Plain strings are written as they are. `Tex.GitHub.EventName.Eq("push")` is a typed condition, and the interpolated
`Run` line embeds `github.run_number` as an expression. Everything that creates an expression starts with `Tex.`, so
IntelliSense on `Tex.` is the quickest way to find what you need. See [Expressions](expressions.md).

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
            ScriptContent = $"echo Building {Tex.DevOps.Build.SourceBranchName}",
            DisplayName = "Say hello",
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
    include: [ main ]
steps:

  - script: echo Building $(Build.SourceBranchName)
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

- [Expressions](expressions.md): conditions, references and interpolated values
- [GitHub Actions Workflows](github-actions.md): the full workflow model
- [Azure DevOps Pipelines](azure-devops-pipelines.md): stages, jobs, deployment strategies, resources
- [Dependabot Configuration](dependabot.md): registries, groups, schedules
- [FAQ](faq.md): common questions and troubleshooting
- API: <xref:Invex.StructuredText.GithubActions.GithubActionWriter>, <xref:Invex.StructuredText.GithubActions.DependabotConfigWriter>,
  <xref:Invex.StructuredText.AzureDevopsPipelines.DevopsPipelineWriter>
