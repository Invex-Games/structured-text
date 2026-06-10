# Expressions

The expression system (namespace `Invex.StructuredText.Expressions`) is the most distinctive feature of this library.
Instead of embedding platform-specific expression strings (`${{ github.ref }}`, `$(Build.SourceBranch)`, …) directly in
your models, you build **platform-agnostic expression trees** out of `TextExpression` records. A platform-specific
formatter then renders the tree into the correct syntax for the target file.

The same expression can produce:

- `${{ contains(github.ref, 'release') }}` for GitHub Actions, via `GithubExpressionFormatter`
- `contains(variables['Build.SourceBranch'], 'release')` for Azure DevOps, via `DevopsExpressionFormatter`

## The building blocks

### Value expressions

| Expression                  | Meaning                                       | GitHub rendering |
|-----------------------------|-----------------------------------------------|------------------|
| `RawExpression("text")`     | Verbatim text, written exactly as-is          | `text`           |
| `StringExpression("text")`  | A string *literal* inside an expression       | `'text'`         |
| `NumberExpression(3.14)`    | A numeric literal                             | `3.14`           |
| `BooleanExpression(true)`   | A boolean literal                             | `true`           |
| `NullExpression()`          | Null / empty                                  | *(empty)*        |

> [!IMPORTANT]
> `RawExpression` vs `StringExpression` is the key distinction: `Raw` is uninterpreted text (use it for plain YAML
> values and context references like `github.ref`); `String` is a quoted string literal *within* an expression.

Implicit conversions make most of this invisible. `TextExpression` converts implicitly from `string` (to
`RawExpression`), `bool`, and all numeric types:

```csharp
TextExpression name = "build";       // RawExpression("build")
TextExpression count = 3;            // NumberExpression(3)
TextExpression flag = true;          // BooleanExpression(true)
```

The static `TextExpressions` class offers explicit factories: `TextExpressions.Raw(...)`, `TextExpressions.From(...)`
(string → `StringExpression`, bool/number → typed literal), `TextExpressions.True`, `TextExpressions.False`,
`TextExpressions.Null`.

### Evaluate — entering “expression context”

Most YAML values are plain text; expressions must be explicitly wrapped to be evaluated. `Evaluate()` produces an
`EvaluateExpression`, which GitHub renders as `${{ ... }}`:

```csharp
TextExpression runNumber = new RawExpression("github.run_number").Evaluate();
// GitHub: ${{ github.run_number }}
```

Condition-like properties (`If` on jobs/steps, `Condition` on Azure DevOps steps) are already in expression context, so
you usually don't need `Evaluate()` there.

### Property and index access

```csharp
TextExpression github = "github";

var refName = github["ref_name"];          // github.ref_name (PropertyAccessExpression)
var firstLabel = github["event"]["labels"][0]; // github.event.labels[0] (IndexAccessExpression)
var matrixOs = new RawExpression("matrix")[new StringExpression("os")]; // matrix['os']
```

## Logic operators

All comparison and logic combinators are available as fluent methods *and* C# operators:

```csharp
TextExpression branch = new RawExpression("github.ref");

var isMain = branch.EqualToString("refs/heads/main");   // github.ref == 'refs/heads/main'
var notMain = branch.NotEqualToString("refs/heads/main");

var both = isMain & new RawExpression("success()");     // ... && success()
var either = isMain | notMain;                          // ... || ...
var negated = !isMain;                                  // !(...)

var ordered = new NumberExpression(5).GreaterThan(3);   // 5 > 3
```

Available combinators: `And` / `&`, `Or` / `|`, `Not` / `!`, `EqualTo`, `NotEqualTo`, `LessThan`, `GreaterThan`,
`LessThanOrEqualTo`, `GreaterThanOrEqualTo` (each with `…String` variants that quote the right-hand side as a string
literal).

## Functions

```csharp
TextExpression branch = new RawExpression("github.ref");

branch.Contains("release");        // contains(github.ref, 'release')
branch.StartsWith("refs/tags/");   // startsWith(github.ref, 'refs/tags/')
branch.EndsWith("-rc");            // endsWith(github.ref, '-rc')
branch.Coalesce("fallback");       // coalesce(github.ref, 'fallback')
branch.Join(", ");                 // join(github.ref, ', ')
branch.ToJson();                   // toJSON(github.ref)

new RawExpression("'**/*.csproj'").HashFiles(); // hashFiles(...) (GitHub-only)
```

