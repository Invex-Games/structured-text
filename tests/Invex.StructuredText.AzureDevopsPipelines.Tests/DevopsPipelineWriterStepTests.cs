namespace Invex.StructuredText.AzureDevopsPipelines.Tests;

[TestFixture]
internal sealed class DevopsPipelineWriterStepTests
{
    [Test]
    public void WriteStep_Task_Minimal_WritesTaskLine()
    {
        Step step = new Step.Task
        {
            TaskName = "DotNetCoreCLI@2",
        };

        var output = PipelineWriterHelper.Write(new DevopsPipeline.DevopsPipelineWithSteps
        {
            Steps = [step],
        });

        output.ShouldContain("- task: DotNetCoreCLI@2");
    }

    [Test]
    public void WriteStep_Task_WithInputs_WritesInputsSection()
    {
        Step step = new Step.Task
        {
            TaskName = "DotNetCoreCLI@2",
            Inputs = new Dictionary<string, Template>
            {
                ["command"] = "build",
                ["projects"] = "**/*.csproj",
            },
        };

        var output = PipelineWriterHelper.Write(new DevopsPipeline.DevopsPipelineWithSteps
        {
            Steps = [step],
        });

        output.ShouldContain("inputs:");
        output.ShouldContain("command: build");
        output.ShouldContain("projects: **/*.csproj");
    }

    [Test]
    public void WriteStep_Task_WithCommonProperties_WritesAll()
    {
        Step step = new Step.Task
        {
            TaskName = "MyTask@1",
            DisplayName = "Run My Task",
            Name = "myStep",
            Condition = Tex.Raw("succeeded()"),
            ContinueOnError = true,
            Enabled = false,
            TimeoutInMinutes = 30,
            RetryCountOnTaskFailure = 2,
            Env = new Dictionary<string, Template>
            {
                ["MY_TOKEN"] = "$(mySecret)",
            },
        };

        var output = PipelineWriterHelper.Write(new DevopsPipeline.DevopsPipelineWithSteps
        {
            Steps = [step],
        });

        output.ShouldContain("displayName: Run My Task");
        output.ShouldContain("name: myStep");
        output.ShouldContain("condition: succeeded()");
        output.ShouldContain("continueOnError: true");
        output.ShouldContain("enabled: false");
        output.ShouldContain("timeoutInMinutes: 30");
        output.ShouldContain("retryCountOnTaskFailure: 2");
        output.ShouldContain("env:");
        output.ShouldContain("MY_TOKEN: $(mySecret)");
    }

    [Test]
    public void WriteStep_Script_WritesScriptLine()
    {
        Step step = new Step.Script
        {
            ScriptContent = "echo hello",
        };

        var output = PipelineWriterHelper.Write(new DevopsPipeline.DevopsPipelineWithSteps
        {
            Steps = [step],
        });

        output.ShouldContain("- script: echo hello");
    }

    [Test]
    public void WriteStep_Script_WithFailOnStderr_WritesProperty()
    {
        Step step = new Step.Script
        {
            ScriptContent = "echo hello",
            FailOnStderr = true,
        };

        var output = PipelineWriterHelper.Write(new DevopsPipeline.DevopsPipelineWithSteps
        {
            Steps = [step],
        });

        output.ShouldContain("failOnStderr: true");
    }

    [Test]
    public void WriteStep_Script_WithWorkingDirectory_WritesProperty()
    {
        Step step = new Step.Script
        {
            ScriptContent = "npm install",
            WorkingDirectory = "$(Build.SourcesDirectory)/frontend",
        };

        var output = PipelineWriterHelper.Write(new DevopsPipeline.DevopsPipelineWithSteps
        {
            Steps = [step],
        });

        output.ShouldContain("workingDirectory: $(Build.SourcesDirectory)/frontend");
    }

