namespace Invex.StructuredText.GithubActions.Tests;

/// <summary>
///     Tests for how scalar properties are written, in particular multiline values that must be
///     emitted as YAML block scalars. These exercise the <c>WriteProperty(string, string)</c> path
///     via a job's <c>env</c> values.
/// </summary>
[TestFixture]
internal sealed class GithubActionWriterPropertyTests
{
    private static string WriteEnvValue(string value)
    {
        var job = GithubActionWriterHelper.MinimalJob() with
        {
            Env = new Dictionary<string, TextExpression>
            {
                ["MY_VAR"] = new RawExpression(value),
            },
        };

        return GithubActionWriterHelper.Write(GithubActionWriterHelper.MinimalAction() with
        {
            Jobs = [job],
        });
    }

    [Test]
    public void WriteProperty_ValueWithLineFeedNewlines_WritesBlockScalar()
    {
        var output = WriteEnvValue("line1\nline2");

        output.ShouldContain("MY_VAR: |");
        output.ShouldContain("line1");
        output.ShouldContain("line2");
    }

    [Test]
    public void WriteProperty_ValueWithCarriageReturnNewlines_WritesBlockScalar()
    {
        var output = WriteEnvValue("line1\rline2");

        output.ShouldContain("MY_VAR: |");
        output.ShouldContain("line1");
        output.ShouldContain("line2");
    }

    [Test]
    public void WriteProperty_ValueWithCarriageReturnLineFeedNewlines_WritesBlockScalar()
    {
        var output = WriteEnvValue("line1\r\nline2");

        output.ShouldContain("MY_VAR: |");
        output.ShouldContain("line1");
        output.ShouldContain("line2");
    }

    [Test]
    public void WriteProperty_ValueWithLineFeedNewlines_DoesNotWriteValueInline()
    {
        var output = WriteEnvValue("line1\nline2");

        // The buggy behaviour would emit "MY_VAR: line1\nline2" inline rather than a block scalar.
        output.ShouldNotContain("MY_VAR: line1");
    }

    [Test]
    public void WriteProperty_SingleLineValue_WritesInline()
    {
        var output = WriteEnvValue("hello");

        output.ShouldContain("MY_VAR: hello");
        output.ShouldNotContain("MY_VAR: |");
    }
}

