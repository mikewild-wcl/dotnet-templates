# GitHub Copilot Instructions

## Project Context

This is a .NET Aspire project. Target framework: net10.0. Package versions are centrally managed in Directory.Packages.props.

## Code Standards

- Use nullable reference types and implicit usings (both enabled)
- Follow SonarAnalyzer rules (code analysis warnings are treated as errors)
- Use file-scoped namespaces
- Don't specify package versions in .csproj files (use central package management)

## .NET Aspire Patterns

- AppHost.cs orchestrates all services using DistributedApplication builder
- ServiceDefaults project provides shared configuration for all services
- Services should call `builder.AddServiceDefaults()` and `app.MapDefaultEndpoints()`