    [Test]
    public void WriteStep_PowerShell_WritesPowerShellLine()
    {
        Step step = new Step.PowerShell
        {
            ScriptContent = "Write-Host 'hello'",
        };

        var output = PipelineWriterHelper.Write(new DevopsPipeline.DevopsPipelineWithSteps
        {
            Steps = [step],
        });

        output.ShouldContain("- powershell: Write-Host 'hello'");
    }

    [Test]
    public void WriteStep_PowerShell_WithErrorActionPreference_WritesProperty()
    {
        Step step = new Step.PowerShell
        {
            ScriptContent = "do-something",
            ErrorActionPreference = "continue",
        };

        var output = PipelineWriterHelper.Write(new DevopsPipeline.DevopsPipelineWithSteps
        {
            Steps = [step],
        });

        output.ShouldContain("errorActionPreference: continue");
    }

    [Test]
    public void WriteStep_PowerShell_WithIgnoreLastExitCode_WritesProperty()
    {
        Step step = new Step.PowerShell
        {
            ScriptContent = "do-something",
            IgnoreLastExitCode = true,
        };

        var output = PipelineWriterHelper.Write(new DevopsPipeline.DevopsPipelineWithSteps
        {
            Steps = [step],
        });

        output.ShouldContain("ignoreLASTEXITCODE: true");
    }

    // ── Pwsh step ─────────────────────────────────────────────────────────────

    [Test]
    public void WriteStep_Pwsh_WritesPwshLine()
    {
        Step step = new Step.Pwsh
        {
            ScriptContent = "Write-Output 'hello'",
        };

        var output = PipelineWriterHelper.Write(new DevopsPipeline.DevopsPipelineWithSteps
        {
            Steps = [step],
        });

        output.ShouldContain("- pwsh: Write-Output 'hello'");
    }

    [Test]
    public void WriteStep_Pwsh_WithAllPsProperties_WritesAll()
    {
        Step step = new Step.Pwsh
        {
            ScriptContent = "do-something",
            FailOnStderr = true,
            WorkingDirectory = "/src",
            ErrorActionPreference = "stop",
            IgnoreLastExitCode = false,
        };

        var output = PipelineWriterHelper.Write(new DevopsPipeline.DevopsPipelineWithSteps
        {
            Steps = [step],
        });

        output.ShouldContain("failOnStderr: true");
        output.ShouldContain("workingDirectory: /src");
        output.ShouldContain("errorActionPreference: stop");
        output.ShouldContain("ignoreLASTEXITCODE: false");
    }

    // ── Bash step ─────────────────────────────────────────────────────────────

    [Test]
    public void WriteStep_Bash_WritesBashLine()
    {
        Step step = new Step.Bash
        {
            ScriptContent = "echo hello",
        };

        var output = PipelineWriterHelper.Write(new DevopsPipeline.DevopsPipelineWithSteps
        {
            Steps = [step],
        });

        output.ShouldContain("- bash: echo hello");
    }

    [Test]
    public void WriteStep_Bash_WithFailOnStderrAndWorkingDirectory_WritesBoth()
    {
        Step step = new Step.Bash
        {
            ScriptContent = "make build",
            FailOnStderr = false,
            WorkingDirectory = "$(Build.SourcesDirectory)",
        };

        var output = PipelineWriterHelper.Write(new DevopsPipeline.DevopsPipelineWithSteps
        {
            Steps = [step],
        });

        output.ShouldContain("failOnStderr: false");
        output.ShouldContain("workingDirectory: $(Build.SourcesDirectory)");
    }

    // ── Checkout step ─────────────────────────────────────────────────────────

    [Test]
    public void WriteStep_Checkout_WithSelf_WritesCheckoutSelf()
    {
        Step step = new Step.Checkout
        {
            Repository = "self",
        };

        var output = PipelineWriterHelper.Write(new DevopsPipeline.DevopsPipelineWithSteps
        {
            Steps = [step],
        });

        output.ShouldContain("- checkout: self");
    }

    [Test]
    public void WriteStep_Checkout_WithNone_WritesCheckoutNone()
    {
        Step step = new Step.Checkout
        {
            Repository = "none",
        };

        var output = PipelineWriterHelper.Write(new DevopsPipeline.DevopsPipelineWithSteps
        {
            Steps = [step],
        });

        output.ShouldContain("- checkout: none");
    }

