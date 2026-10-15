namespace Invex.StructuredText.AzureDevopsPipelines.Tests;

[TestFixture]
internal sealed class DevopsPipelineWriterJobTests
{
    [Test]
    public void WriteJob_RegularJob_Minimal_WritesJobId()
    {
        Job job = new Job.RegularJob
        {
            JobId = "MyJob",
        };

        var output = PipelineWriterHelper.Write(new DevopsPipeline.DevopsPipelineWithJobs
        {
            Jobs = [job],
        });

        output.ShouldContain("- job: MyJob");
    }

    [Test]
    public void WriteJob_RegularJob_WithDisplayName_WritesDisplayName()
    {
        Job job = new Job.RegularJob
        {
            JobId = "MyJob",
            DisplayName = "My Build Job",
        };

        var output = PipelineWriterHelper.Write(new DevopsPipeline.DevopsPipelineWithJobs
        {
            Jobs = [job],
        });

        output.ShouldContain("displayName: My Build Job");
    }

    [Test]
    public void WriteJob_RegularJob_WithDependsOn_WritesDependsOnList()
    {
        Job job = new Job.RegularJob
        {
            JobId = "DeployJob",
            DependsOn = ["BuildJob", "TestJob"],
        };

        var output = PipelineWriterHelper.Write(new DevopsPipeline.DevopsPipelineWithJobs
        {
            Jobs = [job],
        });

        output.ShouldContain("dependsOn: [ BuildJob, TestJob ]");
    }

    [Test]
    public void WriteJob_RegularJob_WithCondition_WritesCondition()
    {
        Job job = new Job.RegularJob
        {
            JobId = "MyJob",
            Condition = Tex.Raw("succeeded()"),
        };

        var output = PipelineWriterHelper.Write(new DevopsPipeline.DevopsPipelineWithJobs
        {
            Jobs = [job],
        });

        output.ShouldContain("condition: succeeded()");
    }

    [Test]
    public void WriteJob_RegularJob_WithTimeouts_WritesTimeouts()
    {
        Job job = new Job.RegularJob
        {
            JobId = "MyJob",
            TimeoutInMinutes = 60,
            CancelTimeoutInMinutes = 5,
        };

        var output = PipelineWriterHelper.Write(new DevopsPipeline.DevopsPipelineWithJobs
        {
            Jobs = [job],
        });

        output.ShouldContain("timeoutInMinutes: 60");
        output.ShouldContain("cancelTimeoutInMinutes: 5");
    }

    [Test]
    public void WriteJob_RegularJob_WithSteps_WritesStepsSection()
    {
        Job job = new Job.RegularJob
        {
            JobId = "MyJob",
            Steps =
            [
                new Step.Script
                {
                    ScriptContent = "echo hello",
                },
            ],
        };

        var output = PipelineWriterHelper.Write(new DevopsPipeline.DevopsPipelineWithJobs
        {
            Jobs = [job],
        });

        output.ShouldContain("steps:");
        output.ShouldContain("- script: echo hello");
    }

    [Test]
    public void WriteJob_RegularJob_WithPool_WritesPool()
    {
        Job job = new Job.RegularJob
        {
            JobId = "MyJob",
            Pool = new Pool.PoolSpec
            {
                VmImage = "windows-latest",
            },
        };

        var output = PipelineWriterHelper.Write(new DevopsPipeline.DevopsPipelineWithJobs
        {
            Jobs = [job],
        });

        output.ShouldContain("pool:");
        output.ShouldContain("vmImage: windows-latest");
    }

    [Test]
    public void WriteJob_RegularJob_WithServices_WritesServices()
    {
        Job job = new Job.RegularJob
        {
            JobId = "IntegrationTestJob",
            Services = new Dictionary<string, Template>
            {
                ["postgres"] = "postgres:14",
            },
        };

        var output = PipelineWriterHelper.Write(new DevopsPipeline.DevopsPipelineWithJobs
        {
            Jobs = [job],
        });

        output.ShouldContain("services:");
        output.ShouldContain("postgres: postgres:14");
    }

    [Test]
    public void WriteJob_RegularJob_WithWorkspace_WritesWorkspace()
    {
        Job job = new Job.RegularJob
        {
            JobId = "MyJob",
            Workspace = new()
            {
                Clean = true,
            },
        };

        var output = PipelineWriterHelper.Write(new DevopsPipeline.DevopsPipelineWithJobs
        {
            Jobs = [job],
        });

        output.ShouldContain("workspace:");
        output.ShouldContain("clean: true");
    }

