namespace Invex.StructuredText.GithubActions.DependabotConfigModel.Model;

/// <summary>
///     Package ecosystem values for Dependabot.
/// </summary>
/// <remarks>
///     <see cref="DependabotUpdate.PackageEcosystem" /> takes the YAML string directly; each member's documentation
///     gives the <c>package-ecosystem</c> value to use.
/// </remarks>
[PublicAPI]
[SuppressMessage("ReSharper", "InconsistentNaming")]
public enum PackageEcosystem
{
    /// <summary>
    ///     Updates Bazel modules and workspace dependencies (<c>bazel</c>).
    /// </summary>
    Bazel,

    /// <summary>
    ///     Updates Bun JavaScript packages (<c>bun</c>).
    /// </summary>
    Bun,

    /// <summary>
    ///     Updates Ruby gems managed by Bundler (<c>bundler</c>).
    /// </summary>
    Bundler,

    /// <summary>
    ///     Updates Rust crates managed by Cargo (<c>cargo</c>).
    /// </summary>
    Cargo,

    /// <summary>
    ///     Updates PHP packages managed by Composer (<c>composer</c>).
    /// </summary>
    Composer,

    /// <summary>
    ///     Updates Conda environment packages (<c>conda</c>).
    /// </summary>
    Conda,

    /// <summary>
    ///     Updates Dev Container features (<c>devcontainers</c>).
    /// </summary>
    Devcontainers,

    /// <summary>
    ///     Updates Docker image references in Dockerfiles and Kubernetes manifests (<c>docker</c>).
    /// </summary>
    Docker,

    /// <summary>
    ///     Updates Docker image references in Docker Compose files (<c>docker-compose</c>).
    /// </summary>
    DockerCompose,

    /// <summary>
    ///     Updates the .NET SDK version pinned in <c>global.json</c> (<c>dotnet-sdk</c>).
    /// </summary>
    DotnetSdk,

    /// <summary>
    ///     Updates Elm packages (<c>elm</c>).
    /// </summary>
    Elm,

    /// <summary>
    ///     Updates actions referenced from GitHub Actions workflows (<c>github-actions</c>).
    /// </summary>
    GithubActions,

    /// <summary>
    ///     Updates Git submodules (<c>gitsubmodule</c>).
    /// </summary>
    Gitsubmodule,

    /// <summary>
    ///     Updates Go modules (<c>gomod</c>).
    /// </summary>
    Gomod,

    /// <summary>
    ///     Updates Java and Kotlin dependencies managed by Gradle (<c>gradle</c>).
    /// </summary>
    Gradle,

    /// <summary>
    ///     Updates Helm chart dependencies (<c>helm</c>).
    /// </summary>
    Helm,

    /// <summary>
    ///     Updates Julia packages (<c>julia</c>).
    /// </summary>
    Julia,

    /// <summary>
    ///     Updates Java dependencies managed by Maven (<c>maven</c>).
    /// </summary>
    Maven,

    /// <summary>
    ///     Updates Elixir packages managed by Mix (Hex) (<c>mix</c>).
    /// </summary>
    Mix,

    /// <summary>
    ///     Updates Nix flake inputs (<c>nix</c>).
    /// </summary>
    Nix,

    /// <summary>
    ///     Updates JavaScript packages managed by npm, Yarn or pnpm (<c>npm</c>).
    /// </summary>
    Npm,

    /// <summary>
    ///     Updates .NET packages from NuGet (<c>nuget</c>).
    /// </summary>
    Nuget,

    /// <summary>
    ///     Updates OpenTofu providers and modules (<c>opentofu</c>).
    /// </summary>
    Opentofu,

    /// <summary>
    ///     Updates Python packages managed by pip, pip-compile, Pipenv or Poetry (<c>pip</c>).
    /// </summary>
    Pip,

    /// <summary>
    ///     Updates pre-commit hook repositories (<c>pre-commit</c>).
    /// </summary>
    PreCommit,

    /// <summary>
    ///     Updates Dart and Flutter packages managed by pub (<c>pub</c>).
    /// </summary>
    Pub,

    /// <summary>
    ///     Updates the Rust toolchain version pinned in <c>rust-toolchain</c> files (<c>rust-toolchain</c>).
    /// </summary>
    RustToolchain,

    /// <summary>
    ///     Updates Swift packages managed by Swift Package Manager (<c>swift</c>).
    /// </summary>
    Swift,

    /// <summary>
    ///     Updates Terraform providers and modules (<c>terraform</c>).
    /// </summary>
    Terraform,

    /// <summary>
    ///     Updates Python packages managed by uv (<c>uv</c>).
    /// </summary>
    Uv,

    /// <summary>
    ///     Updates C and C++ libraries managed by vcpkg (<c>vcpkg</c>).
    /// </summary>
    Vcpkg,
}
