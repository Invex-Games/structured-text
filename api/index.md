# API Reference

This section is the generated reference for every public type in the three Invex.StructuredText packages, built from
the XML documentation comments in the source code; the tables below list each type by package and namespace. Union
records are [Dunet](https://github.com/domn1995/dunet) discriminated unions: construct one of their nested variant
records (for example `new Step.RunStep { ... }`).

## Invex.StructuredText

[NuGet](https://www.nuget.org/packages/Invex.StructuredText). The core: `StructuredTextWriter` and the platform-agnostic expression system.

### Invex.StructuredText

| Type | Kind | Description |
| --- | --- | --- |
| [`StructuredTextWriter`](xref:Invex.StructuredText.StructuredTextWriter) | Class | An indentation-aware text writer for producing structured text such as YAML. |

### Invex.StructuredText.Expressions

| Type | Kind | Description |
| --- | --- | --- |
| [`AndExpression`](xref:Invex.StructuredText.Expressions.AndExpression) | Record | Logical AND over `Source`. |
| [`BooleanExpression`](xref:Invex.StructuredText.Expressions.BooleanExpression) | Record | A boolean literal, rendered as `true` or `false`. |
| [`CastExpression<TTo>`](xref:Invex.StructuredText.Expressions.CastExpression`1) | Record | Wraps an expression to reinterpret its result type at compile time. |
| [`CoalesceExpression`](xref:Invex.StructuredText.Expressions.CoalesceExpression) | Record | Returns the first non-empty value of `Source`, rendered as `coalesce(...)`. |
| [`ConcatExpression`](xref:Invex.StructuredText.Expressions.ConcatExpression) | Record | Concatenates the rendered text of `Values` with no separator. |
| [`ContainsExpression`](xref:Invex.StructuredText.Expressions.ContainsExpression) | Record | Tests whether `Source` contains `Pattern`, rendered as `contains(source, pattern)`. |
| [`EndsWithExpression`](xref:Invex.StructuredText.Expressions.EndsWithExpression) | Record | Tests whether `Source` ends with `Pattern`, rendered as `endsWith(source, pattern)`. |
| [`EqualExpression`](xref:Invex.StructuredText.Expressions.EqualExpression) | Record | Equality comparison. |
| [`EvaluateExpression`](xref:Invex.StructuredText.Expressions.EvaluateExpression) | Record | Marks `Expression` for evaluation in the target platform's expression context. |
| [`FormatExpression`](xref:Invex.StructuredText.Expressions.FormatExpression) | Record | Replaces placeholders (`{0}`, `{1}`, …) in `Source` with `Arguments`, rendered as `format(source, args...)`. |
| [`GreaterThanExpression`](xref:Invex.StructuredText.Expressions.GreaterThanExpression) | Record | Greater-than comparison. |
| [`GreaterThanOrEqualToExpression`](xref:Invex.StructuredText.Expressions.GreaterThanOrEqualToExpression) | Record | Greater-than-or-equal comparison. |
| [`HashFilesExpression`](xref:Invex.StructuredText.Expressions.HashFilesExpression) | Record | Returns a hash of the files matched by the glob pattern(s) in `Source`, rendered as `hashFiles(source)`. |
| [`IndexAccessExpression`](xref:Invex.StructuredText.Expressions.IndexAccessExpression) | Record | Accesses an element of `Array` by `Index`, rendered as `array[index]`. |
| [`ITextExpression<TResult>`](xref:Invex.StructuredText.Expressions.ITextExpression`1) | Interface | Marker interface that documents the result type an expression is expected to resolve to. |
| [`ITextExpressionFormatter`](xref:Invex.StructuredText.Expressions.ITextExpressionFormatter) | Interface | Renders a `TextExpression` tree into platform-specific text. |
| [`JoinExpression`](xref:Invex.StructuredText.Expressions.JoinExpression) | Record | Joins the elements of the array-valued `Source` with `OptionalSeparator` (comma when omitted), rendered as `join(source, separator)`. |
| [`LessThanExpression`](xref:Invex.StructuredText.Expressions.LessThanExpression) | Record | Less-than comparison. |
| [`LessThanOrEqualToExpression`](xref:Invex.StructuredText.Expressions.LessThanOrEqualToExpression) | Record | Less-than-or-equal comparison. |
| [`NotEqualExpression`](xref:Invex.StructuredText.Expressions.NotEqualExpression) | Record | Inequality comparison. |
| [`NotExpression`](xref:Invex.StructuredText.Expressions.NotExpression) | Record | Logical negation of `Source`. |
| [`NullExpression`](xref:Invex.StructuredText.Expressions.NullExpression) | Record | The null literal, rendered as empty text. |
| [`NumberExpression`](xref:Invex.StructuredText.Expressions.NumberExpression) | Record | A numeric literal, rendered using the invariant culture. |
| [`OrExpression`](xref:Invex.StructuredText.Expressions.OrExpression) | Record | Logical OR over `Source`. |
| [`PropertyAccessExpression`](xref:Invex.StructuredText.Expressions.PropertyAccessExpression) | Record | Accesses `Property` of `Object`, rendered as `object.property`. |
| [`RawExpression`](xref:Invex.StructuredText.Expressions.RawExpression) | Record | Verbatim, uninterpreted text. |
| [`StartsWithExpression`](xref:Invex.StructuredText.Expressions.StartsWithExpression) | Record | Tests whether `Source` starts with `Pattern`, rendered as `startsWith(source, pattern)`. |
| [`StepOutcomeExpression`](xref:Invex.StructuredText.Expressions.StepOutcomeExpression) | Record | References the outcome of a step in the current job, rendered as `steps.<step>.outcome`. |
| [`StepOutcomeTypeExpression`](xref:Invex.StructuredText.Expressions.StepOutcomeTypeExpression) | Record | A step outcome literal for comparison with `StepOutcomeExpression`. |
| [`StepOutputExpression`](xref:Invex.StructuredText.Expressions.StepOutputExpression) | Record | References an output of a step in the current job. |
| [`StringExpression`](xref:Invex.StructuredText.Expressions.StringExpression) | Record | A string literal inside an expression, rendered with quotes (e.g. `'value'`). |
| [`TargetOutcomeExpression`](xref:Invex.StructuredText.Expressions.TargetOutcomeExpression) | Record | References the outcome (result/status) of another job or stage. |
| [`TargetOutcomeTypeExpression`](xref:Invex.StructuredText.Expressions.TargetOutcomeTypeExpression) | Record | A job/stage outcome literal for comparison with `TargetOutcomeExpression`. |
| [`TargetOutputExpression`](xref:Invex.StructuredText.Expressions.TargetOutputExpression) | Record | References an output of another job or stage. |
| [`TextExpression`](xref:Invex.StructuredText.Expressions.TextExpression) | Abstract record | The base type of the platform-agnostic expression tree. |
| [`TextExpressionCollection`](xref:Invex.StructuredText.Expressions.TextExpressionCollection) | Class | An ordered collection of `TextExpression` instances. |
| [`TextExpressionFormatter`](xref:Invex.StructuredText.Expressions.TextExpressionFormatter) | Abstract class | Base class for platform-specific expression formatters. |
| [`TextExpressionInterpolatedStringHandler`](xref:Invex.StructuredText.Expressions.TextExpressionInterpolatedStringHandler) | Ref struct | An interpolated string handler that creates a `FormatExpression` from an interpolated string. |
| [`TextExpressions`](xref:Invex.StructuredText.Expressions.TextExpressions) | Static class | The entry point for creating `TextExpression` instances. |
| [`TextExpressionUtils`](xref:Invex.StructuredText.Expressions.TextExpressionUtils) | Static class | Utility helpers for working with sequences and trees of `TextExpression`. |
| [`ToJsonExpression`](xref:Invex.StructuredText.Expressions.ToJsonExpression) | Record | Converts `Source` to its JSON representation, rendered as the platform's JSON serialization function. |
| [`WorkflowExpression<T>`](xref:Invex.StructuredText.Expressions.WorkflowExpression`1) | Record struct | A typed workflow expression that documents the expected result type of the expression. |
| [`WorkflowExpressionCollection<T>`](xref:Invex.StructuredText.Expressions.WorkflowExpressionCollection`1) | Class | A typed workflow expression collection that documents the expected element result type. |
| [`WorkflowExpressionExtensions`](xref:Invex.StructuredText.Expressions.WorkflowExpressionExtensions) | Static class | Extension members that augment `TextExpressions` and `TextExpression` with factory helpers for building expression trees. |

## Invex.StructuredText.GithubActions

[NuGet](https://www.nuget.org/packages/Invex.StructuredText.GithubActions). GitHub Actions workflow and Dependabot configuration generation.

### Invex.StructuredText.GithubActions

| Type | Kind | Description |
| --- | --- | --- |
| [`DependabotConfigWriter`](xref:Invex.StructuredText.GithubActions.DependabotConfigWriter) | Class | Serializes a `DependabotConfig` model to Dependabot v2 configuration YAML (`.github/dependabot.yml`). |
| [`GithubActionWriter`](xref:Invex.StructuredText.GithubActions.GithubActionWriter) | Class | Serializes a `GithubAction` model to GitHub Actions workflow YAML. |
| [`GithubExpressionFormatter`](xref:Invex.StructuredText.GithubActions.GithubExpressionFormatter) | Class | Renders `TextExpression` trees using GitHub Actions expression syntax, such as `${{ ... }}`, `&&` and `||`. |

### Invex.StructuredText.GithubActions.GithubActionModel

| Type | Kind | Description |
| --- | --- | --- |
| [`Concurrency`](xref:Invex.StructuredText.GithubActions.GithubActionModel.Concurrency) | Record | Concurrency settings (`concurrency`) that ensure only one run (or queued run) sharing the same group executes at a time, at either workflow or job level. |
| [`Container`](xref:Invex.StructuredText.GithubActions.GithubActionModel.Container) | Record | A Docker container that a job runs in (`container`) or that hosts a service (`services`). |
| [`Credentials`](xref:Invex.StructuredText.GithubActions.GithubActionModel.Credentials) | Record | Credentials for a private container registry (`credentials`), passed to `docker login`. |
| [`Environment`](xref:Invex.StructuredText.GithubActions.GithubActionModel.Environment) | Record | The deployment environment a job targets (`environment`). |
| [`GithubAction`](xref:Invex.StructuredText.GithubActions.GithubActionModel.GithubAction) | Record | The root model of a GitHub Actions workflow file (`.github/workflows/*.yml`). |
| [`Job`](xref:Invex.StructuredText.GithubActions.GithubActionModel.Job) | Record | A job within a workflow: a set of steps that execute on the same runner. |
| [`Matrix`](xref:Invex.StructuredText.GithubActions.GithubActionModel.Matrix) | Record | A matrix of variable combinations to run a job with (`strategy.matrix`). |
| [`On`](xref:Invex.StructuredText.GithubActions.GithubActionModel.On) | Union record | A workflow trigger event (`on`). |
| [`Permissions`](xref:Invex.StructuredText.GithubActions.GithubActionModel.Permissions) | Union record | `GITHUB_TOKEN` permissions at workflow or job level (`permissions`). |
| [`PermissionsEvent`](xref:Invex.StructuredText.GithubActions.GithubActionModel.PermissionsEvent) | Record | Per-scope `GITHUB_TOKEN` permission levels. |
| [`PermissionsLevel`](xref:Invex.StructuredText.GithubActions.GithubActionModel.PermissionsLevel) | Enum | The access level granted to a `GITHUB_TOKEN` permission scope. |
| [`RunsOn`](xref:Invex.StructuredText.GithubActions.GithubActionModel.RunsOn) | Record | The runner selection for a job (`runs-on`). |
| [`Snapshot`](xref:Invex.StructuredText.GithubActions.GithubActionModel.Snapshot) | Record | Runner image snapshot settings for a job (`snapshot`), used to capture the runner state as a custom image after the job completes. |
| [`Step`](xref:Invex.StructuredText.GithubActions.GithubActionModel.Step) | Union record | A single step within a job. |
| [`Strategy`](xref:Invex.StructuredText.GithubActions.GithubActionModel.Strategy) | Record | The execution strategy for a job (`strategy`), defining a matrix of variations to run. |
| [`WorkflowDispatchInput`](xref:Invex.StructuredText.GithubActions.GithubActionModel.WorkflowDispatchInput) | Union record | A typed input for the `workflow_dispatch` trigger. |

### Invex.StructuredText.GithubActions.DependabotConfigModel.Model

| Type | Kind | Description |
| --- | --- | --- |
| [`BranchNameSeparator`](xref:Invex.StructuredText.GithubActions.DependabotConfigModel.Model.BranchNameSeparator) | Enum | Branch name separator. |
| [`CommitMessageInclude`](xref:Invex.StructuredText.GithubActions.DependabotConfigModel.Model.CommitMessageInclude) | Enum | Commit message include option. |
| [`DependabotAllow`](xref:Invex.StructuredText.GithubActions.DependabotConfigModel.Model.DependabotAllow) | Record | Allow rule for Dependabot updates. |
| [`DependabotCommitMessage`](xref:Invex.StructuredText.GithubActions.DependabotConfigModel.Model.DependabotCommitMessage) | Record | Commit message preferences for Dependabot. |
| [`DependabotConfig`](xref:Invex.StructuredText.GithubActions.DependabotConfigModel.Model.DependabotConfig) | Record | GitHub Dependabot v2 configuration. |
| [`DependabotCooldown`](xref:Invex.StructuredText.GithubActions.DependabotConfigModel.Model.DependabotCooldown) | Record | Cooldown configuration for dependency updates. |
| [`DependabotGroup`](xref:Invex.StructuredText.GithubActions.DependabotConfigModel.Model.DependabotGroup) | Union record | Group configuration for dependencies. |
| [`DependabotIgnore`](xref:Invex.StructuredText.GithubActions.DependabotConfigModel.Model.DependabotIgnore) | Record | Ignore rule for Dependabot updates. |
| [`DependabotMultiEcosystemGroup`](xref:Invex.StructuredText.GithubActions.DependabotConfigModel.Model.DependabotMultiEcosystemGroup) | Record | Multi-ecosystem group configuration that spans multiple package ecosystems. |
| [`DependabotPullRequestBranchName`](xref:Invex.StructuredText.GithubActions.DependabotConfigModel.Model.DependabotPullRequestBranchName) | Record | Pull request branch name preferences. |
| [`DependabotRegistries`](xref:Invex.StructuredText.GithubActions.DependabotConfigModel.Model.DependabotRegistries) | Union record | Registries to use for an update configuration: a list of registry names or `*` for all. |
| [`DependabotRegistry`](xref:Invex.StructuredText.GithubActions.DependabotConfigModel.Model.DependabotRegistry) | Record | Private package registry configuration. |
| [`DependabotSchedule`](xref:Invex.StructuredText.GithubActions.DependabotConfigModel.Model.DependabotSchedule) | Record | Schedule preferences for Dependabot updates. |
| [`DependabotUpdate`](xref:Invex.StructuredText.GithubActions.DependabotConfigModel.Model.DependabotUpdate) | Record | Package ecosystem configuration for Dependabot updates. |
| [`DependabotVersions`](xref:Invex.StructuredText.GithubActions.DependabotConfigModel.Model.DependabotVersions) | Union record | Versions to ignore: a single version string or a list of version strings. |
| [`DependencyType`](xref:Invex.StructuredText.GithubActions.DependabotConfigModel.Model.DependencyType) | Enum | Dependency type for allow/ignore rules. |
| [`GroupAppliesTo`](xref:Invex.StructuredText.GithubActions.DependabotConfigModel.Model.GroupAppliesTo) | Enum | Specifies whether the rules in the group apply to version updates or security updates. |
| [`GroupBy`](xref:Invex.StructuredText.GithubActions.DependabotConfigModel.Model.GroupBy) | Enum | Configure how dependencies are grouped. |
| [`GroupDependencyType`](xref:Invex.StructuredText.GithubActions.DependabotConfigModel.Model.GroupDependencyType) | Enum | Dependency type for groups. |
| [`GroupUpdateType`](xref:Invex.StructuredText.GithubActions.DependabotConfigModel.Model.GroupUpdateType) | Enum | Semantic versioning update type. |
| [`InsecureExternalCodeExecution`](xref:Invex.StructuredText.GithubActions.DependabotConfigModel.Model.InsecureExternalCodeExecution) | Enum | Allow or deny code execution in manifest files. |
| [`PackageEcosystem`](xref:Invex.StructuredText.GithubActions.DependabotConfigModel.Model.PackageEcosystem) | Enum | Package ecosystem values for Dependabot. |
| [`RebaseStrategy`](xref:Invex.StructuredText.GithubActions.DependabotConfigModel.Model.RebaseStrategy) | Enum | Rebase strategy for Dependabot. |
| [`RegistryType`](xref:Invex.StructuredText.GithubActions.DependabotConfigModel.Model.RegistryType) | Enum | Registry type for Dependabot. |
| [`ScheduleDay`](xref:Invex.StructuredText.GithubActions.DependabotConfigModel.Model.ScheduleDay) | Enum | Schedule day for Dependabot updates. |
| [`ScheduleInterval`](xref:Invex.StructuredText.GithubActions.DependabotConfigModel.Model.ScheduleInterval) | Enum | Schedule interval for Dependabot updates. |
| [`SemverUpdateType`](xref:Invex.StructuredText.GithubActions.DependabotConfigModel.Model.SemverUpdateType) | Enum | Semantic version update type. |
| [`VersioningStrategy`](xref:Invex.StructuredText.GithubActions.DependabotConfigModel.Model.VersioningStrategy) | Enum | How to update manifest version requirements. |

## Invex.StructuredText.AzureDevopsPipelines

[NuGet](https://www.nuget.org/packages/Invex.StructuredText.AzureDevopsPipelines). Azure DevOps Pipelines YAML generation.

### Invex.StructuredText.AzureDevopsPipelines

| Type | Kind | Description |
| --- | --- | --- |
| [`DevopsExpressionFormatter`](xref:Invex.StructuredText.AzureDevopsPipelines.DevopsExpressionFormatter) | Class | Renders `TextExpression` trees using Azure DevOps expression syntax, such as `and(...)`, `eq(...)` and `${{ ... }}` template expressions. |
| [`DevopsMacroExpression`](xref:Invex.StructuredText.AzureDevopsPipelines.DevopsMacroExpression) | Record | Wraps a variable name to be expanded using Azure DevOps macro syntax: `$(variableName)`. |
| [`DevopsPipelineWriter`](xref:Invex.StructuredText.AzureDevopsPipelines.DevopsPipelineWriter) | Class | Serializes a `DevopsPipeline` model to Azure Pipelines YAML. |
| [`DevopsRuntimeExpression`](xref:Invex.StructuredText.AzureDevopsPipelines.DevopsRuntimeExpression) | Record | Wraps an expression to be evaluated at runtime using Azure DevOps runtime expression syntax: `$[ expression ]`. |

### Invex.StructuredText.AzureDevopsPipelines.DevopsPipelineModel.Jobs

| Type | Kind | Description |
| --- | --- | --- |
| [`DeploymentEnvironment`](xref:Invex.StructuredText.AzureDevopsPipelines.DevopsPipelineModel.Jobs.DeploymentEnvironment) | Union record | Deployment environment specification. |
| [`DeploymentHook`](xref:Invex.StructuredText.AzureDevopsPipelines.DevopsPipelineModel.Jobs.DeploymentHook) | Record | Deployment lifecycle hook containing steps. |
| [`DeploymentStrategy`](xref:Invex.StructuredText.AzureDevopsPipelines.DevopsPipelineModel.Jobs.DeploymentStrategy) | Union record | Deployment strategy for a deployment job. |
| [`ExplicitResources`](xref:Invex.StructuredText.AzureDevopsPipelines.DevopsPipelineModel.Jobs.ExplicitResources) | Record | Explicit resource references for a job. |
| [`Job`](xref:Invex.StructuredText.AzureDevopsPipelines.DevopsPipelineModel.Jobs.Job) | Union record | A job represents a unit of work which can be assigned to a single agent or server. |
| [`JobContainer`](xref:Invex.StructuredText.AzureDevopsPipelines.DevopsPipelineModel.Jobs.JobContainer) | Union record | Container resource for a job. |
| [`JobStrategy`](xref:Invex.StructuredText.AzureDevopsPipelines.DevopsPipelineModel.Jobs.JobStrategy) | Record | Execution strategy for a job. |

### Invex.StructuredText.AzureDevopsPipelines.DevopsPipelineModel.Pipeline

| Type | Kind | Description |
| --- | --- | --- |
| [`DevopsPipeline`](xref:Invex.StructuredText.AzureDevopsPipelines.DevopsPipelineModel.Pipeline.DevopsPipeline) | Union record | A pipeline is one or more stages that describe a CI/CD process. |

### Invex.StructuredText.AzureDevopsPipelines.DevopsPipelineModel.Resources

| Type | Kind | Description |
| --- | --- | --- |
| [`BuildResource`](xref:Invex.StructuredText.AzureDevopsPipelines.DevopsPipelineModel.Resources.BuildResource) | Record | Build resource definition. |
| [`ContainerResource`](xref:Invex.StructuredText.AzureDevopsPipelines.DevopsPipelineModel.Resources.ContainerResource) | Record | Container image resource definition. |
| [`ContainerResourceTrigger`](xref:Invex.StructuredText.AzureDevopsPipelines.DevopsPipelineModel.Resources.ContainerResourceTrigger) | Record | Container resource trigger definition. |
| [`PackageResource`](xref:Invex.StructuredText.AzureDevopsPipelines.DevopsPipelineModel.Resources.PackageResource) | Record | Package resource definition. |
| [`PipelineResource`](xref:Invex.StructuredText.AzureDevopsPipelines.DevopsPipelineModel.Resources.PipelineResource) | Record | Pipeline resource definition. |
| [`PipelineResourceTrigger`](xref:Invex.StructuredText.AzureDevopsPipelines.DevopsPipelineModel.Resources.PipelineResourceTrigger) | Record | Pipeline resource trigger definition. |
| [`RepositoryResource`](xref:Invex.StructuredText.AzureDevopsPipelines.DevopsPipelineModel.Resources.RepositoryResource) | Record | Repository resource definition. |
| [`Resources`](xref:Invex.StructuredText.AzureDevopsPipelines.DevopsPipelineModel.Resources.Resources) | Record | Resources specifies builds, repositories, pipelines, and other resources used by the pipeline. |
| [`WebhookFilter`](xref:Invex.StructuredText.AzureDevopsPipelines.DevopsPipelineModel.Resources.WebhookFilter) | Record | Webhook filter definition. |
| [`WebhookResource`](xref:Invex.StructuredText.AzureDevopsPipelines.DevopsPipelineModel.Resources.WebhookResource) | Record | Webhook resource definition. |

### Invex.StructuredText.AzureDevopsPipelines.DevopsPipelineModel.Stages

| Type | Kind | Description |
| --- | --- | --- |
| [`Stage`](xref:Invex.StructuredText.AzureDevopsPipelines.DevopsPipelineModel.Stages.Stage) | Union record | A stage is a collection of related jobs. |

### Invex.StructuredText.AzureDevopsPipelines.DevopsPipelineModel.Steps

| Type | Kind | Description |
| --- | --- | --- |
| [`SettableVariables`](xref:Invex.StructuredText.AzureDevopsPipelines.DevopsPipelineModel.Steps.SettableVariables) | Record | Restrictions on which variables can be set from a step. |
| [`Step`](xref:Invex.StructuredText.AzureDevopsPipelines.DevopsPipelineModel.Steps.Step) | Union record | Steps are a linear sequence of operations that make up a job. |
| [`StepTarget`](xref:Invex.StructuredText.AzureDevopsPipelines.DevopsPipelineModel.Steps.StepTarget) | Union record | Environment in which to run a step. |

### Invex.StructuredText.AzureDevopsPipelines.DevopsPipelineModel.Supporting

| Type | Kind | Description |
| --- | --- | --- |
| [`Extends`](xref:Invex.StructuredText.AzureDevopsPipelines.DevopsPipelineModel.Supporting.Extends) | Record | Extends a template. |
| [`IncludeExcludeFilters`](xref:Invex.StructuredText.AzureDevopsPipelines.DevopsPipelineModel.Supporting.IncludeExcludeFilters) | Record | Include and exclude filters for branches, paths, or tags. |
| [`Parameter`](xref:Invex.StructuredText.AzureDevopsPipelines.DevopsPipelineModel.Supporting.Parameter) | Record | Pipeline template parameter definition. |
| [`Pool`](xref:Invex.StructuredText.AzureDevopsPipelines.DevopsPipelineModel.Supporting.Pool) | Union record | Specifies which pool to use for a job of the pipeline. |
| [`Schedule`](xref:Invex.StructuredText.AzureDevopsPipelines.DevopsPipelineModel.Supporting.Schedule) | Record | Scheduled trigger (cron). |
| [`Workspace`](xref:Invex.StructuredText.AzureDevopsPipelines.DevopsPipelineModel.Supporting.Workspace) | Record | Workspace options on the agent. |

### Invex.StructuredText.AzureDevopsPipelines.DevopsPipelineModel.Triggers

| Type | Kind | Description |
| --- | --- | --- |
| [`Pr`](xref:Invex.StructuredText.AzureDevopsPipelines.DevopsPipelineModel.Triggers.Pr) | Union record | A pull request trigger specifies which branches cause a pull request build to run. |
| [`Trigger`](xref:Invex.StructuredText.AzureDevopsPipelines.DevopsPipelineModel.Triggers.Trigger) | Union record | A push trigger specifies which branches cause a continuous integration build to run. |

### Invex.StructuredText.AzureDevopsPipelines.DevopsPipelineModel.Variables

| Type | Kind | Description |
| --- | --- | --- |
| [`Variable`](xref:Invex.StructuredText.AzureDevopsPipelines.DevopsPipelineModel.Variables.Variable) | Union record | A variable definition that can be a named variable, variable group, or template reference. |
| [`Variables`](xref:Invex.StructuredText.AzureDevopsPipelines.DevopsPipelineModel.Variables.Variables) | Union record | Define variables using name/value pairs or variable lists. |

## Typical flow

Build a model, write it with the platform writer, then read the text from the writer's `StructuredTextWriter`:

```csharp
var writer = new GithubActionWriter();           // or DependabotConfigWriter / DevopsPipelineWriter
writer.Write(workflow);                          // DependabotConfigWriter uses WriteConfig(config)

File.WriteAllText(".github/workflows/ci.yml", writer.TextWriter.ToString());
```

For concepts and guides, start with the [Introduction](../docs/introduction.md).
