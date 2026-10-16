# FAQ

Answers to common questions about what the library does, how to use the models and expressions, and how to fix
frequent problems. Check here when something behaves differently from what you expected.

## General

### What does this library actually do?

It turns strongly-typed C# object graphs into YAML text for GitHub Actions workflows, Dependabot configs, and Azure
DevOps Pipelines. You build models, a writer renders them, and you save the resulting string to a file.

### Does it parse existing YAML files?

No. The library is **write-only** by design — it generates YAML from C# models. If you have existing hand-written
workflows, port them to the model once and generate from then on.

### Does it run my pipelines?

No. It only produces the configuration files. GitHub/Azure DevOps execute them as usual once committed.

### Which platforms and frameworks are supported?

.NET 10.0. Generators are typically run as small console apps on any OS.

## Usage

### Why are some properties `required` and nullable at the same time?

Model records deliberately force you to make a decision for schema-significant properties (e.g. `On.Push.Branches`).
`required ... { get; init; }` with a nullable type means "you must say *something*, even if that something is `null`".
This keeps generated YAML intentional rather than accidentally omitting filters.

### Where do I start when writing an expression?

Type `Tex.`. Every way to create an expression is a member of that one class: references such as `Tex.Secret` and
`Tex.JobOutput`, status checks such as `Tex.Success()`, functions, and the platform contexts `Tex.GitHub` and
`Tex.DevOps`. Add `using static Invex.StructuredText.Expressions.Tex;` to drop the prefix. See
[Expressions](expressions.md).

### How do I write `${{ ... }}` into a value?

Put the expression in the value, on its own or in an interpolated string. The writer adds the delimiters:

```csharp
Env = new Dictionary<string, Template>
{
    ["SHA"] = Tex.GitHub.Sha,                       // ${{ github.sha }}
    ["TAG"] = $"v{Tex.JobOutput("setup", "version")}", // v${{ needs.setup.outputs.version }}
};
```

Condition properties (`If`, `Condition`) are written without delimiters, as the platforms expect.

### How do I compare against a string?

Pass a C# string: `Tex.GitHub.EventName.Eq("push")` prints `github.event_name == 'push'`. Literals are always quoted
and escaped, so there is no separate "string" variant of each comparison.

### What if the API doesn't cover something?

Index into a context (`Tex.GitHub.Event["label"]["name"]`), then call `AsString()`, `AsBool()` or `AsNumber()` to
compare it. For anything else, `Tex.Raw("...")` inserts expression text verbatim and works wherever an expression is
expected.

### Can I reuse one writer for several files?

Writers accumulate output. Either create a fresh writer per file or call `writer.TextWriter.Reset()` between
documents.

### How do I share common steps/jobs between workflows?

That's the main payoff of generating from C#: use ordinary functions, constants, and composition.

```csharp
static Step CheckoutStep() =>
    new Step.UsesStep { Uses = "actions/checkout@v6" };

static Job WithDefaults(Job job) =>
    job with { TimeoutMinutes = 30 };
```

### The union types — how do I pick a variant?

Construct the nested record: `new On.Push { ... }`, `new Step.RunStep { ... }`,
`new DevopsPipeline.DevopsPipelineWithStages { ... }`, `new Pool.PoolSpec { ... }`. The base type (`On`, `Step`, …) is
what model properties accept.

### Is the output deterministic?

Yes — same model, same text. That makes generated files diff-friendly, and lets you verify in CI that committed
workflow files match the generator (regenerate and `git diff --exit-code`).

## Troubleshooting

### `NotSupportedException` when writing a model

The platform printer met an expression it can't express, such as `Tex.FromJson` in Azure DevOps (which has no
`fromJSON`), `Tex.DevOps.Runtime(...)` in a GitHub workflow, or a custom `Expr` subclass. Use an expression the target
supports, or `Tex.Raw` with the platform's own syntax.

### My interpolated value lost its `${{ }}`

A conditional like `isNightly ? $"Nightly {Tex.GitHub.RunNumber}" : "Release"` has the type `string`, so C# formats
the expression as plain text before the writer sees it. Cast the interpolated branch:
`isNightly ? (Template)$"Nightly {Tex.GitHub.RunNumber}" : "Release"`.

### My condition is wrapped in `${{ }}` but the others aren't

That's intended. A YAML value can't start with `!`, so the GitHub writer wraps conditions that start with a negation:
`if: ${{ !contains(github.ref, '-') }}`.

### Where is the API reference?

See the [API Reference](../api/index.md), generated from the XML documentation comments in the source.

## See also

- [Expressions](expressions.md): `Tex`, conditions, templates and the `Tex.Raw` escape hatch
- [Getting Started](getting-started.md): the build, write, save pattern
- [Architecture](architecture.md): why the models are shaped the way they are
- <xref:Invex.StructuredText.Expressions.Tex>, <xref:Invex.StructuredText.Expressions.ExpressionPrinter>
