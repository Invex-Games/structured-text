namespace Invex.StructuredText.Samples;

/// <summary>
///     Every sample, in reading order.
/// </summary>
public static class AllSamples
{
    public static IReadOnlyList<Sample> Items { get; } =
    [
        new("conditions", "Conditions", "Write if: conditions with C# operators.", ConditionsSample.Run),
        new("values", "Values", "Build YAML values with interpolated strings.", ValuesSample.Run),
        new("references", "References", "Refer to secrets, variables, inputs, matrix values and other jobs.",
            ReferencesSample.Run),
        new("escape-hatches", "Functions and escape hatches",
            "Reach anything the catalogue doesn't cover.", EscapeHatchesSample.Run),
        new("ci-workflow", "A complete workflow", "Build, test on three platforms, publish stable versions, report failures.",
            CiWorkflowSample.Run),
    ];
}
