# Expressions

The expression API (namespace `Invex.StructuredText.Expressions`) lets you write conditions, references and
interpolated values in C# instead of hand-writing `${{ … }}` or `$( … )` strings. You build a small typed tree, and the
platform writer prints it in that platform's syntax. Read this page whenever a value in your model is a condition, a
reference to a secret or another job's output, or text built from other values.

```csharp
using Invex.StructuredText.Expressions;
using Invex.StructuredText.GithubActions;

BoolExpr publish = Tex.GitHub.EventName.Eq("push") && !Tex.GitHub.Ref.StartsWith("refs/tags/");
// if: github.event_name == 'push' && !startsWith(github.ref, 'refs/tags/')
```

## Start with `Tex`

Everything that creates an expression from scratch is a member of the static class `Tex`. Type `Tex.` and
IntelliSense lists all of it:

| Group | Members |
|---|---|
| References | `Secret(name)`, `Variable(name)`, `Input(name)`, `Matrix(name)`, `StepOutput(step, output)`, `JobOutput(job, output, step?)`, `JobResult(job)`, `Result(RunResult)` |
| Status checks | `Success()`, `Always()`, `Failure()`, `Cancelled()` |
| Functions | `All(conditions)`, `Any(conditions)`, `Format(format, args)`, `HashFiles(patterns)`, `FromJson(json)`, `Null`, `Join(separator, templates)` |
| Escape hatch | `Raw(text)` |
| Platform contexts | `Tex.GitHub` (from `Invex.StructuredText.GithubActions`), `Tex.DevOps` (from `Invex.StructuredText.AzureDevopsPipelines`) |

`Tex.GitHub` and `Tex.DevOps` are C# 14 extension members, so they appear once you reference the platform package and
import its namespace. Libraries built on top can add their own the same way (Atom adds `Tex.ParamOutput`).

Once you have an expression, its own members do the rest: `Eq`, `Contains`, `StartsWith`, the `&&`, `||` and `!`
operators, and indexers on JSON values.

> [!TIP]
> Add `using static Invex.StructuredText.Expressions.Tex;` to drop the prefix and write `Secret("TOKEN")`,
> `Success()` or `GitHub.Ref`.

## Expression types

Each expression has a result type, and the compiler only offers the operations that make sense for it:

| Type | What it is | Operations |
|---|---|---|
| `BoolExpr` | A condition | `&&`, `\|\|`, `!`, `Eq`, `Ne` |
| `StringExpr` | A string value | `Eq`, `Ne`, `Contains`, `StartsWith`, `EndsWith`, `In(collection)` |
| `NumberExpr` | A number | `Eq`, `Ne`, `Lt`, `Le`, `Gt`, `Ge` |
| `JsonExpr` | An object or array | `["name"]`, `[index]`, `AsString()`, `AsBool()`, `AsNumber()`, `Contains(element)`, `Join`, `ToJson` |

All four derive from `Expr`. C# literals convert implicitly: `"push"` becomes a quoted string literal, `100` a number
literal and `true` a boolean literal, so `Tex.GitHub.EventName.Eq("push")` prints `github.event_name == 'push'`.
Quotes inside literals are escaped for you.

## Conditions

Conditions are `BoolExpr` values combined with the C# operators. Parentheses are added only where precedence needs
them, and the platform decides the spelling:

```csharp
var isPush = Tex.GitHub.EventName.Eq("push");
var onMain = Tex.GitHub.Ref.Eq("refs/heads/main");
var isTag = Tex.GitHub.Ref.StartsWith("refs/tags/");
var isBot = Tex.GitHub.Actor.Eq("dependabot[bot]");

If = isPush && onMain,
// github.event_name == 'push' && github.ref == 'refs/heads/main'

If = (isPush || isTag) && !isBot,
// (github.event_name == 'push' || startsWith(github.ref, 'refs/tags/')) && !(github.actor == 'dependabot[bot]')

If = Tex.All([isPush, onMain, Tex.Success()]),
// github.event_name == 'push' && github.ref == 'refs/heads/main' && success()

If = Tex.Always() && Tex.JobResult("test").Eq(Tex.Result(RunResult.Failure)),
// always() && needs.test.result == 'failure'
```

