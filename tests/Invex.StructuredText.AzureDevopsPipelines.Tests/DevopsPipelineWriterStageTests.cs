namespace Invex.StructuredText.AzureDevopsPipelines.Tests;

[TestFixture]
internal sealed class DevopsPipelineWriterStageTests
{
    // ── StageDefinition ───────────────────────────────────────────────────────

    [Test]
    public void WriteStage_StageDefinition_Minimal_WritesStageId()
    {
        Stage stage = new Stage.StageDefinition
        {
            StageId = "BuildStage",
        };

        var output = PipelineWriterHelper.Write(new DevopsPipeline.DevopsPipelineWithStages
        {
            Stages = [stage],
        });

        output.ShouldContain("- stage: BuildStage");
    }

    [Test]
    public void WriteStage_StageDefinition_WithDisplayName_WritesDisplayName()
    {
        Stage stage = new Stage.StageDefinition
        {
            StageId = "BuildStage",
            DisplayName = "Build Application",
        };

        var output = PipelineWriterHelper.Write(new DevopsPipeline.DevopsPipelineWithStages
        {
            Stages = [stage],
        });

        output.ShouldContain("displayName: Build Application");
    }

    [Test]
    public void WriteStage_StageDefinition_WithGroup_WritesGroup()
    {
        Stage stage = new Stage.StageDefinition
        {
            StageId = "BuildStage",
            Group = "BuildGroup",
        };

        var output = PipelineWriterHelper.Write(new DevopsPipeline.DevopsPipelineWithStages
        {
            Stages = [stage],
        });

        output.ShouldContain("group: BuildGroup");
    }

    [Test]
    public void WriteStage_StageDefinition_WithDependsOn_WritesDependsOnList()
    {
        Stage stage = new Stage.StageDefinition
        {
            StageId = "TestStage",
            DependsOn = ["BuildStage"],
        };

        var output = PipelineWriterHelper.Write(new DevopsPipeline.DevopsPipelineWithStages
        {
            Stages = [stage],
        });

        output.ShouldContain("dependsOn: [ BuildStage ]");
    }

    [Test]
    public void WriteStage_StageDefinition_WithCondition_WritesCondition()
    {
        Stage stage = new Stage.StageDefinition
        {
            StageId = "DeployStage",
            Condition = Tex.Raw("succeeded()"),
        };

        var output = PipelineWriterHelper.Write(new DevopsPipeline.DevopsPipelineWithStages
        {
            Stages = [stage],
        });

        output.ShouldContain("condition: succeeded()");
    }

    [Test]
    public void WriteStage_StageDefinition_WithPool_WritesPool()
    {
        Stage stage = new Stage.StageDefinition
        {
            StageId = "BuildStage",
            Pool = new Pool.PoolSpec
            {
                VmImage = "ubuntu-latest",
            },
        };

        var output = PipelineWriterHelper.Write(new DevopsPipeline.DevopsPipelineWithStages
        {
            Stages = [stage],
        });

        output.ShouldContain("pool:");
        output.ShouldContain("vmImage: ubuntu-latest");
    }

    [Test]
    public void WriteStage_StageDefinition_WithLockBehavior_WritesLockBehavior()
    {
        Stage stage = new Stage.StageDefinition
        {
            StageId = "DeployStage",
            LockBehavior = "sequential",
        };

        var output = PipelineWriterHelper.Write(new DevopsPipeline.DevopsPipelineWithStages
        {
            Stages = [stage],
        });

        output.ShouldContain("lockBehavior: sequential");
    }

    [Test]
    public void WriteStage_StageDefinition_WithTrigger_WritesTrigger()
    {
        Stage stage = new Stage.StageDefinition
        {
            StageId = "ManualStage",
            Trigger = "manual",
        };

        var output = PipelineWriterHelper.Write(new DevopsPipeline.DevopsPipelineWithStages
        {
            Stages = [stage],
        });

        output.ShouldContain("trigger: manual");
    }

    [Test]
    public void WriteStage_StageDefinition_WithIsSkippable_WritesIsSkippable()
    {
        Stage stage = new Stage.StageDefinition
        {
            StageId = "RequiredStage",
            IsSkippable = false,
        };

        var output = PipelineWriterHelper.Write(new DevopsPipeline.DevopsPipelineWithStages
        {
            Stages = [stage],
        });

        output.ShouldContain("isSkippable: false");
    }

