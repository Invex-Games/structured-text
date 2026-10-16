namespace Invex.StructuredText.GithubActions.Tests;

[TestFixture]
public class GithubExpressionPrinterTests
{
    private static string Print(Expr expression) =>
        GithubExpressionPrinter.Instance.Print(expression);

    private static string Render(Template template) =>
        GithubExpressionPrinter.Instance.Render(template);

    [Test]
    public void PrintCondition_LeadingNot_WrapsInDelimiters() =>
        GithubExpressionPrinter.Instance.PrintCondition(!Tex.GitHub.Ref.Contains("-"))
            .ShouldBe("${{ !contains(github.ref, '-') }}");

    [Test]
    public void PrintCondition_LeadingWhitespaceThenNot_WrapsInDelimiters() =>
        GithubExpressionPrinter.Instance.PrintCondition(Tex.Raw(" !contains(github.ref, '-')"))
            .ShouldBe("${{  !contains(github.ref, '-') }}");

    [Test]
    public void Catalogue_RootMembersDoNotCollideWithContexts()
    {
        Print(Tex.GitHub.Env["NAME"]).ShouldBe("env.NAME");
        Print(Tex.GitHub.GitHubEnv).ShouldBe("github.env");
        Print(Tex.GitHub.Job.ToJson()).ShouldBe("toJSON(job)");
        Print(Tex.GitHub.GitHubJob).ShouldBe("github.job");
    }

    [Test]
    public void Indexer_KeyThatIsNotAnIdentifier_UsesBracketSyntax()
    {
        Print(Tex.GitHub.Matrix["1st dim"]).ShouldBe("matrix['1st dim']");
        Print(Tex.GitHub.Matrix["a.b"]).ShouldBe("matrix['a.b']");
        Print(Tex.GitHub.Matrix["it's"]).ShouldBe("matrix['it''s']");
        Print(Tex.GitHub.Matrix["build-slice"]).ShouldBe("matrix.build-slice");
    }

    [Test]
    public void Indexer_OnParsedJson_UsesBracketSyntaxWhereNeeded()
    {
        Print(Tex.FromJson("{\"a.b\":1}")["a.b"]).ShouldBe("fromJSON('{\"a.b\":1}')['a.b']");
        Print(Tex.FromJson(Tex.Variable("CONFIG"))["region"]).ShouldBe("fromJSON(vars.CONFIG).region");
    }

    [Test]
    public void Catalogue_MemberNamedPath_KeepsItsName() =>
        Print(Tex.GitHub.Event.WorkflowRun.Path).ShouldBe("github.event.workflow_run.path");

    [Test]
    public void Template_BoolHole_IsLiteralText()
    {
        Template on = $"flag={true}";
        Template off = $"flag={false}";

        Render(on).ShouldBe("flag=true");
        Render(off).ShouldBe("flag=false");
        on.Expressions.ShouldBeEmpty();
        off.Expressions.ShouldBeEmpty();
    }

    [Test]
    public void Raw_ComparisonUnderNot_IsParenthesized() =>
        Print(!Tex.Raw("github.ref == 'main'")).ShouldBe("!(github.ref == 'main')");

    [Test]
    public void Raw_OrUnderAnd_IsParenthesized() =>
        Print(Tex.Raw("a == 'x' || b == 'y'") && Tex.Success())
            .ShouldBe("(a == 'x' || b == 'y') && success()");

    [Test]
    public void Raw_PlainPath_StaysBare() =>
        Print(!Tex.Raw("github.event.forced") && Tex.Success())
            .ShouldBe("!github.event.forced && success()");

    [Test]
    public void Raw_Standalone_IsWrittenVerbatim() =>
        Print(Tex.Raw("a || b")).ShouldBe("a || b");

    [Test]
    public void PrintCondition_LiteralFirst_WrapsInDelimiters() =>
        GithubExpressionPrinter.Instance.PrintCondition(((StringExpr)"main").Eq(Tex.GitHub.Ref))
            .ShouldBe("${{ 'main' == github.ref }}");

    [Test]
    public void PrimitiveArguments_ConvertWhereAnyExpressionIsAccepted()
    {
        Print(Tex.Format("{0}-{1}", "hello", 2)).ShouldBe("format('{0}-{1}', 'hello', 2)");
        Print(Tex.FromJson("[1]").Contains(1)).ShouldBe("contains(fromJSON('[1]'), 1)");
        Print(Tex.FromJson("[true]").Contains(true)).ShouldBe("contains(fromJSON('[true]'), true)");
    }

    [Test]
    public void PrintCondition_NoLeadingNot_StaysBare() =>
        GithubExpressionPrinter.Instance.PrintCondition(Tex.GitHub.Ref.Contains("-"))
            .ShouldBe("contains(github.ref, '-')");

    [Test]
    public void StringLiteral_EscapesSingleQuotes() =>
        Print(Tex.GitHub.Ref.Eq("it's")).ShouldBe("github.ref == 'it''s'");

    [Test]
    public void AndOr_UseCSharpOperators() =>
        Print(Tex.GitHub.EventName.Eq("pull_request") && !Tex.GitHub.Actor.Eq("dependabot[bot]"))
            .ShouldBe("github.event_name == 'pull_request' && !(github.actor == 'dependabot[bot]')");

