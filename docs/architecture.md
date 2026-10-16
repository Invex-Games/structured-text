# Architecture

This page explains how the three packages fit together and the design decisions behind them. Useful if you want to
extend the library, contribute, or build your own generator on top of the core.

## Package layering

```text
┌─────────────────────────────────┐  ┌──────────────────────────────────────┐
│ Invex.StructuredText            │  │ Invex.StructuredText                 │
│ .GithubActions                  │  │ .AzureDevopsPipelines                │
│                                 │  │                                      │
│ • GithubActionWriter            │  │ • DevopsPipelineWriter               │
│ • DependabotConfigWriter        │  │ • DevopsExpressionPrinter            │
│ • GithubExpressionPrinter       │  │ • Tex.DevOps (DevopsContexts)        │
│ • Tex.GitHub (GitHubContexts)   │  │ • DevopsRuntimeExpr                  │
│ • GithubActionModel/*           │  │ • DevopsPipelineModel/*              │
│ • DependabotConfigModel/*       │  │                                      │
└───────────────┬─────────────────┘  └──────────────────┬───────────────────┘
                │                                       │
                └───────────────────┬───────────────────┘
                                    ▼
                  ┌─────────────────────────────────────┐
                  │ Invex.StructuredText (core)         │
                  │                                     │
                  │ • StructuredTextWriter              │
                  │ • Tex (the expression entry point)  │
                  │ • Expr, BoolExpr, StringExpr, …     │
                  │ • Template                          │
                  │ • ExpressionPrinter (base)          │
                  └─────────────────────────────────────┘
```

## Core (`Invex.StructuredText`)

### `StructuredTextWriter`

A minimal, indentation-aware text writer. `WriteSection` returns an `IDisposable` scope that decrements the indent on
dispose, so the structure of the generating C# code mirrors the structure of the generated YAML. A version counter
makes stale scopes harmless after `Reset()`. See [StructuredTextWriter](structured-text-writer.md).

### The expression tree

`Expr` is an abstract record with four typed roots: `BoolExpr`, `StringExpr`, `NumberExpr` and `JsonExpr`. The result
type decides which operations the compiler offers, so `If = Tex.GitHub.Ref` (a string) does not compile. The concrete
nodes are grouped by concern:

- **Literals**: `StringLiteral`, `NumberLiteral`, `BoolLiteral`, `NullLiteral`, created implicitly from C# literals
- **Context paths**: `StringContext`, `BoolContext`, `NumberContext`, `JsonContext`, used by the generated catalogues
  such as `Tex.GitHub`
- **Access and casts**: `MemberAccessExpr`, `IndexAccessExpr`, `StringCast`, `BoolCast`, `NumberCast`
- **Logic**: `ComparisonExpr`, `LogicalExpr` (flattened, so `a && b && c` is one node), `NotExpr`
- **Functions**: `BoolFunction`, `StringFunction`, `JsonFunction`, named with the constants in `FunctionNames`
- **References**: `SecretRef`, `VariableRef`, `InputRef`, `MatrixRef`, `StepOutputRef`, `JobOutputRef`,
  `JobResultRef` and `ResultLiteral`, which each platform spells its own way
- **Raw text**: `RawExpr` and its typed siblings, for the `Tex.Raw` escape hatch

Because they are records, expressions compare structurally, and `DescendantsAndSelf()` walks the tree. Atom uses that
to find the jobs a condition depends on.

`Tex` is the single entry point that creates them. Platform packages and libraries add members to it with C# 14
`extension(Tex)` blocks, which is how `Tex.GitHub`, `Tex.DevOps` and Atom's `Tex.ParamOutput` appear on the same type.

### `Template`

`Template` is a sequence of literal text and embedded expressions. It is an interpolated string handler, so
`$"v{Tex.GitHub.RunNumber}"` builds one directly, and it converts implicitly from `string`, `Expr`, `bool` and numbers.
Every model property that can hold an expression, other than conditions, is a `Template`.

### `ExpressionPrinter`

The abstract printer renders an `Expr` (`Print`) or a `Template` (`Render`). The base class handles operator
precedence (adding parentheses only where needed), literal quoting and escaping, member and index access, casts and
raw text. A platform printer supplies what differs:

1. `WrapEmbedded`: the delimiters around an expression inside a template, such as `${{ … }}`.
2. `PrintReference` and `PrintResult`: how references and job results are spelled.
3. Optional overrides: `FunctionName`, `PrintComparison`, `PrintLogical`, `PrintNot`, `RenderEmbedded` and others.
   Azure DevOps overrides the logic members to print `and(…)`, `eq(…)`, and `RenderEmbedded` to choose between
   `$(…)`, `${{ … }}` and `$[ … ]`.

A node a printer can't express throws `NotSupportedException` rather than producing invalid YAML.

## Platform packages

Each platform package contains:

- **Model types** — records that mirror the official YAML schema for the platform. Conditions are `BoolExpr`, and
  almost every other scalar property is a `Template`, so any value can contain expressions.
- **A writer** — walks the model and emits YAML through a `StructuredTextWriter`, calling the platform expression
  printer for every expression-valued property.
- **An expression printer** — `GithubExpressionPrinter` or `DevopsExpressionPrinter`, an `ExpressionPrinter` for the
  platform's syntax.
- **A context catalogue** — `Tex.GitHub` or `Tex.DevOps`. The GitHub catalogue is generated at build time from
  `Contexts/github.contexts` by `Invex.StructuredText.Generators`.

### GitHub Actions (`Invex.StructuredText.GithubActions`)

- **`GithubActionWriter`** — serializes a `GithubAction` model to workflow YAML.
- **`DependabotConfigWriter`** — serializes a `DependabotConfig` model to `dependabot.yml`.
- **`GithubExpressionPrinter`** — prints expressions using GitHub's syntax, and `${{ }}` inside templates.
- **Model types** — strongly-typed records for all GitHub Actions concepts (triggers, jobs, steps, matrix, permissions,
  etc.) in `GithubActionModel`, and for Dependabot in `DependabotConfigModel.Model`.

### Azure DevOps Pipelines (`Invex.StructuredText.AzureDevopsPipelines`)

- **`DevopsPipelineWriter`** — serializes a `DevopsPipeline` model to pipeline YAML.
- **`DevopsExpressionPrinter`** — prints expressions using Azure DevOps syntax.
- **`DevopsRuntimeExpr`** — created by `Tex.DevOps.Runtime(…)`, embeds an expression as a `$[ … ]` runtime
  expression.
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
| New target platform                  | Reference the core, define model records, write an `ExpressionPrinter` subclass and a writer over `StructuredTextWriter` |
| New `Tex` member                     | Add an `extension(Tex)` block in your package, returning existing node types where you can |
| New context in the GitHub catalogue  | Add a line to `Contexts/github.contexts`; the generator adds the member and its XML docs  |
| Platform-specific expression syntax  | Follow the pattern of `DevopsRuntimeExpr`: define the record in the platform package and print it in that platform's printer (`PrintOther` or `RenderEmbedded`) |

## See also

- [StructuredTextWriter](structured-text-writer.md): the writer every generator is built on
- [Expressions](expressions.md#printing-expressions-yourself): writing your own printer
- [FAQ](faq.md): common questions about the model design
- <xref:Invex.StructuredText.StructuredTextWriter>, <xref:Invex.StructuredText.Expressions.Tex>,
  <xref:Invex.StructuredText.Expressions.Expr>, <xref:Invex.StructuredText.Expressions.ExpressionPrinter>
