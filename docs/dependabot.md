# Dependabot Configuration

The `Invex.StructuredText.GithubActions` package also generates
[Dependabot v2](https://docs.github.com/code-security/dependabot/dependabot-version-updates/configuration-options-for-the-dependabot.yml-file)
configuration files from the `DependabotConfig` model
(namespace `Invex.StructuredText.GithubActions.DependabotConfigModel.Model`).

```csharp
var writer = new DependabotConfigWriter();
writer.WriteConfig(config);
var yaml = writer.TextWriter.ToString();
```

## Minimal config

```csharp
var config = new DependabotConfig
{
    Updates =
    [
        new DependabotUpdate
        {
            PackageEcosystem = "nuget",
            Directory = "/",
            Schedule = new() { Interval = ScheduleInterval.Weekly },
        },
    ],
};
```

```yaml
version: 2

updates:
  - package-ecosystem: nuget
    directory: /
    schedule:
      interval: weekly
```

`Version` defaults to `2` (the only valid value), so you never need to set it.

## The model at a glance

```text
DependabotConfig
├── Version (= 2)
├── EnableBetaEcosystems
├── Registries            (name → DependabotRegistry)
├── MultiEcosystemGroups  (name → DependabotMultiEcosystemGroup)
└── Updates               (list of DependabotUpdate)
    ├── PackageEcosystem, Directory / Directories, TargetBranch
    ├── Schedule (interval, day, time, timezone, cronjob)
    ├── Allow, Ignore, ExcludePaths
    ├── Groups (name → DependabotGroup)
    ├── Registries, Labels, Assignees, Milestone
    ├── CommitMessage, PullRequestBranchName, RebaseStrategy
    ├── OpenPullRequestsLimit, Cooldown, Vendor
    ├── VersioningStrategy, InsecureExternalCodeExecution
    └── MultiEcosystemGroup, Patterns
```

## Private registries

```csharp
var config = new DependabotConfig
{
    Registries = new Dictionary<string, DependabotRegistry>
    {
        ["my-nuget"] = new()
        {
            Type = RegistryType.NugetFeed,
            Url = "https://pkgs.dev.azure.com/myorg/_packaging/my-feed/nuget/v3/index.json",
            Token = "${{ secrets.FEED_TOKEN }}",
        },
        ["dockerhub"] = new()
        {
            Type = RegistryType.DockerRegistry,
            Url = "https://registry.hub.docker.com",
            Username = "my-user",
            Password = "${{ secrets.DOCKER_TOKEN }}",
            ReplacesBase = true,
        },
    },
    Updates =
    [
        new DependabotUpdate
        {
            PackageEcosystem = "nuget",
            Directory = "/",
            Schedule = new() { Interval = ScheduleInterval.Daily },
            Registries = new DependabotRegistries.Named("my-nuget"),
        },
    ],
};
```

> [!NOTE]
> Dependabot config values are plain strings — `${{ secrets.* }}` references are passed through verbatim, exactly as
> Dependabot expects.

## Schedules

```csharp
Schedule = new DependabotSchedule
{
    Interval = ScheduleInterval.Weekly,   // Daily | Weekly | Monthly | Quarterly | Semiannually | Yearly | Cron
    Day = ScheduleDay.Monday,
    Time = "06:00",
    Timezone = "Europe/London",
},

// or cron-based:
Schedule = new DependabotSchedule
{
    Interval = ScheduleInterval.Cron,
    Cronjob = "0 6 * * 1",
},
```

## Groups

Group related dependencies into a single pull request:

```csharp
Groups = new Dictionary<string, DependabotGroup>
{
    ["test-packages"] = new DependabotGroup.FromPatterns
    {
        Patterns = ["NUnit*", "Shouldly", "Verify*"],
    },
    ["analyzers"] = new DependabotGroup.FromPatterns
    {
        Patterns = ["*.Analyzers"],
    },
},
```

Use `new DependabotRegistries.All()` to opt an update into every configured registry, or
`new DependabotRegistries.Named(...)` for a specific list. Group variants include `FromPatterns`,
`FromExcludePatterns`, `FromType`, `FromUpdateTypes`, and `FromGroupBy`.

Multi-ecosystem groups consolidate PRs across package managers — define them at the config level via
`MultiEcosystemGroups` and reference them per-update via `MultiEcosystemGroup` and `Patterns`.

## Allow / ignore rules

```csharp
Allow =
[
    new DependabotAllow
    {
        DependencyName = "Microsoft.*",
        DependencyType = DependencyType.Production,
    },
],
Ignore =
[
    new DependabotIgnore
    {
        DependencyName = "Newtonsoft.Json",
        UpdateTypes = [SemverUpdateType.VersionUpdateSemverMajor],
        Versions = new DependabotVersions.Multiple(["13.x"]),
    },
],
```

## Pull-request presentation

```csharp
CommitMessage = new DependabotCommitMessage
{
    Prefix = "deps",
    PrefixDevelopment = null,
    Include = CommitMessageInclude.Scope,
},
Labels = ["dependencies"],
Assignees = ["octocat"],
OpenPullRequestsLimit = 10,
```

## See also

- [GitHub Actions](github-actions.md) — workflow generation from the same package
- The model closely mirrors the official
  [dependabot-2.0 JSON schema](https://json.schemastore.org/dependabot-2.0.json); the schema file is included in the
  source tree for reference.
