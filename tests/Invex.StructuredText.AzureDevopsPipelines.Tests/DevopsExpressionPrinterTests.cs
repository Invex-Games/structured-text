namespace Invex.StructuredText.AzureDevopsPipelines.Tests;

[TestFixture]
internal sealed class DevopsExpressionPrinterTests
{
    private static string Print(Expr expression) =>
        DevopsExpressionPrinter.Instance.Print(expression);

    private static string Render(Template template) =>
        DevopsExpressionPrinter.Instance.Render(template);

    [Test]
    public void Literals()
    {
        Print((StringExpr)"it's").ShouldBe("'it''s'");
        Print((NumberExpr)3.14).ShouldBe("3.14");
        Print((BoolExpr)true).ShouldBe("true");
    }

    [Test]
    public void Comparisons_UseFunctions()
    {
        Print(Tex.Variable("a").Eq("x")).ShouldBe("eq(variables['a'], 'x')");
        Print(Tex.Variable("a").Ne("x")).ShouldBe("ne(variables['a'], 'x')");
        Print(((NumberExpr)Tex.Raw("n")).Lt(1)).ShouldBe("lt(n, 1)");
        Print(((NumberExpr)Tex.Raw("n")).Ge(1)).ShouldBe("ge(n, 1)");
    }

    [Test]
    public void Logic_UsesFunctions() =>
        Print((Tex.Raw("a") || Tex.Raw("b")) && !Tex.Raw("c") && Tex.Success())
            .ShouldBe("and(or(a, b), not(c), succeeded())");

    [Test]
    public void StatusChecks()
    {
        Print(Tex.Success()).ShouldBe("succeeded()");
        Print(Tex.Failure()).ShouldBe("failed()");
        Print(Tex.Cancelled()).ShouldBe("canceled()");
        Print(Tex.Always()).ShouldBe("always()");
    }

    [Test]
    public void Functions()
    {
        Print(Tex.Variable("Build.SourceBranch").StartsWith("refs/tags/"))
            .ShouldBe("startsWith(variables['Build.SourceBranch'], 'refs/tags/')");

        Print(Tex.Format("{0}-{1}", Tex.Variable("a"), 2)).ShouldBe("format('{0}-{1}', variables['a'], 2)");
        Print(((JsonExpr)Tex.Raw("x")).ToJson()).ShouldBe("convertToJson(x)");
    }

    [Test]
    public void FromJson_IsNotSupported() =>
        Should.Throw<NotSupportedException>(() => Print(Tex.FromJson("{}")));

    [Test]
    public void References_InExpressions()
    {
        Print(Tex.Secret("TOKEN")).ShouldBe("variables['TOKEN']");
        Print(Tex.Matrix("os")).ShouldBe("variables['os']");
        Print(Tex.Input("dryRun")).ShouldBe("parameters.dryRun");
        Print(Tex.JobOutput("Setup", "version")).ShouldBe("dependencies.Setup.outputs['Setup.version']");
        Print(Tex.JobOutput("Setup", "version", "Info")).ShouldBe("dependencies.Setup.outputs['Info.version']");
        Print(Tex.StepOutput("Info", "version")).ShouldBe("variables['Info.version']");
        Print(Tex.JobResult("Build").Eq(Tex.Result(RunResult.Success))).ShouldBe("eq(dependencies.Build.result, 'Succeeded')");
    }

    [Test]
    public void Results()
    {
        Print(Tex.Result(RunResult.Failure)).ShouldBe("'Failed'");
        Print(Tex.Result(RunResult.Cancelled)).ShouldBe("'Canceled'");
        Print(Tex.Result(RunResult.Skipped)).ShouldBe("'Skipped'");
    }

    [Test]
    public void Template_MacroReferences() =>
        Render($"{Tex.Secret("TOKEN")}/{Tex.Variable("Build.BuildId")}/{Tex.Matrix("os")}/{Tex.StepOutput("Info", "version")}")
            .ShouldBe("$(TOKEN)/$(Build.BuildId)/$(os)/$(Info.version)");

    [Test]
    public void Template_JobOutput_IsRuntime() =>
        Render(Tex.JobOutput("Setup", "version")).ShouldBe("$[ dependencies.Setup.outputs['Setup.version'] ]");

    [Test]
    public void Template_Runtime_IsRuntime() =>
        Render(Tex.DevOps.Runtime(Tex.Variable("a").Eq("b"))).ShouldBe("$[ eq(variables['a'], 'b') ]");

    [Test]
    public void Template_OtherExpressions_AreCompileTime() =>
        Render($"v{Tex.Input("version")}").ShouldBe("v${{ parameters.version }}");

    [Test]
    public void PredefinedVariables()
    {
        Render(Tex.DevOps.Build.SourceBranch).ShouldBe("$(Build.SourceBranch)");
        Print(Tex.DevOps.System.AccessToken).ShouldBe("variables['System.AccessToken']");
        Print(Tex.DevOps.Agent.Os.Eq("Linux")).ShouldBe("eq(variables['Agent.OS'], 'Linux')");
    }

    [Test]
    public void Runtime_NestedInAnExpression_PrintsItsOperand() =>
        Print(Tex.DevOps.Runtime(Tex.Raw("a")).AsBool() && Tex.Success()).ShouldBe("and(a, succeeded())");