Model properties that hold a condition (`If` on GitHub jobs and steps, `Condition` in Azure DevOps) are typed
`BoolExpr`, so anything else is a compile error. `Tex.All` and `Tex.Any` take any number of conditions, which is handy
when you build them in a loop.

## Values and templates

Every other expression-capable property (names, `run` lines, `env` values, `with` inputs, outputs) is a `Template`. A
`Template` is literal text with expressions embedded in it, and you create one with an ordinary string or an
interpolated string:

```csharp
Name = "Build",                                        // Build
Env = new Dictionary<string, Template>
{
    ["NUGET_API_KEY"] = Tex.Secret("NUGET_API_KEY"),   // ${{ secrets.NUGET_API_KEY }}
},
Run = [$"dotnet build -p:Version=1.0.{Tex.GitHub.RunNumber}"],
// dotnet build -p:Version=1.0.${{ github.run_number }}

Group = $"{Tex.GitHub.Workflow}-{Tex.GitHub.Ref}",    // ${{ github.workflow }}-${{ github.ref }}
```

Plain strings stay literal text, and expressions in the holes are wrapped in the platform's delimiters. Holes that are
plain C# values (strings, numbers, `bool`) are written as text. `Tex.Join` joins several templates with a separator:

```csharp
Tex.Join("-", ["build", Tex.Matrix("os"), Tex.Matrix("framework")])
// build-${{ matrix.os }}-${{ matrix.framework }}
```

> [!WARNING]
> A conditional that mixes an interpolated string with a plain string needs a `(Template)` cast on the interpolated
> branch: `Name = isNightly ? (Template)$"Nightly {Tex.GitHub.RunNumber}" : "Release"`. Without it, C# builds a
> `string` for the ternary and the expression is written as plain text (`Nightly github.run_number`) instead of
> `Nightly ${{ github.run_number }}`.

## References

| C# | GitHub Actions | Azure DevOps |
|---|---|---|
| `Tex.Secret("TOKEN")` | `secrets.TOKEN` | `$(TOKEN)` |
| `Tex.Variable("REGION")` | `vars.REGION` | `$(REGION)` |
| `Tex.Input("dry-run")` | `inputs.dry-run` | `${{ parameters['dry-run'] }}` |
| `Tex.Matrix("os")` | `matrix.os` | `$(os)` |
| `Tex.StepOutput("version", "semver")` | `steps.version.outputs.semver` | `$(version.semver)` |
| `Tex.JobOutput("setup", "version", "step")` | `needs.setup.outputs.version` | `$[ dependencies.setup.outputs['step.version'] ]` |
| `Tex.JobResult("test")` | `needs.test.result` | `dependencies.test.result` |
| `Tex.Result(RunResult.Failure)` | `'failure'` | `'Failed'` |

The GitHub column shows the bare expression; inside a template it is wrapped in `${{ }}`. The Azure DevOps column shows
how each reference is embedded in a template, because Azure DevOps has a different syntax for each kind of value. The
`step` argument of `JobOutput` is only used by Azure DevOps, where job outputs are addressed through the step that set
them.

## Platform contexts

`Tex.GitHub` is a catalogue of the GitHub Actions contexts, generated from the documented context list with XML docs
on every member:

```csharp
Tex.GitHub.Ref                          // github.ref (StringExpr)
Tex.GitHub.Event.PullRequest.Number     // github.event.pull_request.number (NumberExpr)
Tex.GitHub.Event.PullRequest.Draft      // github.event.pull_request.draft (BoolExpr)
Tex.GitHub.Runner.Os                    // runner.os
Tex.GitHub.Vars["REGION"]               // vars.REGION
Tex.GitHub.Event["label"]["name"]       // github.event.label.name (JsonExpr)
```

