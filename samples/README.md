# Samples

Runnable examples of the typed expression API. Each sample is a small C# file; the output below is what it prints.

Run them all, or pick some by name:

```sh
dotnet run --project samples/Invex.StructuredText.Samples
dotnet run --project samples/Invex.StructuredText.Samples -- conditions ci-workflow
```

The same output is checked by `SamplesTests` in the GitHub Actions test project, so these samples always compile and stay accurate.

## Conditions

Write if: conditions with C# operators. Source: [ConditionsSample.cs](Invex.StructuredText.Samples/ConditionsSample.cs) (`conditions`).

```text
isPush && onMain             => github.event_name == 'push' && github.ref == 'refs/heads/main'
(isPush || isTag) && !bot    => (github.event_name == 'push' || startsWith(github.ref, 'refs/tags/')) && !(github.actor == 'dependabot[bot]')
isPush || isTag && onMain    => github.event_name == 'push' || startsWith(github.ref, 'refs/tags/') && github.ref == 'refs/heads/main'
!isDependabot                => ${{ !(github.actor == 'dependabot[bot]') }}
GitHub.Ref.Eq("it's")        => github.ref == 'it''s'
PR number > 100              => github.event.pull_request.number > 100
isPullRequest && !draft      => github.event_name == 'pull_request' && !github.event.pull_request.draft
Always() && isPush           => always() && github.event_name == 'push'
Tex.Any(...)                 => github.event_name == 'push' || startsWith(github.ref, 'refs/tags/') || github.actor == 'dependabot[bot]'
```

## Values

Build YAML values with interpolated strings. Source: [ValuesSample.cs](Invex.StructuredText.Samples/ValuesSample.cs) (`values`).

```text
concurrency group            => ${{ github.workflow }}-${{ github.ref }}
artifact path                => ${{ github.workspace }}/artifacts/${{ matrix.os }}
step name                    => Setup .NET 10.0
cache key                    => ${{ matrix.os }}-${{ matrix.framework }}-release
env value                    => ${{ secrets.NUGET_API_KEY }}
```

## References

Refer to secrets, variables, inputs, matrix values and other jobs. Source: [ReferencesSample.cs](Invex.StructuredText.Samples/ReferencesSample.cs) (`references`).

```text
Tex.Secret                   => ${{ secrets.NUGET_API_KEY }}
Tex.Variable                 => ${{ vars.DEPLOY_REGION }}
Tex.Input                    => ${{ inputs.dry-run }}
Tex.Matrix                   => ${{ matrix.os }}
Tex.StepOutput               => ${{ steps.version.outputs.semver }}
Tex.JobOutput                => ${{ needs.setup.outputs.build-version }}
JobResult == Success         => needs.test.result == 'success'
prerelease version           => contains(needs.setup.outputs.build-version, '-')
```

## Functions and escape hatches

Reach anything the catalogue doesn't cover. Source: [EscapeHatchesSample.cs](Invex.StructuredText.Samples/EscapeHatchesSample.cs) (`escape-hatches`).

```text
indexer + AsString           => github.event.label.name == 'release'
Tex.Format + HashFiles       => ${{ format('{0}-nuget-{1}', runner.os, hashFiles('**/*.csproj')) }}
value In array               => contains(fromJSON(vars.RELEASE_LABELS), github.event.label.name)
ToJson                       => ${{ toJSON(github.event) }}
BoolExpr.Raw                 => github.event.forced && success()
```

## A complete workflow

Build, test on three platforms, publish stable versions, report failures. Source: [CiWorkflowSample.cs](Invex.StructuredText.Samples/CiWorkflowSample.cs) (`ci-workflow`).

```yaml
name: Build

on:
  pull_request:
    branches: [ main ]
  push:
    branches: [ main ]

permissions: read-all

concurrency:
  group: '${{ github.workflow }}-${{ github.ref }}'
  cancel-in-progress: ${{ github.event_name == 'pull_request' }}

jobs:

  setup:
    runs-on: ubuntu-latest
    outputs:
      version: ${{ steps.version.outputs.semver }}
    steps:

      - name: Checkout
        uses: actions/checkout@v6
        with:
          fetch-depth: 0

      - name: Compute version
        id: version
        run: |
          dotnet tool install --global GitVersion.Tool
          semver=$(dotnet-gitversion /showvariable SemVer)
          echo "semver=$semver" >> "$GITHUB_OUTPUT"

  test:
    needs: [ setup ]
    runs-on: ${{ matrix.os }}
    strategy:
      matrix:
        os: [ ubuntu-latest, windows-latest, macos-latest ]
    steps:

      - name: Checkout
        uses: actions/checkout@v6

      - name: 'Test on ${{ matrix.os }}'
        run: 'dotnet test -p:Version=${{ needs.setup.outputs.version }}'

  publish:
    needs: [ setup, test ]
    if: ${{ !contains(needs.setup.outputs.version, '-') && github.event_name != 'pull_request' }}
    runs-on: ubuntu-latest
    steps:

      - name: Checkout
        uses: actions/checkout@v6

      - name: 'Publish ${{ needs.setup.outputs.version }}'
        run: |
          dotnet pack -c Release -p:Version=${{ needs.setup.outputs.version }}
          dotnet nuget push "**/*.nupkg" --api-key "$NUGET_API_KEY" --source nuget.org
        env:
          NUGET_API_KEY: ${{ secrets.NUGET_API_KEY }}

  notify:
    needs: [ test ]
    if: always() && needs.test.result == 'failure'
    runs-on: ubuntu-latest
    steps:

      - name: Report failure
        run: 'echo "Tests failed on ${{ github.ref }} (${{ github.sha }})"'
```
