# API Reference

This section is the generated reference for every public type in the three Invex.StructuredText packages, built from
the XML documentation comments in the source code; the tables below list each type by package and namespace. Union
records are [Dunet](https://github.com/domn1995/dunet) discriminated unions: construct one of their nested variant
records (for example `new Step.RunStep { ... }`).

## Invex.StructuredText

[NuGet](https://www.nuget.org/packages/Invex.StructuredText). The core: `StructuredTextWriter` and the platform-agnostic expression API.

### Invex.StructuredText

| Type | Kind | Description |
| --- | --- | --- |
| [`StructuredTextWriter`](xref:Invex.StructuredText.StructuredTextWriter) | Class | An indentation-aware text writer for producing structured text such as YAML. |

### Invex.StructuredText.Expressions

Start with `Tex`: every way to create an expression is one of its members. See [Expressions](../docs/expressions.md).

| Type | Kind | Description |
| --- | --- | --- |
| [`BoolCast`](xref:Invex.StructuredText.Expressions.BoolCast) | Record | Treats `Operand` as a boolean. Rendered transparently. |
| [`BoolContext`](xref:Invex.StructuredText.Expressions.BoolContext) | Record | A boolean-valued context path. |
| [`BoolExpr`](xref:Invex.StructuredText.Expressions.BoolExpr) | Abstract record | An expression that evaluates to a boolean. |
| [`BoolFunction`](xref:Invex.StructuredText.Expressions.BoolFunction) | Record | A function call that returns a boolean. |
| [`BoolLiteral`](xref:Invex.StructuredText.Expressions.BoolLiteral) | Record | A boolean literal. |
| [`ComparisonExpr`](xref:Invex.StructuredText.Expressions.ComparisonExpr) | Record | A comparison between two values. |
| [`ComparisonOperator`](xref:Invex.StructuredText.Expressions.ComparisonOperator) | Enum | The comparison operators. |
| [`Expr`](xref:Invex.StructuredText.Expressions.Expr) | Abstract record | The base type of the typed expression tree. Every node derives from one of the four result-typed roots: `BoolExpr`, `StringExpr`, `NumberExpr` or `JsonExpr`. |
| [`ExpressionPrinter`](xref:Invex.StructuredText.Expressions.ExpressionPrinter) | Abstract class | Renders typed expression trees (`Expr`) and `Template`s to platform syntax. |
| [`FunctionNames`](xref:Invex.StructuredText.Expressions.FunctionNames) | Static class | Canonical names of the functions the core knows about. Printers map them to the platform's own names. |
| [`IContextPath`](xref:Invex.StructuredText.Expressions.IContextPath) | Interface | Implemented by nodes that reference a dotted context path such as `a.b.c`. |
| [`IndexAccessExpr`](xref:Invex.StructuredText.Expressions.IndexAccessExpr) | Record | Accesses element `Index` of `Target`. |
| [`InputRef`](xref:Invex.StructuredText.Expressions.InputRef) | Record | An input (parameter) supplied when the run was started. |
| [`IRawExpr`](xref:Invex.StructuredText.Expressions.IRawExpr) | Interface | Implemented by nodes that hold verbatim expression text. |
| [`JobOutputRef`](xref:Invex.StructuredText.Expressions.JobOutputRef) | Record | An output of another job that the current job depends on. |
| [`JobResultRef`](xref:Invex.StructuredText.Expressions.JobResultRef) | Record | The result of another job that the current job depends on. Compare with `ResultLiteral`. |
| [`JsonContext`](xref:Invex.StructuredText.Expressions.JsonContext) | Record | A context path whose value is an object, an array or of unknown type. |
| [`JsonExpr`](xref:Invex.StructuredText.Expressions.JsonExpr) | Abstract record | An expression whose value is an object, an array, or of a type not known statically. |
| [`JsonFunction`](xref:Invex.StructuredText.Expressions.JsonFunction) | Record | A function call that returns an object, an array or a value of unknown type. |
| [`LogicalExpr`](xref:Invex.StructuredText.Expressions.LogicalExpr) | Record | An AND or OR over two or more conditions. Nested nodes with the same operator are flattened. |
| [`LogicalOperator`](xref:Invex.StructuredText.Expressions.LogicalOperator) | Enum | The logical operators. |
| [`MatrixRef`](xref:Invex.StructuredText.Expressions.MatrixRef) | Record | The current value of a matrix dimension. |
| [`MemberAccessExpr`](xref:Invex.StructuredText.Expressions.MemberAccessExpr) | Record | Accesses `Property` of `Target`. |
| [`NotExpr`](xref:Invex.StructuredText.Expressions.NotExpr) | Record | The logical negation of `Operand`. |
| [`NullLiteral`](xref:Invex.StructuredText.Expressions.NullLiteral) | Record | The null literal. |
| [`NumberCast`](xref:Invex.StructuredText.Expressions.NumberCast) | Record | Treats `Operand` as a number. Rendered transparently. |
| [`NumberContext`](xref:Invex.StructuredText.Expressions.NumberContext) | Record | A number-valued context path. |
| [`NumberExpr`](xref:Invex.StructuredText.Expressions.NumberExpr) | Abstract record | An expression that evaluates to a number. |
| [`NumberLiteral`](xref:Invex.StructuredText.Expressions.NumberLiteral) | Record | A numeric literal, rendered with the invariant culture. |
| [`RawExpr`](xref:Invex.StructuredText.Expressions.RawExpr) | Record | Verbatim expression text, created by `Raw`. It is a condition, and converts implicitly to string, number and JSON expressions. |
| [`RawJson`](xref:Invex.StructuredText.Expressions.RawJson) | Record | Verbatim expression text of an object, array or unknown type. |
| [`RawNumber`](xref:Invex.StructuredText.Expressions.RawNumber) | Record | Verbatim number-valued expression text. |
| [`RawString`](xref:Invex.StructuredText.Expressions.RawString) | Record | Verbatim string-valued expression text. |
| [`ReferenceExpr`](xref:Invex.StructuredText.Expressions.ReferenceExpr) | Abstract record | A reference to a value every supported platform provides (a secret, variable, input, matrix value or output). Each platform's printer decides the syntax. |
| [`ResultLiteral`](xref:Invex.StructuredText.Expressions.ResultLiteral) | Record | A result value, rendered with the platform's own spelling. |
| [`RunResult`](xref:Invex.StructuredText.Expressions.RunResult) | Enum | The possible results of a job or step. |
| [`SecretRef`](xref:Invex.StructuredText.Expressions.SecretRef) | Record | A secret. |
| [`StepOutputRef`](xref:Invex.StructuredText.Expressions.StepOutputRef) | Record | An output of an earlier step in the current job. |
| [`StringCast`](xref:Invex.StructuredText.Expressions.StringCast) | Record | Treats `Operand` as a string. Rendered transparently. |
| [`StringContext`](xref:Invex.StructuredText.Expressions.StringContext) | Record | A string-valued context path. |
| [`StringExpr`](xref:Invex.StructuredText.Expressions.StringExpr) | Abstract record | An expression that evaluates to a string. |
| [`StringFunction`](xref:Invex.StructuredText.Expressions.StringFunction) | Record | A function call that returns a string. |
| [`StringLiteral`](xref:Invex.StructuredText.Expressions.StringLiteral) | Record | A string literal, rendered quoted and escaped. |
| [`Template`](xref:Invex.StructuredText.Expressions.Template) | Class | A text value made of literal text and embedded expressions, such as a YAML scalar. |
| [`TemplatePart`](xref:Invex.StructuredText.Expressions.TemplatePart) | Record struct | One part of a `Template`: either literal text or an embedded expression. |
| [`Tex`](xref:Invex.StructuredText.Expressions.Tex) | Static class | The starting point for writing expressions. Everything that creates an expression from scratch lives here: references (`Secret`, `JobOutput`, …), status checks (`Success`, …), functions (`All`, `Format`, …) and the escape hatch `Raw`. Platform packages add their own members, such as `Tex.GitHub` for the GitHub Actions contexts. |
| [`VariableRef`](xref:Invex.StructuredText.Expressions.VariableRef) | Record | A configuration variable defined outside the workflow. |

## Invex.StructuredText.GithubActions

[NuGet](https://www.nuget.org/packages/Invex.StructuredText.GithubActions). GitHub Actions workflow and Dependabot configuration generation.

### Invex.StructuredText.GithubActions

| Type | Kind | Description |
| --- | --- | --- |
| [`DependabotConfigWriter`](xref:Invex.StructuredText.GithubActions.DependabotConfigWriter) | Class | Serializes a `DependabotConfig` model to Dependabot v2 configuration YAML (`.github/dependabot.yml`). |
| [`GithubActionWriter`](xref:Invex.StructuredText.GithubActions.GithubActionWriter) | Class | Serializes a `GithubAction` model to GitHub Actions workflow YAML. Expressions embedded in the model are rendered with `GithubExpressionPrinter`. |
| [`GitHubContexts`](xref:Invex.StructuredText.GithubActions.GitHubContexts) | Class | The GitHub Actions contexts, reached through `Tex.GitHub`. Generated from `Contexts/github.contexts`; nested contexts such as `Tex.GitHub.Event.PullRequest` are `GitHubContexts…Context` classes. |
| [`GithubExpressionPrinter`](xref:Invex.StructuredText.GithubActions.GithubExpressionPrinter) | Class | Renders typed expressions (`Expr`) and `Template`s to GitHub Actions expression syntax: infix operators (`==`, `&&`, `!`), single-quoted string literals, and `${{ … }}` around expressions embedded in text. |
| [`TexGitHubExtensions`](xref:Invex.StructuredText.GithubActions.TexGitHubExtensions) | Static class | Adds the GitHub Actions contexts to `Tex`. |

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
| [`DevopsAgentVariables`](xref:Invex.StructuredText.AzureDevopsPipelines.DevopsAgentVariables) | Class | `Agent.*` predefined variables. |
| [`DevopsBuildVariables`](xref:Invex.StructuredText.AzureDevopsPipelines.DevopsBuildVariables) | Class | `Build.*` predefined variables. |
| [`DevopsContexts`](xref:Invex.StructuredText.AzureDevopsPipelines.DevopsContexts) | Class | Azure DevOps predefined variables and run-time expressions, reached through `Tex.DevOps`. Each variable is a `Variable` reference: `$(Build.BuildId)` in a template and `variables['Build.BuildId']` in a condition. |
| [`DevopsExpressionPrinter`](xref:Invex.StructuredText.AzureDevopsPipelines.DevopsExpressionPrinter) | Class | Renders typed expressions (`Expr`) and `Template`s to Azure DevOps syntax. Operators are functions (`and(...)`, `or(...)`, `not(...)`, `eq(...)`, …) and status checks are `succeeded()`, `failed()`, `canceled()` and `always()`. |
| [`DevopsPipelineWriter`](xref:Invex.StructuredText.AzureDevopsPipelines.DevopsPipelineWriter) | Class | Serializes a `DevopsPipeline` model to Azure Pipelines YAML. Expressions embedded in the model are rendered with `DevopsExpressionPrinter`. |
| [`DevopsRuntimeExpr`](xref:Invex.StructuredText.AzureDevopsPipelines.DevopsRuntimeExpr) | Record | Marks an expression to be evaluated at run time. Embedded in a `Template`, it renders as `$[ … ]` instead of the compile-time `${{ … }}`. Create one with `Tex.DevOps.Runtime(…)`. |
| [`DevopsSystemVariables`](xref:Invex.StructuredText.AzureDevopsPipelines.DevopsSystemVariables) | Class | `System.*` predefined variables. |
| [`TexDevopsExtensions`](xref:Invex.StructuredText.AzureDevopsPipelines.TexDevopsExtensions) | Static class | Adds the Azure DevOps predefined variables and run-time expressions to `Tex`. |

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
