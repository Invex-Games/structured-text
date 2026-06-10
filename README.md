# Invex.StructuredText

[![NuGet](https://img.shields.io/nuget/v/Invex.StructuredText.svg)](https://www.nuget.org/packages/Invex.StructuredText)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE.txt)

A strongly-typed C# library for generating structured YAML text files, specifically GitHub Actions workflows, Dependabot
configurations, and Azure DevOps Pipelines, using a fluent, type-safe API with a powerful expression system.

## Features

### Type-safe workflow authoring

Define CI/CD pipelines as C# objects with full IntelliSense and compile-time validation

### Unified expression system

Build complex expressions (conditions, string interpolation, functions) that compile to platform-specific syntax

### GitHub Actions

Full support for workflow YAML generation including triggers, jobs, steps, matrix strategies,
permissions, concurrency, containers, and more

### Dependabot

Generate `dependabot.yml` configurations with registries, update rules, groups, and schedules

### Azure DevOps Pipelines

Comprehensive pipeline generation with stages, jobs, deployment strategies (runOnce,
rolling, canary), resources, templates, and parameters

## Packages

| Package                                                                                                                 | Description                                                        |
|-------------------------------------------------------------------------------------------------------------------------|--------------------------------------------------------------------|
| [`Invex.StructuredText`](https://www.nuget.org/packages/Invex.StructuredText)                                           | Core library with `StructuredTextWriter` and the expression system |
| [`Invex.StructuredText.GithubActions`](https://www.nuget.org/packages/Invex.StructuredText.GithubActions)               | GitHub Actions workflow and Dependabot config generation           |
| [`Invex.StructuredText.AzureDevopsPipelines`](https://www.nuget.org/packages/Invex.StructuredText.AzureDevopsPipelines) | Azure DevOps Pipelines YAML generation                             |

## Installation

```shell
dotnet add package Invex.StructuredText.GithubActions
# or
dotnet add package Invex.StructuredText.AzureDevopsPipelines
```

Both platform packages depend on `Invex.StructuredText` core, so it will be installed automatically.

## Documentation

Comprehensive guides live in the [`docs/`](docs/introduction.md) directory:

- [Introduction](docs/introduction.md) — what this library is and why it exists
- [Getting Started](docs/getting-started.md) — install and generate your first file
- [Expressions](docs/expressions.md) — the unified expression system
- [GitHub Actions](docs/github-actions.md) — workflow generation in depth
- [Dependabot](docs/dependabot.md) — `dependabot.yml` generation
- [Azure DevOps Pipelines](docs/azure-devops-pipelines.md) — pipelines, stages, jobs, deployment strategies
- [Architecture](docs/architecture.md) — how the pieces fit together
- [FAQ](docs/faq.md)

An auto-generated [API reference](api/index.md) is built with docfx from the XML documentation comments.

## Quick Start

### GitHub Actions

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

var yaml = writer.TextWriter.ToString();
```

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

## Architecture

### Core (`Invex.StructuredText`)

- **`StructuredTextWriter`**: A low-level indentation-aware text writer that handles YAML-style output with automatic
  indent management via `IDisposable` scopes
- **Expression System**: A rich set of record types (`TextExpression` hierarchy) representing values, functions, logic
  operators, and workflow-specific constructs (step outputs, job outcomes, etc.)
- **`TextExpressionFormatter`**: Base class for platform-specific expression formatters

### GitHub Actions (`Invex.StructuredText.GithubActions`)

- **`GithubActionWriter`**: Serializes a `GithubAction` model to valid workflow YAML
- **`DependabotConfigWriter`**: Serializes a `DependabotConfig` model to valid `dependabot.yml`
- **`GithubExpressionFormatter`**: Formats expressions using GitHub's `${{ }}` syntax
- **Model types**: Strongly-typed records for all GitHub Actions concepts (triggers, jobs, steps, matrix, permissions,
  etc.)

### Azure DevOps Pipelines (`Invex.StructuredText.AzureDevopsPipelines`)

- **`DevopsPipelineWriter`**: Serializes a `DevopsPipeline` model to valid pipeline YAML
- **`DevopsExpressionFormatter`**: Formats expressions using Azure DevOps syntax
- **Model types**: Strongly-typed records/unions for pipelines, stages, jobs, steps, resources, triggers, variables,
  deployment strategies, and more

## Requirements

- .NET 8.0, .NET 9.0, or .NET 10.0

## License

This project is licensed under the [MIT License](LICENSE.txt).