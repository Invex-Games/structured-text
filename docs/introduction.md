# Introduction

**Invex.StructuredText** is a family of strongly-typed C# libraries for generating structured YAML text files —
specifically **GitHub Actions workflows**, **Dependabot configurations**, and **Azure DevOps Pipelines**. This page
explains why you would generate CI/CD YAML from C#, when the library is (and is not) a good fit, and the concepts the
rest of the documentation builds on.

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

## When to use it

Invex.StructuredText is a good fit when:

- **You maintain several workflows or pipelines** that share jobs, steps, or conventions, and want to reuse them with
  ordinary C# functions and constants.
- **You maintain many repositories** and want their CI configuration generated from one shared definition.
- **You already have a C# build tool** (for example an `_atom/` or `_build/` console project) that can regenerate the
  files and check them in.
- **Expressions are getting complicated**, and you want conditions and output references checked by the compiler
  instead of discovered at run time.

It is not the right tool when:

- **You need to read or modify existing YAML.** The library is write-only; it does not parse workflow files.
- **You have one small workflow.** Writing the YAML by hand is simpler than adding a generator project.
- **You target another CI system.** Only GitHub Actions, Dependabot, and Azure DevOps Pipelines have models, although
  the core writer and expression system can be used to build your own (see [Architecture](architecture.md)).

## Key concepts at a glance

| Concept | What it is |
| --- | --- |
| Model | An object graph of records rooted at <xref:Invex.StructuredText.GithubActions.GithubActionModel.GithubAction>, <xref:Invex.StructuredText.GithubActions.DependabotConfigModel.Model.DependabotConfig>, or <xref:Invex.StructuredText.AzureDevopsPipelines.DevopsPipelineModel.Pipeline.DevopsPipeline>. |
| Union variant | Schema "one of" shapes (a step is `run:` *or* `uses:`) are discriminated unions; construct the nested variant you need, such as `new Step.RunStep { ... }`. |
| Writer | <xref:Invex.StructuredText.GithubActions.GithubActionWriter>, <xref:Invex.StructuredText.GithubActions.DependabotConfigWriter>, or <xref:Invex.StructuredText.AzureDevopsPipelines.DevopsPipelineWriter> walks a model and emits YAML. |
| `StructuredTextWriter` | The indentation-aware buffer every writer writes into; read the result with `writer.TextWriter.ToString()`. See [StructuredTextWriter](structured-text-writer.md). |
| Expression | A <xref:Invex.StructuredText.Expressions.TextExpression> tree (values, functions, logic, step and job outputs) that a platform formatter renders to that platform's syntax. See [Expressions](expressions.md). |
| `RawExpression` vs `StringExpression` | Verbatim text versus a quoted string literal inside an expression; plain `string` values convert to `RawExpression`. |

## Design principles

- **Model the schema, not a DSL.** Model records mirror the official YAML schemas and their property names map
  mechanically to YAML keys, so platform documentation applies directly.
- **Invalid shapes do not compile.** Discriminated unions rule out combinations the schema forbids, and `required`
  properties force a decision on schema-significant values, even when that decision is `null`.
- **Write once, render per platform.** Expressions are platform-agnostic trees; only the formatter knows the target
  syntax.
- **Deterministic and write-only.** The same model always produces the same text, so you can regenerate in CI and fail
  on `git diff --exit-code`.
- **Small, composable layers.** Platform packages are built on a core writer and expression system that you can reuse
  for your own generators.

## The packages

| Package                                      | What it provides                                                                                        |
|----------------------------------------------|---------------------------------------------------------------------------------------------------------|
| `Invex.StructuredText`                       | The core: `StructuredTextWriter` (indentation-aware text writer) and the `TextExpression` system        |
| `Invex.StructuredText.GithubActions`         | `GithubActionWriter`, `DependabotConfigWriter`, `GithubExpressionFormatter`, and full model types       |
| `Invex.StructuredText.AzureDevopsPipelines`  | `DevopsPipelineWriter`, `DevopsExpressionFormatter`, and full model types                               |

Both platform packages depend on the core package, so installing either one gives you everything you need.

## Next steps

- [Getting Started](getting-started.md) — install the packages and generate your first file
- [Expressions](expressions.md) — learn the unified expression system
- [GitHub Actions Workflows](github-actions.md) — workflow generation in depth
- [Dependabot Configuration](dependabot.md) — `dependabot.yml` generation
- [Azure DevOps Pipelines](azure-devops-pipelines.md) — pipeline generation in depth
- [Architecture](architecture.md) — how the pieces fit together
