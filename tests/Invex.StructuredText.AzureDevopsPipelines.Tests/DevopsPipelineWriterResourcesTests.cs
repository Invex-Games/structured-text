namespace Invex.StructuredText.AzureDevopsPipelines.Tests;

[TestFixture]
internal sealed class DevopsPipelineWriterResourcesTests
{
    private static DevopsPipeline.DevopsPipelineWithStages PipelineWithStageAndResources(Resources resources) =>
        new()
        {
            PipelineResources = resources,
            Stages =
            [
                new Stage.StageDefinition
                {
                    StageId = "Build",
                },
            ],
        };

    [Test]
    public void WriteResources_Builds_WritesBuildsSection()
    {
        var output = PipelineWriterHelper.Write(PipelineWithStageAndResources(new()
        {
            Builds =
            [
                new()
                {
                    Build = "myBuild",
                },
            ],
        }));

        output.ShouldContain("resources:");
        output.ShouldContain("builds:");
        output.ShouldContain("- build: myBuild");
    }

    [Test]
    public void WriteResources_Builds_WithAllOptions_WritesAllProperties()
    {
        var output = PipelineWriterHelper.Write(PipelineWithStageAndResources(new()
        {
            Builds =
            [
                new()
                {
                    Build = "myBuild",
                    Type = "Jenkins",
                    Connection = "myConnection",
                    Source = "myPipeline",
                    Version = "1.0",
                    Branch = "main",
                    Trigger = "true",
                },
            ],
        }));

        output.ShouldContain("- build: myBuild");
        output.ShouldContain("type: Jenkins");
        output.ShouldContain("connection: myConnection");
        output.ShouldContain("source: myPipeline");
        output.ShouldContain("version: 1.0");
        output.ShouldContain("branch: main");
        output.ShouldContain("trigger: true");
    }

    [Test]
    public void WriteResources_Containers_WritesContainersSection()
    {
        var output = PipelineWriterHelper.Write(PipelineWithStageAndResources(new()
        {
            Containers =
            [
                new()
                {
                    Container = "myImage",
                    Image = "ubuntu:latest",
                },
            ],
        }));

        output.ShouldContain("containers:");
        output.ShouldContain("- container: myImage");
        output.ShouldContain("image: ubuntu:latest");
    }

    [Test]
    public void WriteResources_Containers_WithAllOptions_WritesAllProperties()
    {
        var output = PipelineWriterHelper.Write(PipelineWithStageAndResources(new()
        {
            Containers =
            [
                new()
                {
                    Container = "myImage",
                    Image = "node:18",
                    Endpoint = "dockerHub",
                    Options = "--memory 2g",
                    Env = new Dictionary<string, Template>
                    {
                        ["NODE_ENV"] = "test",
                    },
                    Ports = ["8080:80"],
                    Volumes = ["/tmp:/tmp"],
                },
            ],
        }));

        output.ShouldContain("- container: myImage");
        output.ShouldContain("image: node:18");
        output.ShouldContain("endpoint: dockerHub");
        output.ShouldContain("options: --memory 2g");
        output.ShouldContain("env:");
        output.ShouldContain("NODE_ENV: test");
        output.ShouldContain("ports: [ 8080:80 ]");
        output.ShouldContain("volumes: [ /tmp:/tmp ]");
    }

    [Test]
    public void WriteResources_Containers_WithTrigger_WritesTriggerSection()
    {
        var output = PipelineWriterHelper.Write(PipelineWithStageAndResources(new()
        {
            Containers =
            [
                new()
                {
                    Container = "myImage",
                    Image = "ubuntu:latest",
                    Trigger = new()
                    {
                        Enabled = true,
                        Tags = new()
                        {
                            Include = ["v*"],
                        },
                    },
                },
            ],
        }));

        output.ShouldContain("trigger:");
        output.ShouldContain("enabled: true");
        output.ShouldContain("tags:");
        output.ShouldContain("include: [ v* ]");
    }

    // ── Resources: Pipelines ──────────────────────────────────────────────────

    [Test]
    public void WriteResources_Pipelines_WritesPipelinesSection()
    {
        var output = PipelineWriterHelper.Write(PipelineWithStageAndResources(new()
        {
            Pipelines =
            [
                new()
                {
                    Pipeline = "myUpstream",
                    Source = "My Upstream Pipeline",
                },
            ],
        }));

        output.ShouldContain("pipelines:");
        output.ShouldContain("- pipeline: myUpstream");
        output.ShouldContain("source: My Upstream Pipeline");
    }