    [Test]
    public void OrInsideAnd_IsParenthesised() =>
        Print((Tex.GitHub.EventName.Eq("push") || Tex.GitHub.Ref.StartsWith("refs/tags/")) && Tex.Success())
            .ShouldBe("(github.event_name == 'push' || startsWith(github.ref, 'refs/tags/')) && success()");

    [Test]
    public void AndInsideOr_IsNotParenthesised() =>
        Print(Tex.GitHub.EventName.Eq("push") && Tex.GitHub.RefProtected || Tex.Always())
            .ShouldBe("github.event_name == 'push' && github.ref_protected || always()");

    [Test]
    public void ChainedAnd_IsFlattened() =>
        Print(Tex.All(Tex.GitHub.RefProtected, Tex.GitHub.Event.PullRequest.Draft, Tex.Success()))
            .ShouldBe("github.ref_protected && github.event.pull_request.draft && success()");

    [Test]
    public void DoubleNegation_Cancels() =>
        Print(!!Tex.GitHub.RefProtected).ShouldBe("github.ref_protected");

    [Test]
    public void NumberComparison() =>
        Print(Tex.GitHub.Event.PullRequest.Number.Gt(3)).ShouldBe("github.event.pull_request.number > 3");

    [Test]
    public void JsonIndexer_ExtendsContextPath() =>
        Print(Tex.GitHub.Event["number"]).ShouldBe("github.event.number");

    [Test]
    public void JsonIndexer_OnDynamicContext() =>
        Print(Tex.GitHub.Secrets["NUGET_API_KEY"]).ShouldBe("secrets.NUGET_API_KEY");

    [Test]
    public void ContextFunctions() =>
        Print(Tex.GitHub.Context.ToJson()).ShouldBe("toJSON(github)");

    [Test]
    public void Reference_JobOutput() =>
        Print(!Tex.JobOutput("SetupBuildInfo", "build-version").Contains("-"))
            .ShouldBe("!contains(needs.SetupBuildInfo.outputs.build-version, '-')");

    [Test]
    public void Reference_JobResult() =>
        Print(Tex.JobResult("build").Eq(Tex.Result(RunResult.Success))).ShouldBe("needs.build.result == 'success'");

    [TestCase("secret", "secrets.TOKEN")]
    [TestCase("variable", "vars.TOKEN")]
    [TestCase("input", "inputs.TOKEN")]
    [TestCase("matrix", "matrix.TOKEN")]
    public void Reference_Simple(string kind, string expected) =>
        Print(kind switch
            {
                "secret" => Tex.Secret("TOKEN"),
                "variable" => Tex.Variable("TOKEN"),
                "input" => Tex.Input("TOKEN"),
                _ => Tex.Matrix("TOKEN"),
            })
            .ShouldBe(expected);

    [Test]
    public void Reference_NameNeedingIndexSyntax() =>
        Print(Tex.Matrix("1st dim")).ShouldBe("matrix['1st dim']");

    [Test]
    public void Template_Interpolation_WrapsEachExpression() =>
        Render($"{Tex.GitHub.Workflow}-{Tex.GitHub.Ref}").ShouldBe("${{ github.workflow }}-${{ github.ref }}");

    [Test]
    public void Template_FromString_IsLiteral() =>
        Render("github.ref").ShouldBe("github.ref");

    [Test]
    public void Template_FromExpression_IsWrapped() =>
        Render(Tex.Secret("TOKEN")).ShouldBe("${{ secrets.TOKEN }}");

    [Test]
    public void Template_NumberAndBool_AreLiteralText()
    {
        Render(5).ShouldBe("5");
        Render(true).ShouldBe("true");
    }

    [Test]
    public void Template_Join() =>
        Render(Tex.Join("-", [Tex.Matrix("os"), "x", Tex.Matrix("arch")]))
            .ShouldBe("${{ matrix.os }}-x-${{ matrix.arch }}");

    [Test]
    public void Raw_ComparisonUnderAnd_IsParenthesized() =>
        Print(Tex.Raw("github.event_name == 'push'") && Tex.Success())
            .ShouldBe("(github.event_name == 'push') && success()");

    [Test]
    public void DescendantsAndSelf_FindsReferences() =>
        (Tex.GitHub.EventName.Eq("push") && Tex.JobOutput("a", "b").Eq("c"))
        .DescendantsAndSelf()
        .OfType<JobOutputRef>()
        .Single()
        .Job
        .ShouldBe("a");

    [Test]
    public void ToString_UsesNeutralSyntax() =>
        (Tex.GitHub.Ref.Eq("main") && Tex.Secret("X").Eq("y")).ToString()
        .ShouldBe("github.ref == 'main' && secret(X) == 'y'");

    [Test]
    public void CollectionMembership_UsesContains() =>
        Print(Tex.GitHub.Ref.In(Tex.FromJson(Tex.Variable("BRANCHES"))))
            .ShouldBe("contains(fromJSON(vars.BRANCHES), github.ref)");
}
