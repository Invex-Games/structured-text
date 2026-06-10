namespace Invex.StructuredText.GithubActions.GithubActionModel;

/// <summary>
///     A workflow trigger event (<c>on</c>). This is a discriminated union over the GitHub webhook events;
///     construct the variant for the event you want, e.g. <c>new On.Push { ... }</c> or
///     <c>new On.Schedule(["0 6 * * 1"])</c>. Enum member names intentionally match the
///     lowercase/snake_case activity-type strings used in workflow YAML.
/// </summary>
[PublicAPI]
[Union]
[SuppressMessage("ReSharper", "InconsistentNaming")]
public partial record On
{
    /// <summary>
    ///     Triggers when a branch protection rule changes (<c>branch_protection_rule</c>).
    /// </summary>
    /// <param name="Types">The activity types to trigger on.</param>
    [PublicAPI]
    partial record BranchProtectionRule(IReadOnlyList<BranchProtectionRule.BranchProtectionType> Types)
    {
        /// <summary>
        ///     Activity types for the <c>branch_protection_rule</c> event.
        /// </summary>
        [PublicAPI]
        public enum BranchProtectionType
        {
            created,
            edited,
            deleted,
        }
    }

    /// <summary>
    ///     Triggers on check run activity (<c>check_run</c>).
    /// </summary>
    /// <param name="Types">The activity types to trigger on.</param>
    [PublicAPI]
    partial record CheckRun(IReadOnlyList<CheckRun.CheckRunType> Types)
    {
        /// <summary>
        ///     Activity types for the <c>check_run</c> event.
        /// </summary>
        [PublicAPI]
        public enum CheckRunType
        {
            created,
            edited,
            deleted,
        }
    }

    /// <summary>
    ///     Triggers on check suite activity (<c>check_suite</c>).
    /// </summary>
    /// <param name="Types">The activity types to trigger on.</param>
    [PublicAPI]
    partial record CheckSuite(IReadOnlyList<CheckSuite.CheckSuiteType> Types)
    {
        /// <summary>
        ///     Activity types for the <c>check_suite</c> event.
        /// </summary>
        [PublicAPI]
        public enum CheckSuiteType
        {
            completed,
        }
    }

    /// <summary>
    ///     Triggers when a branch or tag is created (<c>create</c>).
    /// </summary>
    [PublicAPI]
    partial record Create;

    /// <summary>
    ///     Triggers when a branch or tag is deleted (<c>delete</c>).
    /// </summary>
    [PublicAPI]
    partial record Delete;

    /// <summary>
    ///     Triggers when a deployment is created (<c>deployment</c>).
    /// </summary>
    [PublicAPI]
    partial record Deployment;

    /// <summary>
    ///     Triggers when a deployment status is provided (<c>deployment_status</c>).
    /// </summary>
    [PublicAPI]
    partial record DeploymentStatus;

    /// <summary>
    ///     Triggers on discussion activity (<c>discussion</c>).
    /// </summary>
    /// <param name="Types">The activity types to trigger on.</param>
    [PublicAPI]
    partial record Discussion(IReadOnlyList<Discussion.DiscussionType> Types)
    {
        /// <summary>
        ///     Activity types for the <c>discussion</c> event.
        /// </summary>
        [PublicAPI]
        public enum DiscussionType
        {
            created,
            edited,
            deleted,
            transferred,
            pinned,
            unpinned,
            labeled,
            unlabeled,
            locked,
            unlocked,
            category_changed,
            answered,
            unanswered,
        }
    }

    /// <summary>
    ///     Triggers on discussion comment activity (<c>discussion_comment</c>).
    /// </summary>
    /// <param name="Types">The activity types to trigger on.</param>
    [PublicAPI]
    partial record DiscussionComment(IReadOnlyList<DiscussionComment.DiscussionCommentType> Types)
    {
        /// <summary>
        ///     Activity types for the <c>discussion_comment</c> event.
        /// </summary>
        [PublicAPI]
        public enum DiscussionCommentType
        {
            created,
            edited,
            deleted,
        }
    }

