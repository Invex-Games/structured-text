namespace Invex.StructuredText.GithubActions.Tests;

[TestFixture]
internal sealed class PackageEcosystemExtensionsTests
{
    [Test]
    public void ToYamlValue_MapsEveryMemberToADistinctValue()
    {
        var values = Enum
            .GetValues<PackageEcosystem>()
            .Select(x => x.ToYamlValue())
            .ToList();

        values.ShouldAllBe(x => !string.IsNullOrEmpty(x));
        values.ShouldBeUnique();
    }

    [TestCase(PackageEcosystem.Nuget, "nuget")]
    [TestCase(PackageEcosystem.GithubActions, "github-actions")]
    [TestCase(PackageEcosystem.DockerCompose, "docker-compose")]
    [TestCase(PackageEcosystem.DotnetSdk, "dotnet-sdk")]
    [TestCase(PackageEcosystem.Gomod, "gomod")]
    [TestCase(PackageEcosystem.PreCommit, "pre-commit")]
    public void ToYamlValue_ReturnsDependabotValue(PackageEcosystem ecosystem, string expected) =>
        ecosystem
            .ToYamlValue()
            .ShouldBe(expected);

    [Test]
    public void ToYamlValue_UndefinedValue_Throws() =>
        Should.Throw<ArgumentOutOfRangeException>(() => ((PackageEcosystem)(-1)).ToYamlValue());

    [Test]
    public void ToYamlValue_WrittenByDependabotConfigWriter()
    {
        var writer = new DependabotConfigWriter();

        writer.WriteConfig(new DependabotConfig
        {
            Updates =
            [
                new DependabotUpdate
                {
                    PackageEcosystem = PackageEcosystem.GithubActions.ToYamlValue(),
                    Directory = "/",
                },
            ],
        });

        writer
            .TextWriter
            .ToString()
            .ShouldContain("  - package-ecosystem: github-actions");
    }
}
