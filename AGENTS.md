# Agent Instructions

Guidance for AI agents working in **Invex.StructuredText** — a family of strongly-typed C# libraries
for generating YAML for GitHub Actions workflows, Dependabot configurations, and Azure DevOps
Pipelines. Keep changes focused and defer to the linked docs for detail.

## What's in the repo

| Project | Role | Target frameworks |
|---------|------|-------------------|
| `Invex.StructuredText` | Core library: `StructuredTextWriter`, the typed expression API (`Tex`, `Expr`, `Template`) and `ExpressionPrinter` | `net10.0` |
| `Invex.StructuredText.GithubActions` | GitHub Actions workflow and Dependabot config generation | `net10.0` |
| `Invex.StructuredText.AzureDevopsPipelines` | Azure DevOps Pipelines YAML generation | `net10.0` |

Sources live under `src/`, tests under `tests/`, the Atom build definition under `_atom/`, and the
DocFX documentation site under `docs/`.

## Build & language specifics

- **.NET 10 SDK** is required. All library and test projects target `net10.0`.
- C# `LangVersion 14`, `ImplicitUsings` and `Nullable` enabled, `TreatWarningsAsErrors` on.
- Global usings live in each project's `_usings.cs` — add shared usings there, not per-file.
- `GenerateDocumentationFile` is on, so **all public members need XML doc comments**.
- Build and test the whole solution:

  ```shell
  dotnet build Invex.StructuredText.slnx
  dotnet test Invex.StructuredText.slnx
  ```

## Architecture overview

### Core (`Invex.StructuredText`)

- **`StructuredTextWriter`** — an indentation-aware text writer. `WriteSection` returns an
  `IDisposable` scope that increments/decrements the indent, so C# nesting mirrors YAML nesting.
- **`Tex`** — the single entry point for creating expressions (references, status checks,
  functions, `Tex.Raw`). Platform packages and libraries add members with C# 14 `extension(Tex)`
  blocks (`Tex.GitHub`, `Tex.DevOps`). Keep new factory methods on `Tex`, not on node types.
- **`Expr` tree** (`Expressions/Typed/`) — platform-agnostic records under four typed roots:
  `BoolExpr`, `StringExpr`, `NumberExpr`, `JsonExpr`. Conditions are `BoolExpr` and use the C#
  operators `&&`, `||`, `!`; comparisons and functions are instance methods (`Eq`, `Contains`, …).
- **`Template`** — an interpolated string handler holding literal text and embedded expressions.
  Every expression-capable model property other than a condition is a `Template`.
- **`ExpressionPrinter`** — abstract base class for platform printers. It handles precedence,
  quoting, access, casts and raw text; platforms implement `WrapEmbedded`, `PrintReference` and
  `PrintResult`, and override the virtual members where their syntax differs.

### Platform packages

Each platform package contains:

- **Model types** — records mirroring the platform's YAML schema; conditions are `BoolExpr`,
  other scalar properties are `Template`, and lists are `IReadOnlyList<Template>`.
- **A writer** — walks the model and emits YAML via `StructuredTextWriter`, calling the platform
  printer for every expression value.
- **An expression printer** — an `ExpressionPrinter` subclass for the platform's syntax
  (`GithubExpressionPrinter`, `DevopsExpressionPrinter`).
- **A context catalogue** — `Tex.GitHub` is generated from `Contexts/github.contexts` by
  `Invex.StructuredText.Generators`; `Tex.DevOps` is hand-written in `Contexts/DevopsContexts.cs`.
- **Discriminated unions** — schema "one of" shapes are modeled with
  [Dunet](https://github.com/domn1995/dunet) (`[Union]` on an abstract `partial record`, variants
  as nested `partial record`s). Construct the variant directly: `new Step.RunStep { … }`.

### Key design rules

- The **core package must not reference any platform package**. Platform-specific knowledge (syntax,
  context paths, outcome strings) belongs entirely in the platform package's formatter and models.
- Core expression XML docs must stay **platform-neutral** — no mentions of GitHub Actions or Azure
  DevOps in `Invex.StructuredText` source files.
- In the Azure DevOps package, `Template` is aliased as `ExprTemplate` (see `_usings.cs`) because
  Dunet union cases named `Template` (`Step.Template`, `Stage.Template`, …) shadow it.

## Atom workflows

- The GitHub Actions workflow YAML under `.github/workflows/` (`Validate.yml`, `Build.yml`,
  `Dependabot Enable auto-merge.yml`, `Cleanup Prereleases.yml`) is **generated** from the Atom
  build definition in `_atom/IBuild.cs`.
- **Whenever you change anything that affects the workflows** — targets, workflow definitions,
  triggers, options, or params/secrets — regenerate the YAML:

  ```shell
  atom gen
  ```

  (equivalently `dotnet run --project _atom -- gen`). Commit the regenerated `.github/workflows/`
  files alongside your `_atom/` changes; **never hand-edit the generated YAML**.