    /// <summary>
    ///     Triggers when the repository is forked (<c>fork</c>).
    /// </summary>
    [PublicAPI]
    partial record Fork;

    /// <summary>
    ///     Triggers when the wiki is updated (<c>gollum</c>).
    /// </summary>
    [PublicAPI]
    partial record Gollum;

    /// <summary>
    ///     Triggers on runner image version events (<c>image_version</c>).
    /// </summary>
    [PublicAPI]
    partial record ImageVersion
    {
        /// <summary>
        ///     The image names to trigger on.
        /// </summary>
        public required IReadOnlyList<string>? Names { get; init; }

        /// <summary>
        ///     The image versions to trigger on.
        /// </summary>
        public required IReadOnlyList<string>? Versions { get; init; }
    }

    /// <summary>
    ///     Triggers on issue/pull-request comment activity (<c>issue_comment</c>).
    /// </summary>
    /// <param name="Types">The activity types to trigger on.</param>
    [PublicAPI]
    partial record IssueComment(IReadOnlyList<IssueComment.IssueCommentType> Types)
    {
        /// <summary>
        ///     Activity types for the <c>issue_comment</c> event.
        /// </summary>
        [PublicAPI]
        public enum IssueCommentType
        {
            created,
            edited,
            deleted,
        }
    }

    /// <summary>
    ///     Triggers on issue activity (<c>issues</c>).
    /// </summary>
    /// <param name="Types">The activity types to trigger on.</param>
    [PublicAPI]
    partial record Issues(IReadOnlyList<Issues.IssuesType> Types)
    {
        /// <summary>
        ///     Activity types for the <c>issues</c> event.
        /// </summary>
        [PublicAPI]
        public enum IssuesType
        {
            opened,
            edited,
            deleted,
            transferred,
            pinned,
            unpinned,
            closed,
            reopened,
            assigned,
            unassigned,
            labeled,
            unlabeled,
            locked,
            unlocked,
            milestoned,
            demilestoned,
            typed,
            untyped,
        }
    }

    /// <summary>
    ///     Triggers on label activity (<c>label</c>).
    /// </summary>
    /// <param name="Types">The activity types to trigger on.</param>
    [PublicAPI]
    partial record Label(IReadOnlyList<Label.LabelType> Types)
    {
        /// <summary>
        ///     Activity types for the <c>label</c> event.
        /// </summary>
        [PublicAPI]
        public enum LabelType
        {
            created,
            edited,
        }
    }

    /// <summary>
    ///     Triggers when a pull request is added to a merge queue (<c>merge_group</c>).
    /// </summary>
    /// <param name="Types">The activity types to trigger on.</param>
    [PublicAPI]
    partial record MergeGroup(IReadOnlyList<MergeGroup.MergeGroupType> Types)
    {
        /// <summary>
        ///     Activity types for the <c>merge_group</c> event.
        /// </summary>
        [PublicAPI]
        public enum MergeGroupType
        {
            checks_requested,
        }
    }

    /// <summary>
    ///     Triggers on milestone activity (<c>milestone</c>).
    /// </summary>
    /// <param name="Types">The activity types to trigger on.</param>
    [PublicAPI]
    partial record Milestone(IReadOnlyList<Milestone.MilestoneType> Types)
    {
        /// <summary>
        ///     Activity types for the <c>milestone</c> event.
        /// </summary>
        [PublicAPI]
        public enum MilestoneType
        {
            created,
            closed,
            opened,
            edited,
            deleted,
        }
    }

    /// <summary>
    ///     Triggers when someone pushes to the GitHub Pages publishing branch (<c>page_build</c>).
    /// </summary>
    [PublicAPI]
    partial record PageBuild;

    /// <summary>
    ///     Triggers on classic project activity (<c>project</c>).
    /// </summary>
    /// <param name="Types">The activity types to trigger on.</param>
    [PublicAPI]
    partial record Project(IReadOnlyList<Project.ProjectType> Types)
    {
        /// <summary>
        ///     Activity types for the <c>project</c> event.
        /// </summary>
        [PublicAPI]
        public enum ProjectType
        {
            created,
            updated,
            closed,
            reopened,
            edited,
            deleted,
        }
    }

