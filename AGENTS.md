# Copilot Instructions

Guidance for AI agents working in **Invex.StructuredText** — a family of strongly-typed C# libraries
for generating YAML for GitHub Actions workflows, Dependabot configurations, and Azure DevOps
Pipelines. Keep changes focused and defer to the linked docs for detail.

## What's in the repo

| Project | Role | Target frameworks |
|---------|------|-------------------|
| `Invex.StructuredText` | Core library: `StructuredTextWriter`, the `TextExpression` tree, `TextExpressionFormatter`, and collection types | `net8.0;net9.0;net10.0` |
| `Invex.StructuredText.GithubActions` | GitHub Actions workflow and Dependabot config generation | `net8.0;net9.0;net10.0` |
| `Invex.StructuredText.AzureDevopsPipelines` | Azure DevOps Pipelines YAML generation | `net8.0;net9.0;net10.0` |

Sources live under `src/`, tests under `tests/`, the Atom build definition under `_atom/`, and the
DocFX documentation site under `docs/`.

## Build & language specifics

- **.NET 10 SDK** is required. All library and test projects multi-target `net8.0;net9.0;net10.0`.
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
- **`TextExpression` hierarchy** — a platform-agnostic expression tree (records). Compose
  expressions with fluent methods (`Contains`, `EqualTo`, `And`, …) and operators (`&`, `|`, `!`).
- **`TextExpressionFormatter`** — abstract base class for platform-specific formatters. The
  `Format()` loop handles `RawExpression`, `ConcatExpression`, and `CastExpression<T>` centrally;
  `Resolve()` is the only method platform formatters implement.
- **`TextExpressionCollection` / `WorkflowExpressionCollection<T>`** — lists with implicit
  conversions from `string`, `string[]`, and `TextExpression` so model properties accept them
  naturally (e.g. `Branches = ["main"]`).

### Platform packages

Each platform package contains:

- **Model types** — records mirroring the platform's YAML schema; scalar properties are
  `WorkflowExpression<T>` (a zero-overhead typed wrapper over `TextExpression`).
- **A writer** — walks the model and emits YAML via `StructuredTextWriter`, calling the platform
  formatter for every expression value.
- **An expression formatter** — a `TextExpressionFormatter` subclass for the platform's syntax
  (`GithubExpressionFormatter`, `DevopsExpressionFormatter`).
- **Discriminated unions** — schema "one of" shapes are modeled with
  [Dunet](https://github.com/domn1995/dunet) (`[Union]` on an abstract `partial record`, variants
  as nested `partial record`s). Construct the variant directly: `new Step.RunStep { … }`.

### Key design rules

- The **core package must not reference any platform package**. Platform-specific knowledge (syntax,
  context paths, outcome strings) belongs entirely in the platform package's formatter and models.
- `TextExpression` XML docs must stay **platform-neutral** — no mentions of GitHub Actions or Azure
  DevOps in `Invex.StructuredText` source files.
- `WorkflowExpression<T>` type parameters are **documentation only** — they have no effect on
  formatting.

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

## Adding a new expression type

1. Add the record to the appropriate `TextExpression.*.cs` partial file in `Invex.StructuredText`.
2. Add `Resolve` cases for it in each platform formatter that should support it.
3. Add it to `TextExpressionUtils.Flatten` if it has sub-expressions.
4. Add unit tests in the relevant test project.
5. Update `PublicApiSurfaceTests.*.verified.txt` (see the Verify workflow above).

## Adding a new platform

1. Add a new library project under `src/`.
2. Reference `Invex.StructuredText`.
3. Define model record types (use `[Union]` + Dunet for discriminated shapes).
4. Implement a `TextExpressionFormatter` subclass.
5. Implement a writer over `StructuredTextWriter`.
6. Add a test project under `tests/`, including a public API surface snapshot test.
7. Register the new projects in `_atom/IBuild.cs` (`ProjectsToPack`, `ProjectsToTest`) and
   regenerate workflows with `atom gen`.

## Defer to the docs

For anything beyond the above, prefer these over duplicating detail:

- [README.md](../README.md) — package overview and quick start.
- [docs/introduction.md](../docs/introduction.md) — what the library is and why it exists.
- [docs/getting-started.md](../docs/getting-started.md) — installation and first examples.
- [docs/expressions.md](../docs/expressions.md) — the full expression system.
- [docs/github-actions.md](../docs/github-actions.md) — GitHub Actions writer in depth.
- [docs/dependabot.md](../docs/dependabot.md) — Dependabot config writer.
- [docs/azure-devops-pipelines.md](../docs/azure-devops-pipelines.md) — Azure DevOps writer in depth.
- [docs/architecture.md](../docs/architecture.md) — how the packages fit together.

