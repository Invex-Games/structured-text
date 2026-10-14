namespace Invex.StructuredText.GithubActions.Tests;

[TestFixture]
public class GithubExpressionPrinterTests
{
    private static string Print(Expr expression) =>
        GithubExpressionPrinter.Instance.Print(expression);

    private static string Render(Template template) =>
        GithubExpressionPrinter.Instance.Render(template);

    [Test]
    public void StringLiteral_EscapesSingleQuotes() =>
        Print(GitHub.Ref.Eq("it's")).ShouldBe("github.ref == 'it''s'");

    [Test]
    public void AndOr_UseCSharpOperators() =>
        Print(GitHub.EventName.Eq("pull_request") && !GitHub.Actor.Eq("dependabot[bot]"))
            .ShouldBe("github.event_name == 'pull_request' && !(github.actor == 'dependabot[bot]')");

    [Test]
    public void OrInsideAnd_IsParenthesised() =>
        Print((GitHub.EventName.Eq("push") || GitHub.Ref.StartsWith("refs/tags/")) && Status.Success())
            .ShouldBe("(github.event_name == 'push' || startsWith(github.ref, 'refs/tags/')) && success()");

    [Test]
    public void AndInsideOr_IsNotParenthesised() =>
        Print(GitHub.EventName.Eq("push") && GitHub.RefProtected || Status.Always())
            .ShouldBe("github.event_name == 'push' && github.ref_protected || always()");

    [Test]
    public void ChainedAnd_IsFlattened() =>
        Print(Fn.All(GitHub.RefProtected, GitHub.Event.PullRequest.Draft, Status.Success()))
            .ShouldBe("github.ref_protected && github.event.pull_request.draft && success()");

    [Test]
    public void DoubleNegation_Cancels() =>
        Print(!!GitHub.RefProtected).ShouldBe("github.ref_protected");

    [Test]
    public void NumberComparison() =>
        Print(GitHub.Event.PullRequest.Number.Gt(3)).ShouldBe("github.event.pull_request.number > 3");

    [Test]
    public void JsonIndexer_ExtendsContextPath() =>
        Print(GitHub.Event["number"]).ShouldBe("github.event.number");

    [Test]
    public void JsonIndexer_OnDynamicContext() =>
        Print(GitHub.Secrets["NUGET_API_KEY"]).ShouldBe("secrets.NUGET_API_KEY");

    [Test]
    public void ContextFunctions() =>
        Print(GitHub.Context.ToJson()).ShouldBe("toJSON(github)");

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
        Render($"{GitHub.Workflow}-{GitHub.Ref}").ShouldBe("${{ github.workflow }}-${{ github.ref }}");

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
    public void Raw_IsVerbatim() =>
        Print(BoolExpr.Raw("github.event_name == 'push'") && Status.Success())
            .ShouldBe("github.event_name == 'push' && success()");

    [Test]
    public void DescendantsAndSelf_FindsReferences() =>
        (GitHub.EventName.Eq("push") && Ref.JobOutput("a", "b").Eq("c"))
        .DescendantsAndSelf()
        .OfType<JobOutputRef>()
        .Single()
        .Job
        .ShouldBe("a");

    [Test]
    public void ToString_UsesNeutralSyntax() =>
        (GitHub.Ref.Eq("main") && Ref.Secret("X").Eq("y")).ToString()
        .ShouldBe("github.ref == 'main' && secret(X) == 'y'");
}