    /// <summary>
    ///     Triggers on classic project card activity (<c>project_card</c>).
    /// </summary>
    /// <param name="Types">The activity types to trigger on.</param>
    [PublicAPI]
    partial record ProjectCard(IReadOnlyList<ProjectCard.ProjectCardType> Types)
    {
        /// <summary>
        ///     Activity types for the <c>project_card</c> event.
        /// </summary>
        [PublicAPI]
        public enum ProjectCardType
        {
            created,
            moved,
            converted,
            edited,
            deleted,
        }
    }

    /// <summary>
    ///     Triggers on classic project column activity (<c>project_column</c>).
    /// </summary>
    /// <param name="Types">The activity types to trigger on.</param>
    [PublicAPI]
    partial record ProjectColumn(IReadOnlyList<ProjectColumn.ProjectColumnType> Types)
    {
        /// <summary>
        ///     Activity types for the <c>project_column</c> event.
        /// </summary>
        [PublicAPI]
        public enum ProjectColumnType
        {
            created,
            updated,
            moved,
            deleted,
        }
    }

    /// <summary>
    ///     Triggers when the repository changes from private to public (<c>public</c>).
    /// </summary>
    [PublicAPI]
    partial record Public;

    /// <summary>
    ///     Triggers on pull request activity (<c>pull_request</c>), with optional branch/tag/path filters.
    /// </summary>
    /// <param name="Types">The activity types to trigger on.</param>
    [PublicAPI]
    partial record PullRequest(IReadOnlyList<PullRequest.PullRequestType> Types)
    {
        /// <summary>
        ///     Activity types for the <c>pull_request</c> event.
        /// </summary>
        [PublicAPI]
        public enum PullRequestType
        {
            assigned,
            unassigned,
            labeled,
            unlabeled,
            opened,
            edited,
            closed,
            reopened,
            synchronized,
            converted_to_draft,
            ready_for_review,
            locked,
            unlocked,
            milestoned,
            demilestoned,
            review_requested,
            review_request_removed,
            auto_merge_enabled,
            auto_merge_disabled,
            enqueued,
            dequeued,
        }

        /// <summary>
        ///     Target branch patterns to include (<c>branches</c>).
        /// </summary>
        public required IReadOnlyList<string>? Branches { get; init; }

        /// <summary>
        ///     Target branch patterns to exclude (<c>branches-ignore</c>).
        /// </summary>
        public required IReadOnlyList<string>? BranchesIgnore { get; init; }

        /// <summary>
        ///     Tag patterns to include (<c>tags</c>).
        /// </summary>
        public required IReadOnlyList<string>? Tags { get; init; }

        /// <summary>
        ///     Tag patterns to exclude (<c>tags-ignore</c>).
        /// </summary>
        public required IReadOnlyList<string>? TagsIgnore { get; init; }

        /// <summary>
        ///     Changed-file path patterns to include (<c>paths</c>).
        /// </summary>
        public required IReadOnlyList<string>? Paths { get; init; }

        /// <summary>
        ///     Changed-file path patterns to exclude (<c>paths-ignore</c>).
        /// </summary>
        public required IReadOnlyList<string>? PathsIgnore { get; init; }
    }

    /// <summary>
    ///     Triggers on pull request review activity (<c>pull_request_review</c>).
    /// </summary>
    /// <param name="Types">The activity types to trigger on.</param>
    [PublicAPI]
    partial record PullRequestReview(IReadOnlyList<PullRequestReview.PullRequestReviewType> Types)
    {
        /// <summary>
        ///     Activity types for the <c>pull_request_review</c> event.
        /// </summary>
        [PublicAPI]
        public enum PullRequestReviewType
        {
            submitted,
            edited,
            dismissed,
        }
    }

    /// <summary>
    ///     Triggers on pull request review comment activity (<c>pull_request_review_comment</c>).
    /// </summary>
    /// <param name="Types">The activity types to trigger on.</param>
    [PublicAPI]
    partial record PullRequestReviewComment(IReadOnlyList<PullRequestReviewComment.PullRequestReviewCommentType> Types)
    {
        /// <summary>
        ///     Activity types for the <c>pull_request_review_comment</c> event.
        /// </summary>
        [PublicAPI]
        public enum PullRequestReviewCommentType
        {
            created,
            edited,
            deleted,
        }
    }