    [Test]
    public void WriteResources_Pipelines_WithAllOptions_WritesAllProperties()
    {
        var output = PipelineWriterHelper.Write(PipelineWithStageAndResources(new()
        {
            Pipelines =
            [
                new()
                {
                    Pipeline = "myUpstream",
                    Source = "upstream-pipeline",
                    Project = "MyProject",
                    Version = "latest",
                    Branch = "main",
                    Trigger = new()
                    {
                        Enabled = true,
                        Branches = new()
                        {
                            Include = ["main"],
                        },
                        Tags = new()
                        {
                            Include = ["v*"],
                        },
                        Stages = ["Deploy"],
                    },
                },
            ],
        }));

        output.ShouldContain("- pipeline: myUpstream");
        output.ShouldContain("source: upstream-pipeline");
        output.ShouldContain("project: MyProject");
        output.ShouldContain("version: latest");
        output.ShouldContain("branch: main");
        output.ShouldContain("enabled: true");
        output.ShouldContain("stages: [ Deploy ]");
    }

    // ── Resources: Repositories ───────────────────────────────────────────────

    [Test]
    public void WriteResources_Repositories_WritesRepositoriesSection()
    {
        var output = PipelineWriterHelper.Write(PipelineWithStageAndResources(new()
        {
            Repositories =
            [
                new()
                {
                    Repository = "myRepo",
                    Type = "git",
                },
            ],
        }));

        output.ShouldContain("repositories:");
        output.ShouldContain("- repository: myRepo");
        output.ShouldContain("type: git");
    }

    [Test]
    public void WriteResources_Repositories_WithAllOptions_WritesAllProperties()
    {
        var output = PipelineWriterHelper.Write(PipelineWithStageAndResources(new()
        {
            Repositories =
            [
                new()
                {
                    Repository = "tools",
                    Type = "github",
                    Endpoint = "myGithubConnection",
                    Name = "myOrg/tools",
                    Ref = "refs/heads/main",
                },
            ],
        }));

        output.ShouldContain("- repository: tools");
        output.ShouldContain("type: github");
        output.ShouldContain("endpoint: myGithubConnection");
        output.ShouldContain("name: myOrg/tools");
        output.ShouldContain("ref: refs/heads/main");
    }

    // ── Resources: Webhooks ───────────────────────────────────────────────────

    [Test]
    public void WriteResources_Webhooks_WritesWebhooksSection()
    {
        var output = PipelineWriterHelper.Write(PipelineWithStageAndResources(new()
        {
            Webhooks =
            [
                new()
                {
                    Webhook = "myWebhook",
                    Connection = "IncomingWebhookConnection",
                },
            ],
        }));

        output.ShouldContain("webhooks:");
        output.ShouldContain("- webhook: myWebhook");
        output.ShouldContain("connection: IncomingWebhookConnection");
    }

    [Test]
    public void WriteResources_Webhooks_WithFilters_WritesFiltersSection()
    {
        var output = PipelineWriterHelper.Write(PipelineWithStageAndResources(new()
        {
            Webhooks =
            [
                new()
                {
                    Webhook = "myWebhook",
                    Connection = "conn",
                    Type = "Incoming",
                    Filters =
                    [
                        new()
                        {
                            Path = "$.action",
                            Value = "opened",
                        },
                    ],
                },
            ],
        }));

        output.ShouldContain("type: Incoming");
        output.ShouldContain("filters:");
        output.ShouldContain("- path: $.action");
        output.ShouldContain("value: opened");
    }

    // ── Resources: Packages ───────────────────────────────────────────────────

    [Test]
    public void WriteResources_Packages_WritesPackagesSection()
    {
        var output = PipelineWriterHelper.Write(PipelineWithStageAndResources(new()
        {
            Packages =
            [
                new()
                {
                    Package = "myPkg",
                    Type = "NuGet",
                },
            ],
        }));

        output.ShouldContain("packages:");
        output.ShouldContain("- package: myPkg");
        output.ShouldContain("type: NuGet");
    }

    [Test]
    public void WriteResources_Packages_WithAllOptions_WritesAllProperties()
    {
        var output = PipelineWriterHelper.Write(PipelineWithStageAndResources(new()
        {
            Packages =
            [
                new()
                {
                    Package = "myPkg",
                    Type = "NuGet",
                    Connection = "myFeed",
                    Name = "My.Package",
                    Version = "1.*",
                    Tag = "latest",
                },
            ],
        }));

        output.ShouldContain("- package: myPkg");
        output.ShouldContain("type: NuGet");
        output.ShouldContain("connection: myFeed");
        output.ShouldContain("name: My.Package");
        output.ShouldContain("version: 1.*");
        output.ShouldContain("tag: latest");
    }

    // ── Resources: Null ───────────────────────────────────────────────────────

    [Test]
    public void WriteResources_Null_WritesNothing()
    {
        var output = PipelineWriterHelper.Write(new DevopsPipeline.DevopsPipelineWithStages
        {
            PipelineResources = null,
            Stages =
            [
                new Stage.StageDefinition
                {
                    StageId = "Build",
                },
            ],
        });

        output.ShouldNotContain("resources:");
    }
}
