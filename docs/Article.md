# Aspire project setup - plus a template

Aspire (formerly .NET Aspire) is a great way to set up your application so that developers can get up and running quickly. It lets you create locally hosted services so other developers on your team can just clone the repo and run without having to spend hours installing dependencies like databases, storage accounts and caches, and it manages the wiring between the services in your application for you. 

When I start a new Aspire project there are a few bits of housekeeping that I like to do to keep my projects consistent.

## Prerequsites

Before starting, make sure your templates are up to date. Aspire is updated frequently so the templates can get out of date. All you need to do to update is run this:
```
dotnet new install Aspire.ProjectTemplates
```

## Creating an empty application

Create a new Aspire Empty App into a new directory; the solution and project folders should go under a `src/` folder. I like to use Visual Studio but you can also create from the command line.

Add `README.md`, default `.gitignore` and a `LICENSE` files - all optional as they can be added when you create a git repo. 

Add a `Directory.Build.props` file to the root folder:
```
<Project>
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <PropertyGroup>
    <AnalysisLevel>Latest</AnalysisLevel>
    <AnalysisMode>All</AnalysisMode>
    <TreatWarningsAsErrors>false</TreatWarningsAsErrors>
    <CodeAnalysisTreatWarningsAsErrors>true</CodeAnalysisTreatWarningsAsErrors>
    <EnforceCodeStyleInBuild>true</EnforceCodeStyleInBuild>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference
	   Include="SonarAnalyzer.CSharp"       
       PrivateAssets="all"
       Condition="$(MSBuildProjectExtension) == '.csproj'" />
  </ItemGroup>

</Project>
```

Now that default preperties are defined the following  lines can be removed from all `.csproj` files:
```
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
```

I also like to use centrally managed nuget packages. Add a `Directory.Packages.props` file to the root folder:
```
<Project>
  <PropertyGroup>
    <ManagePackageVersionsCentrally>true</ManagePackageVersionsCentrally>
  </PropertyGroup>
  <ItemGroup>
    <PackageVersion Include="Microsoft.Extensions.Http.Resilience" Version="10.1.0" />
    <PackageVersion Include="Microsoft.Extensions.ServiceDiscovery" Version="10.1.0" />
    <PackageVersion Include="OpenTelemetry.Exporter.OpenTelemetryProtocol" Version="1.14.0" />
    <PackageVersion Include="OpenTelemetry.Extensions.Hosting" Version="1.14.0" />
    <PackageVersion Include="OpenTelemetry.Instrumentation.AspNetCore" Version="1.14.0" />
    <PackageVersion Include="OpenTelemetry.Instrumentation.Http" Version="1.14.0" />
    <PackageVersion Include="OpenTelemetry.Instrumentation.Runtime" Version="1.14.0" />
    <PackageVersion Include="SonarAnalyzer.CSharp" Version="10.18.0.131500" />
  </ItemGroup>
</Project>
```

You'll then need to remove the version numbers from nuget packages in the ServiceDefaults.csproj file. The package references in section ofhat file should look like:
```
<PackageReference Include="Microsoft.Extensions.Http.Resilience" />
<PackageReference Include="Microsoft.Extensions.ServiceDiscovery" />
<PackageReference Include="OpenTelemetry.Exporter.OpenTelemetryProtocol" />
<PackageReference Include="OpenTelemetry.Extensions.Hosting" />
<PackageReference Include="OpenTelemetry.Instrumentation.AspNetCore" />
<PackageReference Include="OpenTelemetry.Instrumentation.Http" />
<PackageReference Include="OpenTelemetry.Instrumentation.Runtime" />
```

I also add a default `.editorconfig` file to the root folder. 

I also add `.editorconfig`, `Directory.Build.props`, `Directory.Packages.props` and `README.md`  to the solution items to make them easy to open later, and I might prepare the solution for gen ai tools with `CLAUDE.md` and/or `./github/copilot-instructions` files.

In `AppHost.cs` change the Run statement at the end to `await builder.Build().RunAsync();` - this might show a build warning so add this to .editorconfig to suppress it:
```
# Suppressions
[*.cs]
dotnet_diagnostic.CA2007.severity = none # Do not warn about missing ConfigureAwait
```

