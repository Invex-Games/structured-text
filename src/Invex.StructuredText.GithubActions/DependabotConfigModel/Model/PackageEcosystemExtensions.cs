namespace Invex.StructuredText.GithubActions.DependabotConfigModel.Model;

/// <summary>
///     Extension methods for <see cref="PackageEcosystem" />.
/// </summary>
[PublicAPI]
public static class PackageEcosystemExtensions
{
    /// <summary>
    ///     Gets the <c>package-ecosystem</c> YAML value for <paramref name="ecosystem" />, for use with
    ///     <see cref="DependabotUpdate.PackageEcosystem" />.
    /// </summary>
    /// <param name="ecosystem">The package ecosystem.</param>
    /// <returns>The value Dependabot expects, such as <c>nuget</c> or <c>github-actions</c>.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     <paramref name="ecosystem" /> is not a defined <see cref="PackageEcosystem" /> value.
    /// </exception>
    public static string ToYamlValue(this PackageEcosystem ecosystem) =>
        ecosystem switch
        {
            PackageEcosystem.Bazel => "bazel",
            PackageEcosystem.Bun => "bun",
            PackageEcosystem.Bundler => "bundler",
            PackageEcosystem.Cargo => "cargo",
            PackageEcosystem.Composer => "composer",
            PackageEcosystem.Conda => "conda",
            PackageEcosystem.Devcontainers => "devcontainers",
            PackageEcosystem.Docker => "docker",
            PackageEcosystem.DockerCompose => "docker-compose",
            PackageEcosystem.DotnetSdk => "dotnet-sdk",
            PackageEcosystem.Elm => "elm",
            PackageEcosystem.GithubActions => "github-actions",
            PackageEcosystem.Gitsubmodule => "gitsubmodule",
            PackageEcosystem.Gomod => "gomod",
            PackageEcosystem.Gradle => "gradle",
            PackageEcosystem.Helm => "helm",
            PackageEcosystem.Julia => "julia",
            PackageEcosystem.Maven => "maven",
            PackageEcosystem.Mix => "mix",
            PackageEcosystem.Nix => "nix",
            PackageEcosystem.Npm => "npm",
            PackageEcosystem.Nuget => "nuget",
            PackageEcosystem.Opentofu => "opentofu",
            PackageEcosystem.Pip => "pip",
            PackageEcosystem.PreCommit => "pre-commit",
            PackageEcosystem.Pub => "pub",
            PackageEcosystem.RustToolchain => "rust-toolchain",
            PackageEcosystem.Swift => "swift",
            PackageEcosystem.Terraform => "terraform",
            PackageEcosystem.Uv => "uv",
            PackageEcosystem.Vcpkg => "vcpkg",
            _ => throw new ArgumentOutOfRangeException(nameof(ecosystem), ecosystem, null),
        };
}