Reversed-argument helpers are provided where it reads better: `ContainedIn`, `IsStartOf`, `IsEndOf`.

## String interpolation with `Format`

`TextExpressions.Format` accepts an interpolated string whose holes can be other expressions. Literal parts become a
format string; holes become arguments:

```csharp
var version = new RawExpression("github.run_number").Evaluate();

var releaseName = TextExpressions.Format($"Release-{version}");
// GitHub: format('Release-{0}', ${{ github.run_number }})
```

You can also concatenate with `+`, or use `ConcatExpression` for raw concatenation:

```csharp
var combined = TextExpressions.Concat([new RawExpression("v"), version]);
```

## Workflow-run expressions

These records reference outputs and outcomes of other steps/jobs, and each platform formatter renders them with the
appropriate context path:

| Expression                                | Purpose                                            |
|-------------------------------------------|----------------------------------------------------|
| `StepOutputExpression`                    | Output of a step (`steps.<id>.outputs.<name>`)     |
| `StepOutcomeExpression`                   | Outcome of a step (`steps.<id>.outcome`)           |
| `StepOutcomeTypeExpression`               | An outcome literal: success/failure/cancelled/skipped |
| `TargetOutputExpression`                  | Output of a job/stage (`needs.<job>.outputs.<name>`) |
| `TargetOutcomeExpression`                 | Outcome of a job/stage                             |
| `TargetOutcomeTypeExpression`             | An outcome literal for jobs/stages                 |

```csharp
var artifact = new StepOutputExpression
{
    StepName = "build",
    OutputName = "artifact-path",
};

var buildSucceeded = new StepOutcomeExpression { StepName = "build" }
    .EqualTo(new StepOutcomeTypeExpression { Type = StepOutcomeTypeExpression.OutcomeType.Success });
```

## Typed wrappers: `WorkflowExpression<T>` and collections

Many model properties are declared as `WorkflowExpression<T>` rather than plain `TextExpression`. This is a
zero-overhead struct wrapper whose type parameter **documents the expected result type** (`string`, `bool`,
`double`, …). It converts implicitly to and from `TextExpression`, so you can assign expressions or strings directly:

```csharp
// DisplayName is WorkflowExpression<string>
DisplayName = new RawExpression("Build everything"),

// Condition is WorkflowExpression<bool>
Condition = new RawExpression("succeeded()"),
```

Similarly:

- `TextExpressionCollection` — a list of expressions with implicit conversions from `string`, `string[]`,
  `List<string>`, `TextExpression`, `TextExpression[]`, and `List<TextExpression>`. This is why you can write
  `Run = ["dotnet build"]` or `Branches = ["main"]`.
- `WorkflowExpressionCollection<T>` — the typed equivalent.
- `Cast<TTo>()` — reinterprets an expression's compile-time result type without changing the tree.

## Platform-specific expressions

### GitHub Actions (`GithubExpressionFormatter`)

| Expression                  | Output                  |
|-----------------------------|-------------------------|
| `EvaluateExpression`        | `${{ inner }}`          |
| `AndExpression(a, b)`       | `a && b`                |
| `ContainsExpression`        | `contains(a, b)`        |
| `FormatExpression`          | `format('...', args)`   |
| `HashFilesExpression`       | `hashFiles(...)`        |

### Azure DevOps (`DevopsExpressionFormatter`)

Azure DevOps has multiple expression syntaxes, and the package provides explicit wrappers for them:

| Expression                          | Output                |
|-------------------------------------|-----------------------|
| `DevopsMacroExpression(variable)`   | `$(variableName)`     |
| `DevopsRuntimeExpression(expr)`     | `$[ expression ]`     |

```csharp
using Invex.StructuredText.AzureDevopsPipelines;

var buildId = new DevopsMacroExpression(new RawExpression("Build.BuildId"));
// renders as: $(Build.BuildId)

var runtimeCondition = new DevopsRuntimeExpression(
    new RawExpression("variables.isMain").EqualToString("true"));
// renders as: $[ eq(variables.isMain, 'true') ]
```

## Writing your own formatter

To target a new platform, derive from `TextExpressionFormatter` and implement
`protected override TextExpression? Resolve(TextExpression expression)`. The base class handles `RawExpression`,
`ConcatExpression`, and `CastExpression<T>` unwrapping; your `Resolve` method rewrites every other expression node into
simpler nodes (ultimately `RawExpression`s). Returning `null` for an unrecognized node produces a clear
`InvalidOperationException` naming the unhandled expression.