    /// <summary>
    ///     Triggers on pull request activity, running in the context of the base of the pull request
    ///     (<c>pull_request_target</c>). Use with care — the workflow has access to repository secrets.
    /// </summary>
    /// <param name="Types">The activity types to trigger on.</param>
    [PublicAPI]
    partial record PullRequestTarget(IReadOnlyList<PullRequestTarget.PullRequestTargetType> Types)
    {
        /// <summary>
        ///     Activity types for the <c>pull_request_target</c> event.
        /// </summary>
        [PublicAPI]
        public enum PullRequestTargetType
        {
            assigned,
            unassigned,
            labeled,
            unlabeled,
            opened,
            edited,
            closed,
            reopened,
            synchronized,
            converted_to_draft,
            ready_for_review,
            locked,
            unlocked,
            milestoned,
            demilestoned,
            review_requested,
            review_request_removed,
            auto_merge_enabled,
            auto_merge_disabled,
            enqueued,
            dequeued,
        }
    }

    /// <summary>
    ///     Triggers when commits or tags are pushed (<c>push</c>), with optional branch/tag/path filters.
    ///     All filter properties are required so that omissions are explicit; pass <c>null</c> for filters
    ///     you do not want.
    /// </summary>
    [PublicAPI]
    partial record Push
    {
        /// <summary>
        ///     Branch patterns to include (<c>branches</c>).
        /// </summary>
        public required IReadOnlyList<string>? Branches { get; init; }

        /// <summary>
        ///     Branch patterns to exclude (<c>branches-ignore</c>).
        /// </summary>
        public required IReadOnlyList<string>? BranchesIgnore { get; init; }

        /// <summary>
        ///     Tag patterns to include (<c>tags</c>).
        /// </summary>
        public required IReadOnlyList<string>? Tags { get; init; }

        /// <summary>
        ///     Tag patterns to exclude (<c>tags-ignore</c>).
        /// </summary>
        public required IReadOnlyList<string>? TagsIgnore { get; init; }

        /// <summary>
        ///     Changed-file path patterns to include (<c>paths</c>).
        /// </summary>
        public required IReadOnlyList<string>? Paths { get; init; }

        /// <summary>
        ///     Changed-file path patterns to exclude (<c>paths-ignore</c>).
        /// </summary>
        public required IReadOnlyList<string>? PathsIgnore { get; init; }
    }

    /// <summary>
    ///     Triggers on registry package activity (<c>registry_package</c>).
    /// </summary>
    /// <param name="Types">The activity types to trigger on.</param>
    [PublicAPI]
    partial record RegistryPackage(IReadOnlyList<RegistryPackage.RegistryPackageType> Types)
    {
        /// <summary>
        ///     Activity types for the <c>registry_package</c> event.
        /// </summary>
        [PublicAPI]
        public enum RegistryPackageType
        {
            published,
            updated,
        }
    }

    /// <summary>
    ///     Triggers on release activity (<c>release</c>).
    /// </summary>
    /// <param name="Types">The activity types to trigger on.</param>
    [PublicAPI]
    partial record Release(IReadOnlyList<Release.ReleaseType> Types)
    {
        /// <summary>
        ///     Activity types for the <c>release</c> event.
        /// </summary>
        [PublicAPI]
        public enum ReleaseType
        {
            published,
            unpublished,
            created,
            edited,
            deleted,
            prereleased,
            released,
        }
    }

    /// <summary>
    ///     Triggers on a custom webhook event sent via the GitHub API (<c>repository_dispatch</c>).
    /// </summary>
    /// <param name="Types">The custom event types to trigger on.</param>
    [PublicAPI]
    partial record RepositoryDispatch(IReadOnlyList<string> Types);

    /// <summary>
    ///     Triggers on a schedule (<c>schedule</c>).
    /// </summary>
    /// <param name="Crons">POSIX cron expressions, e.g. <c>"0 6 * * 1"</c>.</param>
    [PublicAPI]
    partial record Schedule(IReadOnlyList<string> Crons);

