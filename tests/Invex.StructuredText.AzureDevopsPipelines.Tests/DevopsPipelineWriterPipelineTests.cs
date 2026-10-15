namespace Invex.StructuredText.AzureDevopsPipelines.Tests;

[TestFixture]
internal sealed class DevopsPipelineWriterPipelineTests
{
    [Test]
    public void Write_PipelineWithSteps_Minimal_WritesStepsSection()
    {
        var pipeline = new DevopsPipeline.DevopsPipelineWithSteps
        {
            Steps =
            [
                new Step.Script
                {
                    ScriptContent = "echo hello",
                },
            ],
        };

        string[] lines = ["steps:", "", "  - script: echo hello"];

        PipelineWriterHelper
            .Write(pipeline)
            .ShouldBe(PipelineWriterHelper.JoinLines(lines));
    }

    [Test]
    public void Write_PipelineWithSteps_WithName_WritesNameFirst()
    {
        var pipeline = new DevopsPipeline.DevopsPipelineWithSteps
        {
            Name = "$(Build.DefinitionName)",
            Steps =
            [
                new Step.Script
                {
                    ScriptContent = "echo hi",
                },
            ],
        };

        string[] lines = ["name: $(Build.DefinitionName)", "", "steps:", "", "  - script: echo hi"];

        PipelineWriterHelper
            .Write(pipeline)
            .ShouldBe(PipelineWriterHelper.JoinLines(lines));
    }

    [Test]
    public void Write_PipelineWithSteps_WithAppendCommitMessage_WritesProperty()
    {
        var pipeline = new DevopsPipeline.DevopsPipelineWithSteps
        {
            AppendCommitMessageToRunName = false,
            Steps =
            [
                new Step.Script
                {
                    ScriptContent = "echo hi",
                },
            ],
        };

        var output = PipelineWriterHelper.Write(pipeline);
        output.ShouldContain("appendCommitMessageToRunName: false");
    }

    [Test]
    public void Write_PipelineWithSteps_WithPool_WritesPoolBeforeSteps()
    {
        var pipeline = new DevopsPipeline.DevopsPipelineWithSteps
        {
            Pool = new Pool.PoolSpec
            {
                VmImage = "ubuntu-latest",
            },
            Steps =
            [
                new Step.Script
                {
                    ScriptContent = "echo hi",
                },
            ],
        };

        var output = PipelineWriterHelper.Write(pipeline);
        var poolIndex = output.IndexOf("pool:", StringComparison.Ordinal);
        var stepsIndex = output.IndexOf("steps:", StringComparison.Ordinal);
        poolIndex.ShouldBeLessThan(stepsIndex);
    }

    [Test]
    public void Write_PipelineWithSteps_WithServices_WritesServicesSection()
    {
        var pipeline = new DevopsPipeline.DevopsPipelineWithSteps
        {
            Services = new Dictionary<string, Template>
            {
                ["redis"] = "redis:6",
            },
            Steps =
            [
                new Step.Script
                {
                    ScriptContent = "echo hi",
                },
            ],
        };

        var output = PipelineWriterHelper.Write(pipeline);
        output.ShouldContain("services:");
        output.ShouldContain("redis: redis:6");
    }

    [Test]
    public void Write_PipelineWithSteps_WithWorkspace_WritesWorkspaceSection()
    {
        var pipeline = new DevopsPipeline.DevopsPipelineWithSteps
        {
            Workspace = new()
            {
                Clean = true,
            },
            Steps =
            [
                new Step.Script
                {
                    ScriptContent = "echo hi",
                },
            ],
        };

        var output = PipelineWriterHelper.Write(pipeline);
        output.ShouldContain("workspace:");
        output.ShouldContain("clean: true");
    }

    [Test]
    public void Write_PipelineWithSteps_WithContinueOnError_WritesContinueOnErrorProperty()
    {
        var pipeline = new DevopsPipeline.DevopsPipelineWithSteps
        {
            ContinueOnError = true,
            Steps =
            [
                new Step.Script
                {
                    ScriptContent = "echo hi",
                },
            ],
        };

        var output = PipelineWriterHelper.Write(pipeline);
        output.ShouldContain("continueOnError: true");
    }

    [Test]
    public void Write_PipelineWithJobs_Minimal_WritesJobsSection()
    {
        var pipeline = new DevopsPipeline.DevopsPipelineWithJobs
        {
            Jobs =
            [
                new Job.RegularJob
                {
                    JobId = "MyJob",
                },
            ],
        };

        string[] lines = ["", "jobs:", "", "  - job: MyJob"];

        PipelineWriterHelper
            .Write(pipeline)
            .ShouldBe(PipelineWriterHelper.JoinLines(lines));
    }