    [Test]
    public void WriteJob_RegularJob_WithTemplateContext_WritesTemplateContext()
    {
        Job job = new Job.RegularJob
        {
            JobId = "MyJob",
            TemplateContext = new Dictionary<string, Template>
            {
                ["key1"] = "val1",
            },
        };

        var output = PipelineWriterHelper.Write(new DevopsPipeline.DevopsPipelineWithJobs
        {
            Jobs = [job],
        });

        output.ShouldContain("templateContext:");
        output.ShouldContain("key1: val1");
    }

    [Test]
    public void WriteJob_RegularJob_WithUses_WritesUsesSection()
    {
        Job job = new Job.RegularJob
        {
            JobId = "MyJob",
            Uses = new()
            {
                Repositories = ["myRepo"],
                Pools = ["myPool"],
            },
        };

        var output = PipelineWriterHelper.Write(new DevopsPipeline.DevopsPipelineWithJobs
        {
            Jobs = [job],
        });

        output.ShouldContain("uses:");
        output.ShouldContain("repositories: [ myRepo ]");
        output.ShouldContain("pools: [ myPool ]");
    }

    [Test]
    public void WriteJob_RegularJob_WithStrategy_WritesStrategy()
    {
        Job job = new Job.RegularJob
        {
            JobId = "MatrixJob",
            Strategy = new()
            {
                Matrix = new Dictionary<string, IReadOnlyDictionary<string, Template>>
                {
                    ["windows"] = new Dictionary<string, Template>
                    {
                        ["os"] = "windows-latest",
                    },
                    ["linux"] = new Dictionary<string, Template>
                    {
                        ["os"] = "ubuntu-latest",
                    },
                },
                MaxParallel = 2,
            },
        };

        var output = PipelineWriterHelper.Write(new DevopsPipeline.DevopsPipelineWithJobs
        {
            Jobs = [job],
        });

        output.ShouldContain("strategy:");
        output.ShouldContain("matrix:");
        output.ShouldContain("windows:");
        output.ShouldContain("os: 'windows-latest'");
        output.ShouldContain("linux:");
        output.ShouldContain("os: 'ubuntu-latest'");
        output.ShouldContain("maxParallel: 2");
    }

    [Test]
    public void WriteJob_RegularJob_WithParallelStrategy_WritesParallel()
    {
        Job job = new Job.RegularJob
        {
            JobId = "ParallelJob",
            Strategy = new()
            {
                Parallel = 5,
            },
        };

        var output = PipelineWriterHelper.Write(new DevopsPipeline.DevopsPipelineWithJobs
        {
            Jobs = [job],
        });

        output.ShouldContain("strategy:");
        output.ShouldContain("parallel: 5");
    }

    [Test]
    public void WriteJob_Deployment_Minimal_WritesDeploymentLine()
    {
        Job job = new Job.Deployment
        {
            DeploymentId = "MyDeploy",
            Environment = new DeploymentEnvironment.EnvironmentName
            {
                Name = "production",
            },
            Strategy = new DeploymentStrategy.RunOnce(),
        };

        var output = PipelineWriterHelper.Write(new DevopsPipeline.DevopsPipelineWithJobs
        {
            Jobs = [job],
        });

        output.ShouldContain("- deployment: MyDeploy");
        output.ShouldContain("environment: production");
    }

    [Test]
    public void WriteJob_Deployment_WithEnvironmentSpec_WritesEnvironmentSection()
    {
        Job job = new Job.Deployment
        {
            DeploymentId = "MyDeploy",
            Environment = new DeploymentEnvironment.EnvironmentSpec
            {
                Name = "production",
                ResourceName = "myVM",
                ResourceType = "VirtualMachine",
                ResourceId = 12345,
                Tags = ["primary"],
            },
            Strategy = new DeploymentStrategy.RunOnce(),
        };

        var output = PipelineWriterHelper.Write(new DevopsPipeline.DevopsPipelineWithJobs
        {
            Jobs = [job],
        });

        output.ShouldContain("environment:");
        output.ShouldContain("name: production");
        output.ShouldContain("resourceName: myVM");
        output.ShouldContain("resourceType: VirtualMachine");
        output.ShouldContain("resourceId: 12345");
        output.ShouldContain("tags: [ primary ]");
    }

