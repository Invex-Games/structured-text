# StructuredTextWriter

`StructuredTextWriter` (in the `Invex.StructuredText` namespace) is the low-level, indentation-aware text writer that
all the YAML generators are built on. You only need it directly if you are writing your own generator or extending an
existing one — but understanding it helps explain how the higher-level writers produce their output.

## Basics

```csharp
using Invex.StructuredText;

var writer = new StructuredTextWriter(); // default indent size: 2 spaces

writer.WriteLine("name: CI");
writer.WriteLine();

Console.WriteLine(writer.ToString());
```

| Member                       | Description                                                                       |
|------------------------------|-----------------------------------------------------------------------------------|
| `Write(text, indent = true)` | Appends text without a newline; optionally skips the indent prefix                 |
| `WriteLine(text = null)`     | Appends a line at the current indent level (or a blank line)                       |
| `WriteSection(text = null)`  | Optionally writes a header line, then increases the indent; returns an `IDisposable` that restores it |
| `Reset(indentSize = 2)`      | Clears the buffer and resets indentation                                           |
| `Indent` / `IndentSize`      | Current indent level and the number of spaces per level                            |
| `StringBuilder`              | The underlying buffer                                                              |
| `ToString()`                 | Returns the accumulated text                                                       |

## Sections and scoped indentation

`WriteSection` is the heart of the API. It writes an (optional) header line and bumps the indent level. Disposing the
returned scope restores the previous level, so nesting maps naturally onto C# `using` blocks:

```csharp
var writer = new StructuredTextWriter();

using (writer.WriteSection("jobs:"))
{
    using (writer.WriteSection("build:"))
    {
        writer.WriteLine("runs-on: ubuntu-latest");

        using (writer.WriteSection("steps:"))
        {
            writer.WriteLine("- run: dotnet build");
        }
    }
}

Console.WriteLine(writer.ToString());
```

```yaml
jobs:
  build:
    runs-on: ubuntu-latest
    steps:
      - run: dotnet build
```

## Reset and versioning

`Reset()` clears the buffer, resets the indent, and increments an internal `Version`. Any section scopes that were
created before the reset become no-ops on dispose — they will not corrupt the indentation of content written after the
reset. This makes it safe to reuse a single writer across multiple documents:

```csharp
var writer = new StructuredTextWriter();
// ... write first document ...
var first = writer.ToString();

writer.Reset();
// ... write second document ...
var second = writer.ToString();
```

## Using it with the high-level writers

Each platform writer exposes its `StructuredTextWriter` via the `TextWriter` property:

```csharp
var actionWriter = new GithubActionWriter();
actionWriter.Write(workflow);

var yaml = actionWriter.TextWriter.ToString();
```

You can also supply your own instance (for example, to share one buffer or use a different indent size):

```csharp
var writer = new GithubActionWriter
{
    TextWriter = new StructuredTextWriter(indentSize: 4),
};
```