    [Test]
    public void WriteStep_Checkout_WithAllOptions_WritesAllProperties()
    {
        Step step = new Step.Checkout
        {
            Repository = "self",
            Clean = true,
            FetchDepth = 0,
            Lfs = true,
            Submodules = false,
            Path = "$(Build.SourcesDirectory)/src",
            PersistCredentials = true,
        };

        var output = PipelineWriterHelper.Write(new DevopsPipeline.DevopsPipelineWithSteps
        {
            Steps = [step],
        });

        output.ShouldContain("clean: true");
        output.ShouldContain("fetchDepth: 0");
        output.ShouldContain("lfs: true");
        output.ShouldContain("submodules: false");
        output.ShouldContain("path: $(Build.SourcesDirectory)/src");
        output.ShouldContain("persistCredentials: true");
    }

    // ── Download step ─────────────────────────────────────────────────────────

    [Test]
    public void WriteStep_Download_WritesDownloadLine()
    {
        Step step = new Step.Download
        {
            Pipeline = "current",
        };

        var output = PipelineWriterHelper.Write(new DevopsPipeline.DevopsPipelineWithSteps
        {
            Steps = [step],
        });

        output.ShouldContain("- download: current");
    }

    [Test]
    public void WriteStep_Download_WithAllOptions_WritesAllProperties()
    {
        Step step = new Step.Download
        {
            Pipeline = "myUpstream",
            Artifact = "drop",
            Patterns = ["**/*.zip"],
            Path = "$(Pipeline.Workspace)/drop",
        };

        var output = PipelineWriterHelper.Write(new DevopsPipeline.DevopsPipelineWithSteps
        {
            Steps = [step],
        });

        output.ShouldContain("artifact: drop");
        output.ShouldContain("patterns: [ **/*.zip ]");
        output.ShouldContain("path: $(Pipeline.Workspace)/drop");
    }

    // ── DownloadBuild step ────────────────────────────────────────────────────

    [Test]
    public void WriteStep_DownloadBuild_WritesDownloadBuildLine()
    {
        Step step = new Step.DownloadBuild
        {
            Build = "myBuild",
        };

        var output = PipelineWriterHelper.Write(new DevopsPipeline.DevopsPipelineWithSteps
        {
            Steps = [step],
        });

        output.ShouldContain("- downloadBuild: myBuild");
    }

    [Test]
    public void WriteStep_DownloadBuild_WithAllOptions_WritesAllProperties()
    {
        Step step = new Step.DownloadBuild
        {
            Build = "myBuild",
            Artifact = "logs",
            Patterns = ["**/*.log"],
            Path = "$(Build.StagingDirectory)",
        };

        var output = PipelineWriterHelper.Write(new DevopsPipeline.DevopsPipelineWithSteps
        {
            Steps = [step],
        });

        output.ShouldContain("artifact: logs");
        output.ShouldContain("patterns: [ **/*.log ]");
        output.ShouldContain("path: $(Build.StagingDirectory)");
    }

    // ── GetPackage step ───────────────────────────────────────────────────────

    [Test]
    public void WriteStep_GetPackage_WritesGetPackageLine()
    {
        Step step = new Step.GetPackage
        {
            Package = "myPackage",
        };

        var output = PipelineWriterHelper.Write(new DevopsPipeline.DevopsPipelineWithSteps
        {
            Steps = [step],
        });

        output.ShouldContain("- getPackage: myPackage");
    }

    [Test]
    public void WriteStep_GetPackage_WithVersionAndPath_WritesBoth()
    {
        Step step = new Step.GetPackage
        {
            Package = "myNuGet",
            Version = "1.2.3",
            Path = "$(Build.ArtifactStagingDirectory)",
        };

        var output = PipelineWriterHelper.Write(new DevopsPipeline.DevopsPipelineWithSteps
        {
            Steps = [step],
        });

        output.ShouldContain("version: 1.2.3");
        output.ShouldContain("path: $(Build.ArtifactStagingDirectory)");
    }

