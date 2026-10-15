using static Invex.StructuredText.Expressions.Tex;

namespace Invex.StructuredText.GithubActions.Tests;

[TestFixture]
public class TexTests
{
    private static string Print(Expr expression) =>
        GithubExpressionPrinter.Instance.Print(expression);

    [Test]
    public void UsingStatic_DropsThePrefix() =>
        Print(Success() && JobResult("test").Eq(Result(RunResult.Success)) && Secret("TOKEN").Ne(""))
            .ShouldBe("success() && needs.test.result == 'success' && secrets.TOKEN != ''");

    [Test]
    public void GitHubContexts_HangOffTex() =>
        Print(Tex.GitHub.Event.PullRequest.Draft || Tex.GitHub.Ref.StartsWith("refs/tags/"))
            .ShouldBe("github.event.pull_request.draft || startsWith(github.ref, 'refs/tags/')");

    [Test]
    public void Raw_ConvertsToEveryExpressionType()
    {
        StringExpr text = Raw("github.head_ref");
        NumberExpr number = Raw("github.run_attempt");

        Print(text.Eq("main")).ShouldBe("github.head_ref == 'main'");
        Print(number.Gt(1)).ShouldBe("github.run_attempt > 1");
        Print(!Raw("a || b")).ShouldBe("!(a || b)");
    }
}
