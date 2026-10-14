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
        GithubExpressionPrinter.Instance.PrintCondition(!GitHubContexts.Ref.Contains("-"))
            .ShouldBe("${{ !contains(github.ref, '-') }}");

    [Test]
    public void PrintCondition_LeadingWhitespaceThenNot_WrapsInDelimiters() =>
        GithubExpressionPrinter.Instance.PrintCondition(BoolExpr.Raw(" !contains(github.ref, '-')"))
            .ShouldBe("${{  !contains(github.ref, '-') }}");

    [Test]
    public void Catalogue_RootMembersDoNotCollideWithContexts()
    {
        Print(GitHubContexts.Env["NAME"]).ShouldBe("env.NAME");
        Print(GitHubContexts.GitHubEnv).ShouldBe("github.env");
        Print(GitHubContexts.Job.ToJson()).ShouldBe("toJSON(job)");
        Print(GitHubContexts.GitHubJob).ShouldBe("github.job");
    }

    [Test]
    public void Indexer_KeyThatIsNotAnIdentifier_UsesBracketSyntax()
    {
        Print(GitHubContexts.Matrix["1st dim"]).ShouldBe("matrix['1st dim']");
        Print(GitHubContexts.Matrix["a.b"]).ShouldBe("matrix['a.b']");
        Print(GitHubContexts.Matrix["it's"]).ShouldBe("matrix['it''s']");
        Print(GitHubContexts.Matrix["build-slice"]).ShouldBe("matrix.build-slice");
    }

    [Test]
    public void Indexer_OnParsedJson_UsesBracketSyntaxWhereNeeded()
    {
        Print(Fn.FromJson("{\"a.b\":1}")["a.b"]).ShouldBe("fromJSON('{\"a.b\":1}')['a.b']");
        Print(Fn.FromJson(Ref.Variable("CONFIG"))["region"]).ShouldBe("fromJSON(vars.CONFIG).region");
    }

    [Test]
    public void Raw_ComparisonUnderNot_IsParenthesized() =>
        Print(!BoolExpr.Raw("github.ref == 'main'")).ShouldBe("!(github.ref == 'main')");

    [Test]
    public void Raw_OrUnderAnd_IsParenthesized() =>
        Print(BoolExpr.Raw("a == 'x' || b == 'y'") && Status.Success())
            .ShouldBe("(a == 'x' || b == 'y') && success()");

    [Test]
    public void Raw_PlainPath_StaysBare() =>
        Print(!BoolExpr.Raw("github.event.forced") && Status.Success())
            .ShouldBe("!github.event.forced && success()");

    [Test]
    public void Raw_Standalone_IsWrittenVerbatim() =>
        Print(BoolExpr.Raw("a || b")).ShouldBe("a || b");

    [Test]
    public void PrintCondition_LiteralFirst_WrapsInDelimiters() =>
        GithubExpressionPrinter.Instance.PrintCondition(((StringExpr)"main").Eq(GitHubContexts.Ref))
            .ShouldBe("${{ 'main' == github.ref }}");

    [Test]
    public void PrimitiveArguments_ConvertWhereAnyExpressionIsAccepted()
    {
        Print(Fn.Format("{0}-{1}", "hello", 2)).ShouldBe("format('{0}-{1}', 'hello', 2)");
        Print(Fn.FromJson("[1]").Contains(1)).ShouldBe("contains(fromJSON('[1]'), 1)");
        Print(Fn.FromJson("[true]").Contains(true)).ShouldBe("contains(fromJSON('[true]'), true)");
    }

    [Test]
    public void PrintCondition_NoLeadingNot_StaysBare() =>
        GithubExpressionPrinter.Instance.PrintCondition(GitHubContexts.Ref.Contains("-"))
            .ShouldBe("contains(github.ref, '-')");

    [Test]
    public void StringLiteral_EscapesSingleQuotes() =>
        Print(GitHubContexts.Ref.Eq("it's")).ShouldBe("github.ref == 'it''s'");

    [Test]
    public void AndOr_UseCSharpOperators() =>
        Print(GitHubContexts.EventName.Eq("pull_request") && !GitHubContexts.Actor.Eq("dependabot[bot]"))
            .ShouldBe("github.event_name == 'pull_request' && !(github.actor == 'dependabot[bot]')");

    [Test]
    public void OrInsideAnd_IsParenthesised() =>
        Print((GitHubContexts.EventName.Eq("push") || GitHubContexts.Ref.StartsWith("refs/tags/")) && Status.Success())
            .ShouldBe("(github.event_name == 'push' || startsWith(github.ref, 'refs/tags/')) && success()");

    [Test]
    public void AndInsideOr_IsNotParenthesised() =>
        Print(GitHubContexts.EventName.Eq("push") && GitHubContexts.RefProtected || Status.Always())
            .ShouldBe("github.event_name == 'push' && github.ref_protected || always()");

    [Test]
    public void ChainedAnd_IsFlattened() =>
        Print(Fn.All(GitHubContexts.RefProtected, GitHubContexts.Event.PullRequest.Draft, Status.Success()))
            .ShouldBe("github.ref_protected && github.event.pull_request.draft && success()");

    [Test]
    public void DoubleNegation_Cancels() =>
        Print(!!GitHubContexts.RefProtected).ShouldBe("github.ref_protected");

    [Test]
    public void NumberComparison() =>
        Print(GitHubContexts.Event.PullRequest.Number.Gt(3)).ShouldBe("github.event.pull_request.number > 3");

    [Test]
    public void JsonIndexer_ExtendsContextPath() =>
        Print(GitHubContexts.Event["number"]).ShouldBe("github.event.number");

    [Test]
    public void JsonIndexer_OnDynamicContext() =>
        Print(GitHubContexts.Secrets["NUGET_API_KEY"]).ShouldBe("secrets.NUGET_API_KEY");

    [Test]
    public void ContextFunctions() =>
        Print(GitHubContexts.Context.ToJson()).ShouldBe("toJSON(github)");

    [Test]
    public void Reference_JobOutput() =>
        Print(!Ref.JobOutput("SetupBuildInfo", "build-version").Contains("-"))
            .ShouldBe("!contains(needs.SetupBuildInfo.outputs.build-version, '-')");

    [Test]
    public void Reference_JobResult() =>
        Print(Ref.JobResult("build").Eq(Ref.Result(RunResult.Success))).ShouldBe("needs.build.result == 'success'");

    [TestCase("secret", "secrets.TOKEN")]
    [TestCase("variable", "vars.TOKEN")]
    [TestCase("input", "inputs.TOKEN")]
    [TestCase("matrix", "matrix.TOKEN")]
    public void Reference_Simple(string kind, string expected) =>
        Print(kind switch
            {
                "secret" => Ref.Secret("TOKEN"),
                "variable" => Ref.Variable("TOKEN"),
                "input" => Ref.Input("TOKEN"),
                _ => Ref.Matrix("TOKEN"),
            })
            .ShouldBe(expected);

    [Test]
    public void Reference_NameNeedingIndexSyntax() =>
        Print(Ref.Matrix("1st dim")).ShouldBe("matrix['1st dim']");

    [Test]
    public void Template_Interpolation_WrapsEachExpression() =>
        Render($"{GitHubContexts.Workflow}-{GitHubContexts.Ref}").ShouldBe("${{ github.workflow }}-${{ github.ref }}");

    [Test]
    public void Template_FromString_IsLiteral() =>
        Render("github.ref").ShouldBe("github.ref");

    [Test]
    public void Template_FromExpression_IsWrapped() =>
        Render(Ref.Secret("TOKEN")).ShouldBe("${{ secrets.TOKEN }}");

    [Test]
    public void Template_NumberAndBool_AreLiteralText()
    {
        Render(5).ShouldBe("5");
        Render(true).ShouldBe("true");
    }

    [Test]
    public void Template_Join() =>
        Render(Template.Join("-", [Ref.Matrix("os"), "x", Ref.Matrix("arch")]))
            .ShouldBe("${{ matrix.os }}-x-${{ matrix.arch }}");

    [Test]
    public void Raw_ComparisonUnderAnd_IsParenthesized() =>
        Print(BoolExpr.Raw("github.event_name == 'push'") && Status.Success())
            .ShouldBe("(github.event_name == 'push') && success()");

    [Test]
    public void DescendantsAndSelf_FindsReferences() =>
        (GitHubContexts.EventName.Eq("push") && Ref.JobOutput("a", "b").Eq("c"))
        .DescendantsAndSelf()
        .OfType<JobOutputRef>()
        .Single()
        .Job
        .ShouldBe("a");

    [Test]
    public void ToString_UsesNeutralSyntax() =>
        (GitHubContexts.Ref.Eq("main") && Ref.Secret("X").Eq("y")).ToString()
        .ShouldBe("github.ref == 'main' && secret(X) == 'y'");
}