    // ── Publish step ──────────────────────────────────────────────────────────

    [Test]
    public void WriteStep_Publish_WritesPublishLine()
    {
        Step step = new Step.Publish
        {
            PublishPath = "$(Build.ArtifactStagingDirectory)",
        };

        var output = PipelineWriterHelper.Write(new DevopsPipeline.DevopsPipelineWithSteps
        {
            Steps = [step],
        });

        output.ShouldContain("- publish: $(Build.ArtifactStagingDirectory)");
    }

    [Test]
    public void WriteStep_Publish_WithArtifact_WritesArtifactProperty()
    {
        Step step = new Step.Publish
        {
            PublishPath = "$(Build.ArtifactStagingDirectory)",
            Artifact = "drop",
        };

        var output = PipelineWriterHelper.Write(new DevopsPipeline.DevopsPipelineWithSteps
        {
            Steps = [step],
        });

        output.ShouldContain("artifact: drop");
    }

    // ── Template step ─────────────────────────────────────────────────────────

    [Test]
    public void WriteStep_Template_NoParams_WritesInlineLine()
    {
        Step step = new Step.Template
        {
            TemplatePath = "templates/common-steps.yml",
        };

        var output = PipelineWriterHelper.Write(new DevopsPipeline.DevopsPipelineWithSteps
        {
            Steps = [step],
        });

        output.ShouldContain("- template: templates/common-steps.yml");
    }

    [Test]
    public void WriteStep_Template_WithParams_WritesSection()
    {
        Step step = new Step.Template
        {
            TemplatePath = "templates/deploy.yml",
            Parameters = new Dictionary<string, Template>
            {
                ["environment"] = "prod",
            },
        };

        var output = PipelineWriterHelper.Write(new DevopsPipeline.DevopsPipelineWithSteps
        {
            Steps = [step],
        });

        output.ShouldContain("- template: templates/deploy.yml");
        output.ShouldContain("parameters:");
        output.ShouldContain("environment: prod");
    }

    // ── ReviewApp step ────────────────────────────────────────────────────────

    [Test]
    public void WriteStep_ReviewApp_WritesReviewAppLine()
    {
        Step step = new Step.ReviewApp
        {
            ReviewAppType = "kubernetes",
        };

        var output = PipelineWriterHelper.Write(new DevopsPipeline.DevopsPipelineWithSteps
        {
            Steps = [step],
        });

        output.ShouldContain("- reviewApp: kubernetes");
    }

    // ── StepTarget ────────────────────────────────────────────────────────────

    [Test]
    public void WriteStepTarget_TargetName_WritesSimpleTargetProperty()
    {
        Step step = new Step.Task
        {
            TaskName = "MyTask@1",
            Target = new StepTarget.TargetName
            {
                Name = "myContainer",
            },
        };

        var output = PipelineWriterHelper.Write(new DevopsPipeline.DevopsPipelineWithSteps
        {
            Steps = [step],
        });

        output.ShouldContain("target: myContainer");
    }

    [Test]
    public void WriteStepTarget_TargetSpec_WithContainer_WritesContainerInSection()
    {
        Step step = new Step.Task
        {
            TaskName = "MyTask@1",
            Target = new StepTarget.TargetSpec
            {
                Container = "myContainer",
            },
        };

        var output = PipelineWriterHelper.Write(new DevopsPipeline.DevopsPipelineWithSteps
        {
            Steps = [step],
        });

        output.ShouldContain("target:");
        output.ShouldContain("container: myContainer");
    }

    [Test]
    public void WriteStepTarget_TargetSpec_WithCommands_WritesCommandsProperty()
    {
        Step step = new Step.Task
        {
            TaskName = "MyTask@1",
            Target = new StepTarget.TargetSpec
            {
                Commands = "restricted",
            },
        };

        var output = PipelineWriterHelper.Write(new DevopsPipeline.DevopsPipelineWithSteps
        {
            Steps = [step],
        });

        output.ShouldContain("target:");
        output.ShouldContain("commands: restricted");
    }