    [Test]
    public void WriteStage_StageDefinition_WithTemplateContext_WritesTemplateContext()
    {
        Stage stage = new Stage.StageDefinition
        {
            StageId = "BuildStage",
            TemplateContext = new Dictionary<string, Template>
            {
                ["region"] = "eastus",
            },
        };

        var output = PipelineWriterHelper.Write(new DevopsPipeline.DevopsPipelineWithStages
        {
            Stages = [stage],
        });

        output.ShouldContain("templateContext:");
        output.ShouldContain("region: eastus");
    }

    [Test]
    public void WriteStage_StageDefinition_WithVariables_WritesVariables()
    {
        Stage stage = new Stage.StageDefinition
        {
            StageId = "BuildStage",
            Variables = new Variables.Dictionary
            {
                Values = new Dictionary<string, Template>
                {
                    ["STAGE_VAR"] = "stage-value",
                },
            },
        };

        var output = PipelineWriterHelper.Write(new DevopsPipeline.DevopsPipelineWithStages
        {
            Stages = [stage],
        });

        output.ShouldContain("variables:");
        output.ShouldContain("STAGE_VAR: stage-value");
    }

    [Test]
    public void WriteStage_StageDefinition_WithJobs_WritesJobsSection()
    {
        Stage stage = new Stage.StageDefinition
        {
            StageId = "BuildStage",
            Jobs =
            [
                new Job.RegularJob
                {
                    JobId = "CompileJob",
                },
            ],
        };

        var output = PipelineWriterHelper.Write(new DevopsPipeline.DevopsPipelineWithStages
        {
            Stages = [stage],
        });

        output.ShouldContain("- stage: BuildStage");
        output.ShouldContain("jobs:");
        output.ShouldContain("- job: CompileJob");
    }

    [Test]
    public void WriteStage_StageDefinition_WithMultipleJobs_WritesAll()
    {
        Stage stage = new Stage.StageDefinition
        {
            StageId = "TestStage",
            Jobs =
            [
                new Job.RegularJob
                {
                    JobId = "UnitTests",
                },
                new Job.RegularJob
                {
                    JobId = "IntegrationTests",
                },
            ],
        };

        var output = PipelineWriterHelper.Write(new DevopsPipeline.DevopsPipelineWithStages
        {
            Stages = [stage],
        });

        output.ShouldContain("- job: UnitTests");
        output.ShouldContain("- job: IntegrationTests");
    }

    // ── Stage: Template ───────────────────────────────────────────────────────

    [Test]
    public void WriteStage_Template_NoParams_WritesInlineLine()
    {
        Stage stage = new Stage.Template
        {
            TemplatePath = "templates/build-stage.yml",
        };

        var output = PipelineWriterHelper.Write(new DevopsPipeline.DevopsPipelineWithStages
        {
            Stages = [stage],
        });

        output.ShouldContain("- template: templates/build-stage.yml");
    }

    [Test]
    public void WriteStage_Template_WithParams_WritesSection()
    {
        Stage stage = new Stage.Template
        {
            TemplatePath = "templates/build-stage.yml",
            Parameters = new Dictionary<string, Template>
            {
                ["stageEnv"] = "production",
            },
        };

        var output = PipelineWriterHelper.Write(new DevopsPipeline.DevopsPipelineWithStages
        {
            Stages = [stage],
        });

        output.ShouldContain("- template: templates/build-stage.yml");
        output.ShouldContain("parameters:");
        output.ShouldContain("stageEnv: production");
    }

    // ── Multiple stages ───────────────────────────────────────────────────────