    /// <summary>
    ///     Triggers when the status of a commit changes (<c>status</c>).
    /// </summary>
    [PublicAPI]
    partial record Status;

    /// <summary>
    ///     Triggers when the repository is starred (<c>watch</c>).
    /// </summary>
    /// <param name="Types">The activity types to trigger on.</param>
    [PublicAPI]
    partial record Watch(params Watch.WatchType[] Types)
    {
        /// <summary>
        ///     Activity types for the <c>watch</c> event.
        /// </summary>
        [PublicAPI]
        public enum WatchType
        {
            started,
        }
    }

    /// <summary>
    ///     Allows the workflow to be called from another workflow (<c>workflow_call</c>),
    ///     making it a reusable workflow.
    /// </summary>
    [PublicAPI]
    partial record WorkflowCall;

    /// <summary>
    ///     Allows the workflow to be run manually from the GitHub UI or API (<c>workflow_dispatch</c>).
    /// </summary>
    /// <param name="Inputs">The typed inputs the user can provide when dispatching.</param>
    [PublicAPI]
    partial record WorkflowDispatch(IReadOnlyList<WorkflowDispatchInput> Inputs);

    /// <summary>
    ///     Triggers when another workflow runs (<c>workflow_run</c>).
    /// </summary>
    [PublicAPI]
    partial record WorkflowRun
    {
        /// <summary>
        ///     Activity types for the <c>workflow_run</c> event.
        /// </summary>
        [PublicAPI]
        public enum WorkflowDispatchTypes
        {
            requested,
            completed,
            in_progress,
        }

        /// <summary>
        ///     The names of the workflows to watch.
        /// </summary>
        public required IReadOnlyList<string>? Workflows { get; init; }

        /// <summary>
        ///     Branch patterns the watched workflow must run on.
        /// </summary>
        public required IReadOnlyList<string>? Branches { get; init; }

        /// <summary>
        ///     The activity types to trigger on.
        /// </summary>
        public required IReadOnlyList<WorkflowDispatchTypes>? Types { get; init; }
    }
}

/// <summary>
///     A typed input for the <c>workflow_dispatch</c> trigger. This is a discriminated union;
///     construct <see cref="String" />, <see cref="Number" />, <see cref="Boolean" />, or
///     <see cref="Choice" /> for the desired input type.
/// </summary>
[PublicAPI]
[Union]
public partial record WorkflowDispatchInput
{
    /// <summary>
    ///     The input name, used as the YAML key under <c>inputs:</c>.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    ///     A description shown in the dispatch UI.
    /// </summary>
    public required string? Description { get; init; }

    /// <summary>
    ///     Whether the input must be provided when dispatching.
    /// </summary>
    public required bool? Required { get; init; }

    /// <summary>
    ///     The default value used when the input is not provided.
    /// </summary>
    public required string? Default { get; init; }

    /// <summary>
    ///     The YAML <c>type</c> value of the input, supplied by each variant.
    /// </summary>
    public abstract string Type { get; }

    /// <summary>
    ///     A free-text input (<c>type: string</c>).
    /// </summary>
    [PublicAPI]
    public partial record String
    {
        /// <inheritdoc />
        public override string Type => "string";
    }

    /// <summary>
    ///     A numeric input (<c>type: number</c>).
    /// </summary>
    [PublicAPI]
    public partial record Number
    {
        /// <inheritdoc />
        public override string Type => "number";
    }

    /// <summary>
    ///     A true/false input (<c>type: boolean</c>).
    /// </summary>
    [PublicAPI]
    public partial record Boolean
    {
        /// <inheritdoc />
        public override string Type => "boolean";
    }

    /// <summary>
    ///     A dropdown input restricted to a set of options (<c>type: choice</c>).
    /// </summary>
    [PublicAPI]
    public partial record Choice
    {
        /// <inheritdoc />
        public override string Type => "choice";

        /// <summary>
        ///     The selectable options.
        /// </summary>
        public required IReadOnlyList<string>? Options { get; init; }
    }
}
