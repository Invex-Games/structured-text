# FAQ

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

.NET 8.0, 9.0, and 10.0. Generators are typically run as small console apps on any OS.

## Usage

### Why are some properties `required` and nullable at the same time?

Model records deliberately force you to make a decision for schema-significant properties (e.g. `On.Push.Branches`).
`required ... { get; init; }` with a nullable type means "you must say *something*, even if that something is `null`".
This keeps generated YAML intentional rather than accidentally omitting filters.

### Why both `RawExpression` and `StringExpression`?

`RawExpression` is verbatim text — used for plain YAML values and references like `github.ref`. `StringExpression` is
a quoted string *literal inside an expression* — `'release'` in `contains(github.ref, 'release')`. See
[Expressions](expressions.md).

### How do I write `${{ ... }}` into a value?

Wrap the expression with `.Evaluate()`:

```csharp
Env = new Dictionary<string, TextExpression>
{
    ["SHA"] = new RawExpression("github.sha").Evaluate(), // ${{ github.sha }}
};
```

Condition properties (`If`, `Condition`) are already in expression context — no `Evaluate()` needed there.

If you really just want literal text, plain strings pass through untouched: `Token = "${{ secrets.TOKEN }}"` works for
Dependabot configs, which are string-based.

### Can I reuse one writer for several files?

Writers accumulate output. Either create a fresh writer per file or call `writer.TextWriter.Reset()` between
documents.

### How do I share common steps/jobs between workflows?

That's the main payoff of generating from C#: use ordinary functions, constants, and composition.

```csharp
static Step CheckoutStep() =>
    new Step.UsesStep { Uses = new RawExpression("actions/checkout@v4") };

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

### `InvalidOperationException: No writer found to handle expression ...`

You used an expression node the target platform's formatter doesn't support (for example, a `DevopsMacroExpression` in
a GitHub Actions workflow, or a custom `TextExpression` subclass). Use the expression types appropriate to the target,
or extend the formatter.

### My condition rendered as plain text instead of being evaluated

Plain values are written verbatim. For value-position properties (env vars, names, inputs), wrap the expression in
`.Evaluate()` to get `${{ ... }}`. Condition-position properties are evaluated automatically.

### Where is the API reference?

See the [API Reference](../api/index.md), generated from the XML documentation comments in the source.

