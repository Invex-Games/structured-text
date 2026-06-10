# Introduction

**Invex.StructuredText** is a family of strongly-typed C# libraries for generating structured YAML text files —
specifically **GitHub Actions workflows**, **Dependabot configurations**, and **Azure DevOps Pipelines** — using a
fluent, type-safe API with a unified expression system.

## Why generate YAML from C#?

CI/CD configuration files grow quickly. Hand-written YAML suffers from:

- **No compile-time validation** — typos in keys, indentation mistakes, and invalid values are only discovered when the
  pipeline runs (or fails to parse).
- **No refactoring support** — renaming a job, output, or variable means find-and-replace across files and hoping you
  caught every reference.
- **Duplication** — YAML has limited reuse mechanisms, so common patterns get copy-pasted between workflows.
- **Stringly-typed expressions** — `${{ }}` and `$( )` expressions are opaque strings to your editor.

Invex.StructuredText solves these problems by letting you define your pipelines as **C# object graphs**:

- Full IntelliSense and compile-time validation for every property
- Models closely follow the official schemas (GitHub workflow schema, Dependabot v2 schema, Azure Pipelines YAML
  schema)
- A single, platform-agnostic **expression system** that compiles down to GitHub's `${{ }}` syntax or Azure DevOps
  macro/runtime expression syntax
- Deterministic, cleanly formatted YAML output that is easy to diff and review

## The packages

| Package                                      | What it provides                                                                                        |
|----------------------------------------------|---------------------------------------------------------------------------------------------------------|
| `Invex.StructuredText`                       | The core: `StructuredTextWriter` (indentation-aware text writer) and the `TextExpression` system        |
| `Invex.StructuredText.GithubActions`         | `GithubActionWriter`, `DependabotConfigWriter`, `GithubExpressionFormatter`, and full model types       |
| `Invex.StructuredText.AzureDevopsPipelines`  | `DevopsPipelineWriter`, `DevopsExpressionFormatter`, and full model types                               |

Both platform packages depend on the core package, so installing either one gives you everything you need.

## A taste

```csharp
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
                new Step.UsesStep { Uses = new RawExpression("actions/checkout@v4") },
                new Step.RunStep { Run = [new RawExpression("dotnet build -c Release")] },
            ],
        },
    ],
};

var writer = new GithubActionWriter();
writer.Write(workflow);

File.WriteAllText(".github/workflows/ci.yml", writer.TextWriter.ToString());
```

## Where to go next

- [Getting Started](getting-started.md) — install the packages and generate your first file
- [Expressions](expressions.md) — learn the unified expression system
- [GitHub Actions](github-actions.md) — workflow generation in depth
- [Dependabot](dependabot.md) — `dependabot.yml` generation
- [Azure DevOps Pipelines](azure-devops-pipelines.md) — pipeline generation in depth
- [Architecture](architecture.md) — how the pieces fit together