- A drift between `_atom/IBuild.cs` and the committed YAML should be treated as a missing
  `atom gen` run.

## Conventions

- Annotate every new public member with `[PublicAPI]` — the in-repo analyzer flags anything missing,
  and warnings are errors.
- Add XML doc comments to all public types and members. Match the existing `<summary>` / `<param>` /
  `<remarks>` style. Doc comments in `Invex.StructuredText` must not mention specific platforms;
  doc comments in platform packages may and should describe platform-specific syntax.
- Use [Conventional Commits](https://www.conventionalcommits.org/) — the prefix drives versioning:

  | Prefix | Version bump |
  |--------|-------------|
  | `breaking:` / `major:` | Major |
  | `feat:` / `feature:` / `minor:` | Minor |
  | `fix:` / `patch:` | Patch |
  | `semver-none` / `semver-skip` | No bump |

- When adding user-facing features, update the relevant `docs/` page and `README.md`.

## Testing & the Verify workflow

- Tests use **NUnit** with **Shouldly** and **Verify** (`Verify.NUnit`) for snapshot/approval testing.
- A snapshot test fails when its output differs from the committed `*.verified.txt`. On failure,
  Verify writes a `*.received.txt` next to it.
- **If the diff is unintended**, fix the code. **If the change is valid (expected new output)**,
  accept it and re-run:
  1. Overwrite the `*.verified.txt` with the contents of the matching `*.received.txt`.
  2. Delete the `*.received.txt`.
  3. Re-run `dotnet test` to confirm the suite is green.
- Each test project has a **public API surface snapshot test** whose `*.verified.txt` tracks the
  complete public API. An unexpected diff there signals an unintentional API change — treat it as
  such and double-check before accepting.

## Adding a new expression

1. Prefer reusing an existing node: most functions are a `BoolFunction` / `StringFunction` /
   `JsonFunction` with a name from `FunctionNames`, exposed as a `Tex` member or an instance method.
2. For a new node, add a record in `Expressions/Typed/ExprNodes.cs` deriving from the right typed
   root, with `Children` listing its sub-expressions so `DescendantsAndSelf()` sees them.
3. Print it in `ExpressionPrinter` (or a platform printer's `PrintOther` if it is platform-only);
   unsupported nodes must throw `NotSupportedException`.
4. Add unit tests in the relevant test project.
5. Update `PublicApiTests.*.verified.txt` (see the Verify workflow above).

## Adding a new platform

1. Add a new library project under `src/`.
2. Reference `Invex.StructuredText`.
3. Define model record types (use `[Union]` + Dunet for discriminated shapes).
4. Implement an `ExpressionPrinter` subclass.
5. Implement a writer over `StructuredTextWriter`.
6. Add a test project under `tests/`, including a public API surface snapshot test.
7. Register the new projects in `_atom/IBuild.cs` (`ProjectsToPack`, `ProjectsToTest`) and
   regenerate workflows with `atom gen`.

## Documentation

Docs follow the shared Invex documentation standard used by every Invex library:

- **README.md** is also the NuGet readme. Keep the fixed section order (title, badges, pitch, Packages, Features,
  Installation, Quick start, Usage, Documentation, Development, License) and use absolute links only
  (`https://github.com/Invex-Games/structured-text/blob/main/...` or the docs site), so they work on nuget.org.
- **DocFX site** (`docfx.json`, `toc.yml`, `index.md`, `docs/`, `api/index.md`), published to
  https://invex-games.github.io/structured-text/. `docs/introduction.md` and `docs/getting-started.md` come first;
  every page opens with one to three sentences on what it covers and ends with a "See also" or "Next steps" list.
  Link API types with `<xref:Namespace.Type>`, not file paths.
- **`api/index.md`** is the only hand-written file under `api/`; everything else there is generated. Put per-type
  detail in XML docs, not in hand-written API pages.
- **XML docs are enforced.** `CS1591` is not suppressed for shipped projects, so every public type and member needs
  `<summary>`, plus `<param>`, `<typeparam>`, `<returns>` and `<exception>` where they apply, `<remarks>` for
  non-obvious behaviour, and `<see cref>`/`<paramref>`/`<see langword>` for references.

## Defer to the docs

For anything beyond the above, prefer these over duplicating detail:

- [README.md](README.md) — package overview and quick start.
- [docs/introduction.md](docs/introduction.md) — what the library is and why it exists.
- [docs/getting-started.md](docs/getting-started.md) — installation and first examples.
- [docs/expressions.md](docs/expressions.md) — the expression API (`Tex`, conditions, templates).
- [samples/README.md](samples/README.md) — runnable expression samples, checked by `SamplesTests`.
- [docs/github-actions.md](docs/github-actions.md) — GitHub Actions writer in depth.
- [docs/dependabot.md](docs/dependabot.md) — Dependabot config writer.
- [docs/azure-devops-pipelines.md](docs/azure-devops-pipelines.md) — Azure DevOps writer in depth.
- [docs/architecture.md](docs/architecture.md) — how the packages fit together.

