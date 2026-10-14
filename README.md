# Invex.StructuredText

[![Build](https://github.com/Invex-Games/structured-text/actions/workflows/Build.yml/badge.svg)](https://github.com/Invex-Games/structured-text/actions/workflows/Build.yml)
[![NuGet](https://img.shields.io/nuget/v/Invex.StructuredText.svg)](https://www.nuget.org/packages/Invex.StructuredText)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](https://github.com/Invex-Games/structured-text/blob/main/LICENSE.txt)
[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4.svg)](https://dotnet.microsoft.com/)

A strongly-typed C# library for generating structured YAML text files, specifically GitHub Actions workflows, Dependabot
configurations, and Azure DevOps Pipelines, using a fluent, type-safe API with a powerful expression system.

## Packages

| Package | What it does |
| --- | --- |
| [Invex.StructuredText](https://www.nuget.org/packages/Invex.StructuredText) | Core library with `StructuredTextWriter` and the expression system |
| [Invex.StructuredText.GithubActions](https://www.nuget.org/packages/Invex.StructuredText.GithubActions) | GitHub Actions workflow and Dependabot config generation |
| [Invex.StructuredText.AzureDevopsPipelines](https://www.nuget.org/packages/Invex.StructuredText.AzureDevopsPipelines) | Azure DevOps Pipelines YAML generation |

## Features

- **Type-safe workflow authoring.** Define CI/CD pipelines as C# objects with full IntelliSense and compile-time validation.
- **Unified expression system.** Build conditions, string interpolation and function calls once and render them to each platform's syntax.
- **GitHub Actions.** Generate workflows with triggers, jobs, steps, matrix strategies, permissions, concurrency, containers, and more.
- **Dependabot.** Generate `dependabot.yml` configurations with registries, update rules, groups, and schedules.
- **Azure DevOps Pipelines.** Generate pipelines with stages, jobs, deployment strategies (runOnce, rolling, canary), resources, templates, and parameters.
- **Deterministic output.** The same model always produces the same text, so generated files diff cleanly.

## Installation

```shell
dotnet add package Invex.StructuredText.GithubActions
# or
dotnet add package Invex.StructuredText.AzureDevopsPipelines
```

Both platform packages depend on `Invex.StructuredText` core, so it will be installed automatically. All three packages
target `net10.0`. The platform packages also depend on the
[Dunet](https://github.com/domn1995/dunet) source generator, which provides their discriminated-union model types.

## Quick start

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
                    Run = ["dotnet build --configuration Release"],
                },
            ],
        },
    ],
};

var writer = new GithubActionWriter();
writer.Write(workflow);

File.WriteAllText(".github/workflows/ci.yml", writer.TextWriter.ToString());
```

## Usage

### Azure DevOps Pipelines

```csharp
var pipeline = new DevopsPipeline.DevopsPipelineWithSteps
{
    Trigger = new Trigger.BranchList { Branches = ["main"] },
    Steps =
    [
        new Step.Script { ScriptContent = new RawExpression("echo Hello, world!") },
    ],
};

var writer = new DevopsPipelineWriter();
writer.Write(pipeline);

var yaml = writer.TextWriter.ToString();
```

See [Azure DevOps Pipelines](https://github.com/Invex-Games/structured-text/blob/main/docs/azure-devops-pipelines.md).

### Dependabot

```csharp
var config = new DependabotConfig
{
    Updates =
    [
        new DependabotUpdate
        {
            PackageEcosystem = PackageEcosystem.Nuget.ToYamlValue(),
            Directory = "/",
            Schedule = new() { Interval = ScheduleInterval.Weekly },
        },
    ],
};

var writer = new DependabotConfigWriter();
writer.WriteConfig(config);

var yaml = writer.TextWriter.ToString();
```

See [Dependabot Configuration](https://github.com/Invex-Games/structured-text/blob/main/docs/dependabot.md).

### Expressions

The expression system provides a fluent API for building platform-specific expressions that are formatted differently
depending on the target (GitHub Actions uses `${{ }}` syntax, Azure DevOps uses its own macro/runtime expression
syntax).

```csharp
// Reference step outputs
var output = new StepOutputExpression
{
    StepName = "build-step",
    OutputName = "artifact-path",
};

// Use conditions
var condition = output.Contains("release").Evaluate();

// String interpolation with format expressions
var formatted = TextExpressions.Format($"Release-{output}");

// Logic operators
var combined = output.Contains("main") & new BooleanExpression(true);
```

See [Expressions](https://github.com/Invex-Games/structured-text/blob/main/docs/expressions.md).

### Generating files as part of your build

A small console project in your repository can regenerate every workflow file; commit the output and the diff shows
exactly what changed. Use a fresh writer per file, or call `writer.TextWriter.Reset()` between documents. See
[Getting Started](https://github.com/Invex-Games/structured-text/blob/main/docs/getting-started.md#a-common-workflow-generate-and-commit).

## Documentation

Full documentation: [https://invex-games.github.io/structured-text/](https://invex-games.github.io/structured-text/)

| Guide | Contents |
| --- | --- |
| [Introduction](https://github.com/Invex-Games/structured-text/blob/main/docs/introduction.md) | What this library is, when to use it, and its key concepts |
| [Getting Started](https://github.com/Invex-Games/structured-text/blob/main/docs/getting-started.md) | Install and generate your first file |
| [Expressions](https://github.com/Invex-Games/structured-text/blob/main/docs/expressions.md) | The unified expression system |
| [StructuredTextWriter](https://github.com/Invex-Games/structured-text/blob/main/docs/structured-text-writer.md) | The indentation-aware writer behind every generator |
| [GitHub Actions Workflows](https://github.com/Invex-Games/structured-text/blob/main/docs/github-actions.md) | Workflow generation in depth |
| [Dependabot Configuration](https://github.com/Invex-Games/structured-text/blob/main/docs/dependabot.md) | `dependabot.yml` generation |
| [Azure DevOps Pipelines](https://github.com/Invex-Games/structured-text/blob/main/docs/azure-devops-pipelines.md) | Pipelines, stages, jobs, deployment strategies |
| [Architecture](https://github.com/Invex-Games/structured-text/blob/main/docs/architecture.md) | How the packages fit together and how to extend them |
| [FAQ](https://github.com/Invex-Games/structured-text/blob/main/docs/faq.md) | Common questions and troubleshooting |
| [API Reference](https://invex-games.github.io/structured-text/api/index.html) | Generated reference for every public type |

## Development

The repository requires the .NET 10 SDK selected by `global.json`:

```shell
dotnet build Invex.StructuredText.slnx
dotnet test Invex.StructuredText.slnx
```

Build and preview the documentation site with the Atom build:

```shell
atom BuildDocs
atom ServeDocs
```

See [.github/copilot-instructions.md](https://github.com/Invex-Games/structured-text/blob/main/.github/copilot-instructions.md)
for repository structure, code conventions, and contributor guidance.

## License

This project is licensed under the [MIT License](https://github.com/Invex-Games/structured-text/blob/main/LICENSE.txt).