    [Test]
    public void WriteJob_Deployment_RunOnce_WithDeploy_WritesRunOnceStrategy()
    {
        Job job = new Job.Deployment
        {
            DeploymentId = "DeployProd",
            Environment = new DeploymentEnvironment.EnvironmentName
            {
                Name = "prod",
            },
            Strategy = new DeploymentStrategy.RunOnce
            {
                Deploy = new()
                {
                    Steps =
                    [
                        new Step.Script
                        {
                            ScriptContent = "echo deploying",
                        },
                    ],
                },
            },
        };

        var output = PipelineWriterHelper.Write(new DevopsPipeline.DevopsPipelineWithJobs
        {
            Jobs = [job],
        });

        output.ShouldContain("strategy:");
        output.ShouldContain("runOnce:");
        output.ShouldContain("deploy:");
        output.ShouldContain("steps:");
        output.ShouldContain("- script: echo deploying");
    }

    [Test]
    public void WriteJob_Deployment_RunOnce_WithOnSuccess_WritesOnSection()
    {
        Job job = new Job.Deployment
        {
            DeploymentId = "DeployProd",
            Environment = new DeploymentEnvironment.EnvironmentName
            {
                Name = "prod",
            },
            Strategy = new DeploymentStrategy.RunOnce
            {
                OnSuccess = new()
                {
                    Steps =
                    [
                        new Step.Script
                        {
                            ScriptContent = "echo success",
                        },
                    ],
                },
            },
        };

        var output = PipelineWriterHelper.Write(new DevopsPipeline.DevopsPipelineWithJobs
        {
            Jobs = [job],
        });

        output.ShouldContain("on:");
        output.ShouldContain("success:");
        output.ShouldContain("- script: echo success");
    }

    [Test]
    public void WriteJob_Deployment_RunOnce_WithOnFailure_WritesOnFailureSection()
    {
        Job job = new Job.Deployment
        {
            DeploymentId = "DeployProd",
            Environment = new DeploymentEnvironment.EnvironmentName
            {
                Name = "prod",
            },
            Strategy = new DeploymentStrategy.RunOnce
            {
                OnFailure = new()
                {
                    Steps =
                    [
                        new Step.Script
                        {
                            ScriptContent = "echo failed",
                        },
                    ],
                },
            },
        };

        var output = PipelineWriterHelper.Write(new DevopsPipeline.DevopsPipelineWithJobs
        {
            Jobs = [job],
        });

        output.ShouldContain("on:");
        output.ShouldContain("failure:");
        output.ShouldContain("- script: echo failed");
    }

    [Test]
    public void WriteJob_Deployment_Rolling_WithMaxParallel_WritesRollingStrategy()
    {
        Job job = new Job.Deployment
        {
            DeploymentId = "DeployRolling",
            Environment = new DeploymentEnvironment.EnvironmentName
            {
                Name = "prod",
            },
            Strategy = new DeploymentStrategy.Rolling
            {
                MaxParallel = 25,
                Deploy = new()
                {
                    Steps =
                    [
                        new Step.Script
                        {
                            ScriptContent = "echo rolling",
                        },
                    ],
                },
            },
        };

        var output = PipelineWriterHelper.Write(new DevopsPipeline.DevopsPipelineWithJobs
        {
            Jobs = [job],
        });

        output.ShouldContain("rolling:");
        output.ShouldContain("maxParallel: 25");
        output.ShouldContain("deploy:");
        output.ShouldContain("- script: echo rolling");
    }

    [Test]
    public void WriteJob_Deployment_Canary_WithIncrements_WritesCanaryStrategy()
    {
        Job job = new Job.Deployment
        {
            DeploymentId = "DeployCanary",
            Environment = new DeploymentEnvironment.EnvironmentName
            {
                Name = "prod",
            },
            Strategy = new DeploymentStrategy.Canary
            {
                Increments = new Template[] { 10, 50 },
                Deploy = new()
                {
                    Steps =
                    [
                        new Step.Script
                        {
                            ScriptContent = "echo canary",
                        },
                    ],
                },
            },
        };

        var output = PipelineWriterHelper.Write(new DevopsPipeline.DevopsPipelineWithJobs
        {
            Jobs = [job],
        });

        output.ShouldContain("canary:");
        output.ShouldContain("increments: [ 10, 50 ]");
        output.ShouldContain("- script: echo canary");
    }

