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

I also add `.editorconfig`, `Directory.Build.props`, `Directory.Packages.props` and `README.md`  to the solution items to make them easy to open later, and I might prepare the solution for gen ai tools with `CLAUDE.md` and/or `./github/copilot-instructions` files.

In `AppHost.cs` change the Run statement at the end to `await builder.Build().RunAsync();` - this might show a build warning so add this to .editorconfig to suppress it:
```
# Suppressions
[*.cs]
dotnet_diagnostic.CA2007.severity = none # Do not warn about missing ConfigureAwait
```

There will also be some warnings in in `ServiceDefaults/Extensions.cs` about commented code and some suggested improvements. Some of these may disappear in future versions of the Aspire templates, and you're free to make any changes you like as you develop your applications. But to start with I suppress them by adding a using statement for `System.Diagnostics.CodeAnalysiss`, wrapping a prgma around the namespace, and addind suppression attributes:
```
[**/*ServiceDefaults/**.cs] 
dotnet_style_namespace_match_folder = false:silent # IDE0130
dotnet_diagnostic.CA1062.severity = none # CA1062:Validate arguments of public methods
dotnet_diagnostic.CA1307.severity = none # CA1307:Specify StringComparison for clarity
dotnet_diagnostic.S125.severity = silent # S125:Remove this commented out code
```

It's worth adding a couple of other suppressions for future us, including this for unit test names:
```
# Unit test project rules
[**/*Tests/**.cs]
dotnet_diagnostic.CA1707.severity = none # Allow underscores in test names
```


At this point you should be able to build and run the solution, and see a running application host with no resources. Done! You can go ahead with adding projects and building an awesome application. 

## Template it

The above is a pretty quick and painless process, especially if you have files that you can copy from an existing project. But we can do better by creating a template.

A bit of searching made this look trivial - just add a template configuration file and run a command. But it's never that easy...

[Automation!](https://xkcd.com/1319/)

There are a few problems. There are ports in `launchSettings.json` that need to be replaced, and if you selected the option to use the dev.localhost TLD when creating the soltion then the launch uris might be wrong because they have '.' where there should be '_'. That causes certificate errors at runtime because the dev https certificate doesn't trust the uri with dots in it. There are also a few files that need to be excluded. For example the bin and obj folders also need to be excluded, if the `.vs` folder is copied any files open in the template are displayed when the new project is opened.

These are all fixable but it takes some work. Fortunately more searching got me most of the way there, and the Aspire repo got me the rest - the original [Aspire templates](src/Aspire.ProjectTemplates/templates) are there for reference.

The problem with the uris for TLD went away when I renamed the project so it had an underscore in `launchSettings.json` so there was no need to fix it. That was a relief because it looked like it needed some regex magic.

** Ports ** - since I copied the port replacements from the Aspire project, I changed `launchSettings.json` to match. 


----------------------------

## Adding a Shared project, sometimes

The standard Aspire templates use “magic strings” to name projects and resources. I often ignore this for smaller projects or proofs of concepts, but will for larger projects I will add a shared project with constants. The details on how this works can be found in [Removing Magic Strings from Your .NET Aspire Project](Removing Magic Strings from Your .NET Aspire Projec) - Michael S. Collier's Blog, which also mentions a video [.NET Aspire - Project Names and Constants](https://youtu.be/Jt39GzYCRgo?si=31l51xklkOjKP_Ns). 
-	Add a new class library. I like to name it <my-project>.Shared
-	Add the project as a project reference in the AppHost and other projects that need have the magic strings
-	In the AppHost project csproj file, add this attribute to the shared project – IsAspireProjectResource="false"
  <ItemGroup>
    <ProjectReference Include="..\Aspire.Default.Shared\Aspire.Default.Shared.csproj" IsAspireProjectResource="false" />
  </ItemGroup>
- If you're using strict static cheecks, the name "Shared" will cause a build error (Visual Studio helpfully hides this in the Build output instead of in the Error List) but it can easily be ignored by adding the following to the `<my-project>.Shared.csproj`:
```
<PropertyGroup>
  <!-- Suppress CA1716: Rename namespace so that it no longer conflicts with the reserved language keyword 'Shared' -->
  <NoWarn>CA1716</NoWarn>
</PropertyGroup>
```
- The PropertyGroup in that file can be removed because everything will come from the directory build props.
-	For this initial project I have added Resources and Parameters classes with sample constants.

I've also included this in the AppHost.csproj as a hint to how you can use a shorter name for projects:
```
<!-- 
  Tip: Use AspireProjectMetadataTypeName with new projects and use the short name in AppHost.cs:
    builder.AddProject<SampleApi>(ResourceNames.SampleApi)
-->
<!--<ProjectReference Include="..\Aspire.EmptyStarter.SampleApi\Aspire.EmptyStarter.SampleApi.csproj" AspireProjectMetadataTypeName="SampleApi" />-->
```

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

## Future improvements

The Aspire templates use “magic strings” to name projects and resources so you need to make sure you keep the names consistent across projects. If you want to avoid this, and as a best practice, 

See [Removing Magic Strings from Your .NET Aspire Project] on Michael S. Collier's Blog, which also references a [video](https://www.youtube.com/watch?v=Jt39GzYCRgo) by [Jeff Fritz](https://www.youtube.com/@csharpfritz) on the topic. https. I often ignore this for smaller projects or proofs of concepts, but will do it for larger projects.
-	Add a new class library. I like to name it with a `.Shared` extension, or possibly `.Configuration` since I might want to add a shared configuration class as well.
-	Add the project as a project reference in the AppHost and other projects that need have the magic strings
-	In the AppHost project csproj file, add this attribute to the shared project – IsAspireProjectResource="false"
  <ItemGroup>
    <ProjectReference Include="..\Aspire.My.Awesome.Project.Shared\Aspire.My.Awesome.Project.Shared.csproj" IsAspireProjectResource="false" />
  </ItemGroup>
-	Add constants to the shared project, such as Services.MyService and replace the magic strings in the AppHost code.

## References

 - [](https://learn.microsoft.com/en-us/dotnet/core/tutorials/cli-templates-create-project-template)
 - [Reference for template.json](https://github.com/dotnet/templating/wiki/Reference-for-template.json)

 ### Templating notes and links

 https://github.com/dotnet/templating/wiki/Reference-for-template.json
 Ports - https://github.com/dotnet/templating/wiki/Available-Symbols-Generators#port
 Regex - https://github.com/dotnet/templating/wiki/Available-Symbols-Generators#regex
	

 https://github.com/sayedihashimi/template-sample/blob/main/src/Content/MyWebApp/.template.config/template.json


--------------------

[!NOTE] 
> Source code is available on [GitHub](https://github.com/mikewild-wcl/dotnet-templates)