    [Test]
    public void Write_PipelineWithJobs_MultipleJobs_WritesEmptyLineBetweenJobs()
    {
        var pipeline = new DevopsPipeline.DevopsPipelineWithJobs
        {
            Jobs =
            [
                new Job.RegularJob
                {
                    JobId = "Job1",
                },
                new Job.RegularJob
                {
                    JobId = "Job2",
                },
            ],
        };

        var output = PipelineWriterHelper.Write(pipeline);

        var job1Index = output.IndexOf("- job: Job1", StringComparison.Ordinal);
        var job2Index = output.IndexOf("- job: Job2", StringComparison.Ordinal);

        job1Index.ShouldBeLessThan(job2Index);
        output.ShouldContain("- job: Job1");
        output.ShouldContain("- job: Job2");
    }

    [Test]
    public void Write_PipelineWithJobs_WithPool_WritesPool()
    {
        var pipeline = new DevopsPipeline.DevopsPipelineWithJobs
        {
            Pool = new Pool.PoolName
            {
                Name = "my-pool",
            },
            Jobs =
            [
                new Job.RegularJob
                {
                    JobId = "MyJob",
                },
            ],
        };

        var output = PipelineWriterHelper.Write(pipeline);
        output.ShouldContain("pool: my-pool");
    }

    [Test]
    public void Write_PipelineWithStages_Minimal_WritesStagesSection()
    {
        var pipeline = new DevopsPipeline.DevopsPipelineWithStages
        {
            Stages =
            [
                new Stage.StageDefinition
                {
                    StageId = "BuildStage",
                },
            ],
        };

        string[] lines = ["stages:", "  - stage: BuildStage"];

        PipelineWriterHelper
            .Write(pipeline)
            .ShouldBe(PipelineWriterHelper.JoinLines(lines));
    }

    [Test]
    public void Write_PipelineWithStages_WithLockBehavior_WritesLockBehavior()
    {
        var pipeline = new DevopsPipeline.DevopsPipelineWithStages
        {
            LockBehavior = "runLatest",
            Stages =
            [
                new Stage.StageDefinition
                {
                    StageId = "BuildStage",
                },
            ],
        };

        var output = PipelineWriterHelper.Write(pipeline);
        output.ShouldContain("lockBehavior: runLatest");
    }

    [Test]
    public void Write_PipelineWithExtends_Minimal_WritesExtendsSection()
    {
        var pipeline = new DevopsPipeline.DevopsPipelineWithExtends
        {
            Extends = new()
            {
                Template = "templates/base.yml",
            },
        };

        string[] lines = ["extends:", "  template: templates/base.yml"];

        PipelineWriterHelper
            .Write(pipeline)
            .ShouldBe(PipelineWriterHelper.JoinLines(lines));
    }

    [Test]
    public void Write_PipelineWithExtends_WithParameters_WritesParametersUnderExtends()
    {
        var pipeline = new DevopsPipeline.DevopsPipelineWithExtends
        {
            Extends = new()
            {
                Template = "templates/base.yml",
                Parameters = new Dictionary<string, Template>
                {
                    ["environment"] = "production",
                    ["runTests"] = true,
                },
            },
        };

        var output = PipelineWriterHelper.Write(pipeline);
        output.ShouldContain("extends:");
        output.ShouldContain("template: templates/base.yml");
        output.ShouldContain("parameters:");
        output.ShouldContain("environment: production");
        output.ShouldContain("runTests: true");
    }

    [Test]
    public void Write_PipelineWithExtends_WithExtends_ParametersNested()
    {
        var pipeline = new DevopsPipeline.DevopsPipelineWithExtends
        {
            Extends = new()
            {
                Template = "templates/secure.yml",
                Parameters = new Dictionary<string, Template>
                {
                    ["param1"] = "val1",
                },
            },
        };

        string[] lines = ["extends:", "  template: templates/secure.yml", "  parameters:", "    param1: val1"];

        PipelineWriterHelper
            .Write(pipeline)
            .ShouldBe(PipelineWriterHelper.JoinLines(lines));
    }

    [Test]
    public void DevopsPipelineWriter_DefaultTextWriter_HasDefaultIndentSize()
    {
        var writer = new DevopsPipelineWriter();
        writer.TextWriter.IndentSize.ShouldBe(2);
    }

    [Test]
    public void DevopsPipelineWriter_AfterWrite_TextWriterContainsOutput()
    {
        var writer = new DevopsPipelineWriter();

        writer.Write(new DevopsPipeline.DevopsPipelineWithSteps
        {
            Steps =
            [
                new Step.Script
                {
                    ScriptContent = "echo hi",
                },
            ],
        });

        writer
            .TextWriter
            .ToString()
            .ShouldNotBeEmpty();
    }
}
