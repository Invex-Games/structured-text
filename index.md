# Invex.StructuredText

A strongly-typed C# library for generating structured YAML text files, specifically GitHub Actions workflows, Dependabot
configurations, and Azure DevOps Pipelines, using a fluent, type-safe API with a powerful expression system.

## Start here

- [Introduction](docs/introduction.md): why generate YAML from C#, and the key concepts
- [Getting Started](docs/getting-started.md): install and generate your first file
- [Expressions](docs/expressions.md): the unified expression system
- [GitHub Actions Workflows](docs/github-actions.md): workflow generation in depth
- [API Reference](api/index.md): generated reference for every public type

## Packages

| Package | What it does |
| --- | --- |
| [Invex.StructuredText](https://www.nuget.org/packages/Invex.StructuredText) | Core library with `StructuredTextWriter` and the expression system |
| [Invex.StructuredText.GithubActions](https://www.nuget.org/packages/Invex.StructuredText.GithubActions) | GitHub Actions workflow and Dependabot config generation |
| [Invex.StructuredText.AzureDevopsPipelines](https://www.nuget.org/packages/Invex.StructuredText.AzureDevopsPipelines) | Azure DevOps Pipelines YAML generation |
