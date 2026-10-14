# Architecture

This page explains how the three packages fit together and the design decisions behind them. Useful if you want to
extend the library, contribute, or build your own generator on top of the core.

## Package layering

```text
┌─────────────────────────────────┐  ┌──────────────────────────────────────┐
│ Invex.StructuredText            │  │                                      │
│ .GithubActions                  │  │ Invex.StructuredText                 │
│                                 │  │ .AzureDevopsPipelines                │
│ • GithubActionWriter            │  │                                      │
│ • DependabotConfigWriter        │  │ • DevopsPipelineWriter               │
│ • GithubExpressionFormatter     │  │ • DevopsExpressionFormatter          │
│ • GithubActionModel/*           │  │ • DevopsMacroExpression              │
│ • DependabotConfigModel/*       │  │ • DevopsRuntimeExpression            │
│                                 │  │ • DevopsPipelineModel/*              │
└───────────────┬─────────────────┘  └──────────────────┬───────────────────┘
                │                                       │
                └───────────────────┬───────────────────┘
                                    ▼
                  ┌─────────────────────────────────────┐
                  │ Invex.StructuredText (core)          │
                  │                                     │
                  │ • StructuredTextWriter              │
                  │ • TextExpression hierarchy          │
                  │ • TextExpressionFormatter (base)    │
                  │ • TextExpressionCollection          │
                  │ • WorkflowExpression<T>             │
                  └─────────────────────────────────────┘
```

## Core (`Invex.StructuredText`)

### `StructuredTextWriter`

A minimal, indentation-aware text writer. `WriteSection` returns an `IDisposable` scope that decrements the indent on
dispose, so the structure of the generating C# code mirrors the structure of the generated YAML. A version counter
makes stale scopes harmless after `Reset()`. See [StructuredTextWriter](structured-text-writer.md).

### The expression tree

`TextExpression` is an abstract record with a closed-ish set of derived records grouped by concern:

- **Values** — `RawExpression`, `StringExpression`, `NumberExpression`, `BooleanExpression`, `NullExpression`
- **Accessors** — `PropertyAccessExpression`, `IndexAccessExpression`, `EvaluateExpression`
- **Logic** — `AndExpression`, `OrExpression`, `NotExpression`, `EqualExpression`, `NotEqualExpression`,
  comparison expressions
- **Functions** — `ContainsExpression`, `StartsWithExpression`, `EndsWithExpression`, `CoalesceExpression`,
  `FormatExpression`, `JoinExpression`, `ToJsonExpression`, `HashFilesExpression`, `ConcatExpression`
- **Workflow-run** — `StepOutputExpression`, `StepOutcomeExpression`, `TargetOutputExpression`,
  `TargetOutcomeExpression`, and outcome-type literals
- **Typing helpers** — `CastExpression<T>`, `WorkflowExpression<T>`, `WorkflowExpressionCollection<T>`

Because they are records, expressions have structural equality and readable `ToString()` output, which makes tests and
debugging straightforward.

Fluent builder methods (`Contains`, `EqualTo`, `And`, …), operators (`&`, `|`, `!`, `+`), implicit conversions from
primitives, and an interpolated-string handler (`TextExpressions.Format($"...")`) make tree construction ergonomic.

### `TextExpressionFormatter`

The abstract base class implements `Format(TextExpression?)` as a **rewrite loop**:

1. `null` → `null`; `RawExpression` → its value; `ConcatExpression` → concatenated formatted parts.
2. `CastExpression<T>` is unwrapped transparently.
3. Otherwise the platform-specific `Resolve(expression)` is called, which returns a *simpler* expression (often a
   `RawExpression` containing rendered text). The loop repeats until everything bottoms out in raw text.
4. If `Resolve` cannot handle a node, an `InvalidOperationException` names the offending expression.

This design means platform formatters only describe *one rewrite step per node type* and get recursion, casting, and
null-handling for free.

## Platform packages

Each platform package contains:

- **Model types** — records that mirror the official YAML schema for the platform. Almost every scalar property is a
  `TextExpression` / `WorkflowExpression<T>` so any value can be an expression.
- **A writer** — walks the model and emits YAML through a `StructuredTextWriter`, calling the platform expression
  formatter for every expression-valued property.
- **An expression formatter** — `GithubExpressionFormatter` or `DevopsExpressionFormatter`, implementing `Resolve` for
  the platform's syntax.

### GitHub Actions (`Invex.StructuredText.GithubActions`)

- **`GithubActionWriter`** — serializes a `GithubAction` model to workflow YAML.
- **`DependabotConfigWriter`** — serializes a `DependabotConfig` model to `dependabot.yml`.
- **`GithubExpressionFormatter`** — formats expressions using GitHub's `${{ }}` syntax.
- **Model types** — strongly-typed records for all GitHub Actions concepts (triggers, jobs, steps, matrix, permissions,
  etc.) in `GithubActionModel`, and for Dependabot in `DependabotConfigModel.Model`.

### Azure DevOps Pipelines (`Invex.StructuredText.AzureDevopsPipelines`)

- **`DevopsPipelineWriter`** — serializes a `DevopsPipeline` model to pipeline YAML.
- **`DevopsExpressionFormatter`** — formats expressions using Azure DevOps syntax.
- **`DevopsMacroExpression` / `DevopsRuntimeExpression`** — wrappers for the `$(...)` macro and `$[ ... ]` runtime
  expression syntaxes.
- **Model types** — strongly-typed records and unions for pipelines, stages, jobs, steps, resources, triggers,
  variables, deployment strategies, and more, under `DevopsPipelineModel.*`.

### Discriminated unions

YAML schemas are full of "one of" shapes (a step is `run:` *or* `uses:`; a pool is a name *or* a spec). These are
modeled as discriminated unions using the [Dunet](https://github.com/domn1995/dunet) source generator: an abstract
base record with nested variant records. You construct the variant you need (`new Step.RunStep { ... }`), and the
writers pattern-match on the variant to emit the right YAML shape. This rules out invalid combinations at compile
time.

### Schema fidelity

The GitHub workflow JSON schema and Dependabot v2 JSON schema are checked into the source tree alongside the models,
and the Azure DevOps models carry XML docs lifted from the official YAML schema reference. Property names map
mechanically to YAML keys (PascalCase → kebab-case / camelCase as appropriate per platform).

## Public API discipline

The libraries use:

- `[PublicAPI]` annotations (JetBrains.Annotations) on all public surface
- Public API surface snapshot tests (`PublicApiSurfaceTests` with verified `.txt` baselines in each test project), so
  any accidental API change shows up as a test diff
- Targets .NET 10

## Extending the library

| Goal                                 | How                                                                                      |
|--------------------------------------|-------------------------------------------------------------------------------------------|
| New target platform                  | Reference the core, define model records, write a `TextExpressionFormatter` subclass and a writer over `StructuredTextWriter` |
| New expression node                  | Derive a record from `TextExpression`; handle it in each formatter's `Resolve`            |
| Platform-specific expression syntax  | Follow the pattern of `DevopsMacroExpression` / `DevopsRuntimeExpression`: define the record in the platform package and resolve it in that platform's formatter |

## See also

- [StructuredTextWriter](structured-text-writer.md): the writer every generator is built on
- [Expressions](expressions.md#writing-your-own-formatter): writing your own formatter
- [FAQ](faq.md): common questions about the model design
- <xref:Invex.StructuredText.StructuredTextWriter>, <xref:Invex.StructuredText.Expressions.TextExpression>,
  <xref:Invex.StructuredText.Expressions.TextExpressionFormatter>