There will also be some warnings in in `ServiceDefaults/Extensions.cs` about commented code and some suggested improvements. Some of these may disappear in future versions of the Aspire templates, and you're free to make any changes you like as you develop your applications. But to start with I suppress them by adding a using statement for `System.Diagnostics.CodeAnalysiss`, wrapping a prgma around the namespace, and addind suppression attributes:
```
using System.Diagnostics.CodeAnalysis;

#pragma warning disable IDE0130
namespace Microsoft.Extensions.Hosting;
#pragma warning restore IDE0130

[SuppressMessage("Minor Code Smell", "S125:Remove this commented out code", Justification = "Allowing comments in this class", Scope = "type", Target = "~T:Microsoft.Extensions.Hosting.Extensions")]
[SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "Allowed in this project for now", Scope = "member", Target = "~M:Microsoft.Extensions.Hosting.Extensions.MapDefaultEndpoints(Microsoft.AspNetCore.Builder.WebApplication)~Microsoft.AspNetCore.Builder.WebApplication")]
[SuppressMessage("Globalization", "CA1307:Specify StringComparison for clarity", Justification = "Allowed in this project for now", Scope = "member", Target = "~M:Microsoft.Extensions.Hosting.Extensions.ConfigureOpenTelemetry``1(``0)~``0")]
[SuppressMessage("Style", "CA1724: The type name Extensions conflicts in whole or in part with the namespace name 'Microsoft.AspNetCore.Builder.Extensions'", Justification = "This is extending the target namespace and is not an error", Scope = "type", Target = "~T:Microsoft.Extensions.Hosting.Extensions")]
public static class Extensions
```

At this point you should be able to build and run the solution, and see a running application host with no resources. Done! You can go ahead with adding projects and building an awesome application. 

## Template it

The above is a pretty quick and painless process, especially if you have files that you can copy from an existing project. But we can do better by creating a template.

A bit of searching made this look trivial - just add a template configuration file and run a command. But it's never that easy...

[Automation!](https://xkcd.com/1319/)

There are a few problems. There are ports in `launchSettings.json` that need to be replaced, and if you selected the option to use the dev.localhost TLD when creating the soltion then the launch uris might be wrong because they have '.' where there should be '_'. These are all fixable but it takes some work. Fortunately more searching got me most of the way there, and the Aspire repo got me the rest - the original [Aspire templates](src/Aspire.ProjectTemplates/templates) are there for reference.

----------------------------

This is a starter app for an empty aspire project. It contains the minimal files and structure needed to get started with aspire development.

The repo has a folder `.template.config` with a file `template.json` that describes the template and its parameters.

The template has been configured to replace the user secrets guid in the AppHost project (see the `guids` section of the template.json file).

## Installing the template

Install the template by running this command in the repo root:
```
dotnet new install ./
```

If the template already exists, you need to add --force to reinstall:
```
dotnet new install ./ --force
```

You can list available templates with 
```
dotnet new list aspire
```

## Using the template

> [!NOTE]
> This won't work if the original project is open in Visual Studio. Close Visual Studio first.

Create a new project by running the following command:
```
dotnet new aspire-empty-starter -n My.Awesome.Project -o c:/dev/my-awesome-project
```

The -o parameter is optional - if you don't use it then the project will be created in a folder with the same name as the project.

This will create a new folder `My.Awesome.Project` with the aspire project structure.

## References

 - [](https://learn.microsoft.com/en-us/dotnet/core/tutorials/cli-templates-create-project-template)
 - [Reference for template.json](https://github.com/dotnet/templating/wiki/Reference-for-template.json)

 ### Templating notes

 https://github.com/dotnet/templating/wiki/Reference-for-template.json
 Ports - https://github.com/dotnet/templating/wiki/Available-Symbols-Generators#port
 Regex - https://github.com/dotnet/templating/wiki/Available-Symbols-Generators#regex
	

 https://github.com/sayedihashimi/template-sample/blob/main/src/Content/MyWebApp/.template.config/template.json


--------------------

[!NOTE] 
> The source for this article is available on [GitHub](https://github.com/mikewild-wcl/dotnet-templates)