    [Test]
    public void WriteJob_Deployment_WithHook_HasPool_WritesPoolInHook()
    {
        Job job = new Job.Deployment
        {
            DeploymentId = "MyDeploy",
            Environment = new DeploymentEnvironment.EnvironmentName
            {
                Name = "prod",
            },
            Strategy = new DeploymentStrategy.RunOnce
            {
                Deploy = new()
                {
                    Pool = new Pool.PoolSpec
                    {
                        VmImage = "ubuntu-latest",
                    },
                    Steps =
                    [
                        new Step.Script
                        {
                            ScriptContent = "echo deploy",
                        },
                    ],
                },
            },
        };

        var output = PipelineWriterHelper.Write(new DevopsPipeline.DevopsPipelineWithJobs
        {
            Jobs = [job],
        });

        output.ShouldContain("vmImage: ubuntu-latest");
    }

    [Test]
    public void WriteJob_Deployment_WithAllBasicHooks_WritesAll()
    {
        var simpleHook = new DeploymentHook
        {
            Steps =
            [
                new Step.Script
                {
                    ScriptContent = "echo step",
                },
            ],
        };

        Job job = new Job.Deployment
        {
            DeploymentId = "FullDeploy",
            Environment = new DeploymentEnvironment.EnvironmentName
            {
                Name = "prod",
            },
            Strategy = new DeploymentStrategy.RunOnce
            {
                PreDeploy = simpleHook,
                Deploy = simpleHook,
                RouteTraffic = simpleHook,
                PostRouteTraffic = simpleHook,
            },
        };

        var output = PipelineWriterHelper.Write(new DevopsPipeline.DevopsPipelineWithJobs
        {
            Jobs = [job],
        });

        output.ShouldContain("preDeploy:");
        output.ShouldContain("deploy:");
        output.ShouldContain("routeTraffic:");
        output.ShouldContain("postRouteTraffic:");
    }

    [Test]
    public void WriteJob_Template_NoParams_WritesInlineLine()
    {
        Job job = new Job.Template
        {
            TemplatePath = "templates/build-job.yml",
        };

        var output = PipelineWriterHelper.Write(new DevopsPipeline.DevopsPipelineWithJobs
        {
            Jobs = [job],
        });

        output.ShouldContain("- template: templates/build-job.yml");
    }

    [Test]
    public void WriteJob_Template_WithParams_WritesSection()
    {
        Job job = new Job.Template
        {
            TemplatePath = "templates/build-job.yml",
            Parameters = new Dictionary<string, Template>
            {
                ["buildType"] = "release",
            },
        };

        var output = PipelineWriterHelper.Write(new DevopsPipeline.DevopsPipelineWithJobs
        {
            Jobs = [job],
        });

        output.ShouldContain("- template: templates/build-job.yml");
        output.ShouldContain("parameters:");
        output.ShouldContain("buildType: release");
    }

    [Test]
    public void WriteJob_Deployment_WithServices_WritesServices()
    {
        Job job = new Job.Deployment
        {
            DeploymentId = "MyDeploy",
            Environment = new DeploymentEnvironment.EnvironmentName
            {
                Name = "prod",
            },
            Strategy = new DeploymentStrategy.RunOnce(),
            Services = new Dictionary<string, Template>
            {
                ["db"] = "postgres:14",
            },
        };

        var output = PipelineWriterHelper.Write(new DevopsPipeline.DevopsPipelineWithJobs
        {
            Jobs = [job],
        });

        output.ShouldContain("services:");
        output.ShouldContain("db: postgres:14");
    }

    [Test]
    public void WriteJob_Deployment_WithTemplateContext_WritesTemplateContext()
    {
        Job job = new Job.Deployment
        {
            DeploymentId = "MyDeploy",
            Environment = new DeploymentEnvironment.EnvironmentName
            {
                Name = "prod",
            },
            Strategy = new DeploymentStrategy.RunOnce(),
            TemplateContext = new Dictionary<string, Template>
            {
                ["region"] = "eastus",
            },
        };

        var output = PipelineWriterHelper.Write(new DevopsPipeline.DevopsPipelineWithJobs
        {
            Jobs = [job],
        });

        output.ShouldContain("templateContext:");
        output.ShouldContain("region: eastus");
    }
}