    [Test]
    public void WriteStepTarget_TargetSpec_WithSettableVariables_AllowedBool_WritesProperty()
    {
        Step step = new Step.Task
        {
            TaskName = "MyTask@1",
            Target = new StepTarget.TargetSpec
            {
                SettableVariables = new()
                {
                    Allowed = false,
                },
            },
        };

        var output = PipelineWriterHelper.Write(new DevopsPipeline.DevopsPipelineWithSteps
        {
            Steps = [step],
        });

        output.ShouldContain("settableVariables: false");
    }

    [Test]
    public void WriteStepTarget_TargetSpec_WithSettableVariables_List_WritesList()
    {
        Step step = new Step.Task
        {
            TaskName = "MyTask@1",
            Target = new StepTarget.TargetSpec
            {
                SettableVariables = new()
                {
                    AllowedVariables = ["MY_VAR", "ANOTHER_VAR"],
                },
            },
        };

        var output = PipelineWriterHelper.Write(new DevopsPipeline.DevopsPipelineWithSteps
        {
            Steps = [step],
        });

        output.ShouldContain("settableVariables: [ MY_VAR, ANOTHER_VAR ]");
    }

    // ── JobContainer inline ───────────────────────────────────────────────────

    [Test]
    public void WriteJobContainer_ContainerName_WritesSimpleContainerProperty()
    {
        var output = PipelineWriterHelper.Write(new DevopsPipeline.DevopsPipelineWithSteps
        {
            Container = new JobContainer.ContainerName
            {
                Name = "myContainer",
            },
            Steps =
            [
                new Step.Script
                {
                    ScriptContent = "echo hi",
                },
            ],
        });

        output.ShouldContain("container: myContainer");
    }

    [Test]
    public void WriteJobContainer_ContainerSpec_WritesSection()
    {
        var output = PipelineWriterHelper.Write(new DevopsPipeline.DevopsPipelineWithSteps
        {
            Container = new JobContainer.ContainerSpec
            {
                Image = "ubuntu:22.04",
                Options = "--user root",
                Endpoint = "myRegistry",
                Env = new Dictionary<string, Template>
                {
                    ["ENV_VAR"] = "val",
                },
                Ports = ["8080:80"],
                Volumes = ["/opt:/opt"],
                MapDockerSocket = true,
            },
            Steps =
            [
                new Step.Script
                {
                    ScriptContent = "echo hi",
                },
            ],
        });

        output.ShouldContain("container:");
        output.ShouldContain("image: ubuntu:22.04");
        output.ShouldContain("options: --user root");
        output.ShouldContain("endpoint: myRegistry");
        output.ShouldContain("ENV_VAR: val");
        output.ShouldContain("ports: [ 8080:80 ]");
        output.ShouldContain("volumes: [ /opt:/opt ]");
        output.ShouldContain("mapDockerSocket: true");
    }

    // ── Multiple steps ────────────────────────────────────────────────────────

    [Test]
    public void WriteSteps_Multiple_WritesAllInOrder()
    {
        var pipeline = new DevopsPipeline.DevopsPipelineWithSteps
        {
            Steps =
            [
                new Step.Checkout
                {
                    Repository = "self",
                },
                new Step.Script
                {
                    ScriptContent = "echo build",
                },
                new Step.Publish
                {
                    PublishPath = "drop",
                },
            ],
        };

        var output = PipelineWriterHelper.Write(pipeline);
        var checkoutIndex = output.IndexOf("- checkout:", StringComparison.Ordinal);
        var scriptIndex = output.IndexOf("- script:", StringComparison.Ordinal);
        var publishIndex = output.IndexOf("- publish:", StringComparison.Ordinal);

        checkoutIndex.ShouldBeLessThan(scriptIndex);
        scriptIndex.ShouldBeLessThan(publishIndex);
    }
}