`Tex.DevOps` covers the common Azure DevOps predefined variables (`Tex.DevOps.Build.SourceBranch`,
`Tex.DevOps.System.AccessToken`, `Tex.DevOps.Agent.Os`, …) and runtime expressions; see
[Azure DevOps Pipelines](azure-devops-pipelines.md#expressions-in-azure-devops).

Anything the catalogue doesn't cover can be reached through a `JsonExpr` indexer. Call `AsString()`, `AsBool()` or
`AsNumber()` when you need to compare the result:

```csharp
Tex.GitHub.Event["label"]["name"].AsString().Eq("release")
// github.event.label.name == 'release'

Tex.GitHub.Event["label"]["name"].AsString().In(Tex.FromJson(Tex.Variable("RELEASE_LABELS")))
// contains(fromJSON(vars.RELEASE_LABELS), github.event.label.name)
```

## Functions

```csharp
Tex.Format("{0}-nuget-{1}", Tex.GitHub.Runner.Os, Tex.HashFiles("**/*.csproj"))
// ${{ format('{0}-nuget-{1}', runner.os, hashFiles('**/*.csproj')) }}

Tex.GitHub.Event.ToJson()
// ${{ toJSON(github.event) }}

Tex.GitHub.Ref.EndsWith("-rc")
// endsWith(github.ref, '-rc')
```

Interpolated templates cover most uses of `format`, so reach for `Tex.Format` only when the platform has to do the
formatting at run time.

## The escape hatch: `Tex.Raw`

`Tex.Raw(text)` inserts expression text verbatim. It is a `BoolExpr` that converts implicitly to the other expression
types, so it works anywhere. When you combine it with other expressions it is parenthesized as a unit, so the result
keeps its meaning:

```csharp
!Tex.Raw("github.ref == 'main'")                // !(github.ref == 'main')
Tex.Raw("github.event.forced") && Tex.Success() // github.event.forced && success()
```

Prefer the typed members where they exist: raw text isn't checked, escaped or translated between platforms.

## Behavioral notes

- **Conditions starting with `!`.** A YAML value can't start with `!` (it is a tag), so the GitHub writer wraps such a
  condition in `${{ }}`: `if: ${{ !contains(github.ref, '-') }}`.
- **Literals are quoted and escaped.** `Tex.GitHub.Ref.Eq("it's")` prints `github.ref == 'it''s'`.
- **Expressions are records.** They compare structurally, and `DescendantsAndSelf()` walks the tree, which is how
  tools find the jobs a condition depends on (look for `JobOutputRef` nodes).
- **`ToString()` is for debugging.** It prints a neutral form; use the platform printer to get real output.
- **Unsupported nodes fail loudly.** A printer throws `NotSupportedException` for a node it can't express, such as
  `Tex.FromJson` in Azure DevOps.

## Printing expressions yourself

The writers print expressions for you. To print one directly, for a test or a custom writer, use the platform's
printer:

```csharp
GithubExpressionPrinter.Instance.Print(expression);        // the bare expression
GithubExpressionPrinter.Instance.Render(template);         // a template, with ${{ }} around expressions
GithubExpressionPrinter.Instance.PrintCondition(condition); // an if: value, wrapped only when it must be
DevopsExpressionPrinter.Instance.Render(template);
```

To target another platform, derive from `ExpressionPrinter`. The base class handles precedence, literal quoting,
member access, casts and raw text. You implement `WrapEmbedded` (the template delimiters), `PrintReference` (how
`Tex.Secret`, `Tex.JobOutput` and the other references are spelled) and `PrintResult`, and override the virtual members
such as `FunctionName` or `PrintLogical` where the syntax differs.

## See also

- [GitHub Actions Workflows](github-actions.md#referencing-outputs-between-steps-and-jobs): expressions in workflow
  models
- [Azure DevOps Pipelines](azure-devops-pipelines.md#expressions-in-azure-devops): Azure DevOps expression syntax
- [Samples](https://github.com/Invex-Games/structured-text/blob/main/samples/README.md): runnable examples with
  their output
- <xref:Invex.StructuredText.Expressions.Tex>, <xref:Invex.StructuredText.Expressions.Expr>,
  <xref:Invex.StructuredText.Expressions.Template>, <xref:Invex.StructuredText.Expressions.ExpressionPrinter>
