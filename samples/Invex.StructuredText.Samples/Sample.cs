namespace Invex.StructuredText.Samples;

/// <summary>
///     One runnable sample: a title, a short explanation and the text it produces.
/// </summary>
public sealed record Sample(string Name, string Title, string Summary, Func<string> Run);

/// <summary>
///     Helpers shared by the samples.
/// </summary>
public static class Render
{
    /// <summary>Prints a condition the way it appears after <c>if:</c>.</summary>
    public static string Condition(BoolExpr condition) =>
        GithubExpressionPrinter.Instance.PrintCondition(condition);

    /// <summary>Renders a value the way it appears in YAML, with <c>${{ }}</c> around embedded expressions.</summary>
    public static string Value(Template template) =>
        GithubExpressionPrinter.Instance.Render(template);

    /// <summary>Writes a complete workflow file.</summary>
    public static string Workflow(GithubAction workflow)
    {
        var writer = new GithubActionWriter();
        writer.Write(workflow);

        return writer.TextWriter.ToString();
    }

    /// <summary>Formats C# source labels next to the text they produce, one per line.</summary>
    public static string Lines(params (string Label, string Output)[] lines) =>
        string.Join("\n", lines.Select(x => $"{x.Label,-28} => {x.Output}"));
}