    [Test]
    public void WriteStages_Multiple_WritesAllInOrder()
    {
        var pipeline = new DevopsPipeline.DevopsPipelineWithStages
        {
            Stages =
            [
                new Stage.StageDefinition
                {
                    StageId = "BuildStage",
                },
                new Stage.StageDefinition
                {
                    StageId = "TestStage",
                },
                new Stage.StageDefinition
                {
                    StageId = "DeployStage",
                },
            ],
        };

        var output = PipelineWriterHelper.Write(pipeline);

        var buildIndex = output.IndexOf("- stage: BuildStage", StringComparison.Ordinal);
        var testIndex = output.IndexOf("- stage: TestStage", StringComparison.Ordinal);
        var deployIndex = output.IndexOf("- stage: DeployStage", StringComparison.Ordinal);

        buildIndex.ShouldBeLessThan(testIndex);
        testIndex.ShouldBeLessThan(deployIndex);
    }

    // ── Full nested stages → jobs → steps pipeline ────────────────────────────

    [Test]
    public void Write_FullNestedPipeline_WithStagesJobsAndSteps_WritesCorrectly()
    {
        var pipeline = new DevopsPipeline.DevopsPipelineWithStages
        {
            Stages =
            [
                new Stage.StageDefinition
                {
                    StageId = "CI",
                    Jobs =
                    [
                        new Job.RegularJob
                        {
                            JobId = "Build",
                            Pool = new Pool.PoolSpec
                            {
                                VmImage = "ubuntu-latest",
                            },
                            Steps =
                            [
                                new Step.Checkout
                                {
                                    Repository = "self",
                                },
                                new Step.Script
                                {
                                    ScriptContent = "dotnet build",
                                },
                            ],
                        },
                    ],
                },
            ],
        };

        var output = PipelineWriterHelper.Write(pipeline);
        output.ShouldContain("stages:");
        output.ShouldContain("- stage: CI");
        output.ShouldContain("jobs:");
        output.ShouldContain("- job: Build");
        output.ShouldContain("vmImage: ubuntu-latest");
        output.ShouldContain("steps:");
        output.ShouldContain("- checkout: self");
        output.ShouldContain("- script: dotnet build");
    }

    // ── Complete real-world-like pipeline ─────────────────────────────────────

    [Test]
    public void Write_CompleteRealWorldPipeline_ProducesExpectedYaml()
    {
        var pipeline = new DevopsPipeline.DevopsPipelineWithStages
        {
            Trigger = new Trigger.BranchList
            {
                Branches = ["main"],
            },
            Variables = new Variables.Dictionary
            {
                Values = new Dictionary<string, Template>
                {
                    ["buildConfiguration"] = "Release",
                },
            },
            Stages =
            [
                new Stage.StageDefinition
                {
                    StageId = "Build",
                    Jobs =
                    [
                        new Job.RegularJob
                        {
                            JobId = "BuildAndTest",
                            Pool = new Pool.PoolSpec
                            {
                                VmImage = "ubuntu-latest",
                            },
                            Steps =
                            [
                                new Step.Checkout
                                {
                                    Repository = "self",
                                },
                                new Step.Task
                                {
                                    TaskName = "DotNetCoreCLI@2",
                                    DisplayName = "Restore",
                                    Inputs = new Dictionary<string, Template>
                                    {
                                        ["command"] = "restore",
                                    },
                                },
                                new Step.Task
                                {
                                    TaskName = "DotNetCoreCLI@2",
                                    DisplayName = "Build",
                                    Inputs = new Dictionary<string, Template>
                                    {
                                        ["command"] = "build",
                                        ["arguments"] = "--configuration $(buildConfiguration)",
                                    },
                                },
                            ],
                        },
                    ],
                },
            ],
        };

        var output = PipelineWriterHelper.Write(pipeline);

        output.ShouldContain("trigger:");
        output.ShouldContain("branches:");
        output.ShouldContain("include: [ main ]");
        output.ShouldContain("variables:");
        output.ShouldContain("buildConfiguration: Release");
        output.ShouldContain("stages:");
        output.ShouldContain("- stage: Build");
        output.ShouldContain("jobs:");
        output.ShouldContain("- job: BuildAndTest");
        output.ShouldContain("vmImage: ubuntu-latest");
        output.ShouldContain("steps:");
        output.ShouldContain("- checkout: self");
        output.ShouldContain("- task: DotNetCoreCLI@2");
        output.ShouldContain("displayName: Restore");
        output.ShouldContain("command: restore");
        output.ShouldContain("command: build");
    }
}