    [Test]
    public void Runtime_RawFunctionCall_IsNotParenthesized()
    {
        Print(Tex.DevOps.Runtime(Tex.Raw("eq(variables['a'], 'b')")).AsBool())
            .ShouldBe("eq(variables['a'], 'b')");

        Print(Tex.DevOps.Runtime(Tex.Raw("eq(variables['a'], 'b')")).AsBool() && Tex.Success())
            .ShouldBe("and(eq(variables['a'], 'b'), succeeded())");
    }

    [Test]
    public void Join_PutsTheSeparatorFirst()
    {
        var items = (JsonExpr)Tex.Raw("parameters.items");

        Print(items.Join("-")).ShouldBe("join('-', parameters.items)");
        Print(items.Join()).ShouldBe("join(',', parameters.items)");
    }

    [Test]
    public void CollectionMembership_UsesContainsValue()
    {
        var branches = (JsonExpr)Tex.Raw("parameters.branches");

        Print(Tex.DevOps.Build.SourceBranch.In(branches))
            .ShouldBe("containsValue(parameters.branches, variables['Build.SourceBranch'])");

        Print(branches.Contains("main")).ShouldBe("containsValue(parameters.branches, 'main')");
        Print(Tex.DevOps.Build.SourceBranch.Contains("main")).ShouldBe("contains(variables['Build.SourceBranch'], 'main')");
    }

    [Test]
    public void Template_CompoundExpressionReadingAJobOutput_IsRuntime() =>
        Render(Tex.Format("v{0}", Tex.JobOutput("Setup", "version", "Info")))
            .ShouldBe("$[ format('v{0}', dependencies.Setup.outputs['Info.version']) ]");

    [Test]
    public void Template_JobResult_IsRuntime() =>
        Render(Tex.JobResult("Build").Eq(Tex.Result(RunResult.Success)))
            .ShouldBe("$[ eq(dependencies.Build.result, 'Succeeded') ]");

    [Test]
    public void Template_CastRuntimeExpression_StaysRuntime() =>
        Render(Tex.DevOps.Runtime(Tex.Variable("a").Eq("b")).AsBool())
            .ShouldBe("$[ eq(variables['a'], 'b') ]");

    [Test]
    public void Template_RuntimeExpressionWithSurroundingText_IsNotSupported()
    {
        Should.Throw<NotSupportedException>(() => Render($"v{Tex.JobOutput("Setup", "version", "Info")}"));
        Should.Throw<NotSupportedException>(() => Render($"v{Tex.DevOps.Runtime(Tex.Raw("a"))}"));
    }

    [Test]
    public void Template_MultipleRuntimeHoles_IsNotSupported() =>
        Should.Throw<NotSupportedException>(() =>
            Render($"{Tex.JobOutput("A", "x")}{Tex.JobOutput("B", "y")}"));

    [Test]
    public void Input_InARuntimeExpression_IsNotSupported()
    {
        Should.Throw<NotSupportedException>(() =>
            Render(Tex.Format("{0}-{1}", Tex.Input("prefix"), Tex.JobOutput("Setup", "version", "Info"))));

        Should.Throw<NotSupportedException>(() => Render(Tex.DevOps.Runtime(Tex.Input("prefix").Eq("x"))));
    }

    [Test]
    public void Input_InACondition_IsNotSupported()
    {
        Should.Throw<NotSupportedException>(() => DevopsExpressionPrinter.Instance.PrintCondition(Tex.Input("environment").Eq("prod")));
        Should.Throw<NotSupportedException>(() =>
            DevopsExpressionPrinter.Instance.PrintCondition(Tex.Success() && Tex.Input("environment").Eq("prod")));
    }

    [Test]
    public void Condition_WithVariablesAndStatus_IsPrinted() =>
        DevopsExpressionPrinter.Instance.PrintCondition(Tex.Success() && Tex.Variable("environment").Eq("prod"))
            .ShouldBe("and(succeeded(), eq(variables['environment'], 'prod'))");

    [Test]
    public void RuntimeOnlyValue_InACompoundCompileTimeExpression_IsNotSupported()
    {
        Should.Throw<NotSupportedException>(() => Render(Tex.Format("v{0}", Tex.StepOutput("Info", "version"))));
        Should.Throw<NotSupportedException>(() => Render(Tex.Format("v{0}", Tex.Secret("TOKEN"))));
        Should.Throw<NotSupportedException>(() => Render(Tex.Format("v{0}", Tex.Matrix("os"))));
    }

    [Test]
    public void StepOutput_InterpolatedDirectly_IsAMacro() =>
        Render($"v{Tex.StepOutput("Info", "version")}").ShouldBe("v$(Info.version)");

    [Test]
    public void StepOutput_InAnExplicitRuntimeExpression_IsRuntime() =>
        Render(Tex.DevOps.Runtime(Tex.Format("v{0}", Tex.StepOutput("Info", "version"))))
            .ShouldBe("$[ format('v{0}', variables['Info.version']) ]");

    [Test]
    public void HyphenatedNames_UseBracketSyntax()
    {
        Render(Tex.Input("dry-run")).ShouldBe("${{ parameters['dry-run'] }}");
        Print(Tex.JobResult("build-linux")).ShouldBe("dependencies['build-linux'].result");
        Print(Tex.Input("dryRun")).ShouldBe("parameters.dryRun");
    }
}
