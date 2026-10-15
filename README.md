# Invex.StructuredText

[![Build](https://github.com/Invex-Games/structured-text/actions/workflows/Build.yml/badge.svg)](https://github.com/Invex-Games/structured-text/actions/workflows/Build.yml)
[![NuGet](https://img.shields.io/nuget/v/Invex.StructuredText.svg)](https://www.nuget.org/packages/Invex.StructuredText)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](https://github.com/Invex-Games/structured-text/blob/main/LICENSE.txt)
[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4.svg)](https://dotnet.microsoft.com/)

A strongly-typed C# library for generating structured YAML text files, specifically GitHub Actions workflows, Dependabot
configurations, and Azure DevOps Pipelines, using a type-safe model and typed expressions.

## Packages

| Package | What it does |
| --- | --- |
| [Invex.StructuredText](https://www.nuget.org/packages/Invex.StructuredText) | Core library with `StructuredTextWriter` and the expression API |
| [Invex.StructuredText.GithubActions](https://www.nuget.org/packages/Invex.StructuredText.GithubActions) | GitHub Actions workflow and Dependabot config generation |
| [Invex.StructuredText.AzureDevopsPipelines](https://www.nuget.org/packages/Invex.StructuredText.AzureDevopsPipelines) | Azure DevOps Pipelines YAML generation |

## Features

- **Type-safe workflow authoring.** Define CI/CD pipelines as C# objects with full IntelliSense and compile-time validation.
- **Typed expressions.** Write conditions with C# operators and values as interpolated strings, starting from one entry point, `Tex`, and render them in each platform's syntax.
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

File.WriteAllText(".github/workflows/ci.yml", writer.TextWriter.ToString());
```

## Usage

### Expressions

Everything that creates an expression starts with `Tex.`: references, status checks, functions and the platform
contexts. Conditions use C# operators, and values are interpolated strings.

```csharp
// if: (github.event_name == 'push' || startsWith(github.ref, 'refs/tags/')) && success()
If = (Tex.GitHub.EventName.Eq("push") || Tex.GitHub.Ref.StartsWith("refs/tags/")) && Tex.Success(),

// group: ${{ github.workflow }}-${{ github.ref }}
Group = $"{Tex.GitHub.Workflow}-{Tex.GitHub.Ref}",

// NUGET_API_KEY: ${{ secrets.NUGET_API_KEY }}
["NUGET_API_KEY"] = Tex.Secret("NUGET_API_KEY"),

// version: ${{ needs.setup.outputs.version }}
["version"] = Tex.JobOutput("setup", "version"),
```

The same expressions print in Azure DevOps syntax when used in a pipeline. See
[Expressions](https://github.com/Invex-Games/structured-text/blob/main/docs/expressions.md) and the
[samples](https://github.com/Invex-Games/structured-text/blob/main/samples/README.md).

### Azure DevOps Pipelines

```csharp
var pipeline = new DevopsPipeline.DevopsPipelineWithSteps
{
    Trigger = new Trigger.BranchList { Branches = ["main"] },
    Steps =
    [
        new Step.Script
        {
            ScriptContent = $"echo Building {Tex.DevOps.Build.SourceBranchName}",
            Condition = Tex.DevOps.Build.Reason.Ne("PullRequest"),
        },
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
| [Expressions](https://github.com/Invex-Games/structured-text/blob/main/docs/expressions.md) | Conditions, references and interpolated values |
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

See [AGENTS.md](https://github.com/Invex-Games/structured-text/blob/main/AGENTS.md)
for repository structure, code conventions, and contributor guidance.

## License

This project is licensed under the [MIT License](https://github.com/Invex-Games/structured-text/blob/main/LICENSE.txt).
