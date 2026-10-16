using Invex.StructuredText.Samples;

namespace Invex.StructuredText.GithubActions.Tests;

/// <summary>
///     Keeps the samples compiling and records what each one produces.
///     The verified files next to this test are the samples' expected output.
/// </summary>
[TestFixture]
public sealed class SamplesTests
{
    private static IEnumerable<TestCaseData> Samples() =>
        AllSamples.Items.Select(x => new TestCaseData(x.Name).SetName($"Sample_{x.Name}"));

    [TestCaseSource(nameof(Samples))]
    public Task Sample_ProducesExpectedOutput(string name) =>
        Verify(AllSamples.Items.Single(x => x.Name == name).Run())
            .UseParameters(name);
}
