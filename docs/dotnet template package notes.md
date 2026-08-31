# Create a dotnet template package

Changes needed to the dotnet template project.


**README.md**
[!NOTE] - the README file will be overwritten and used in the packaging process, so consider rewriting it and adding a docs folder.

## Packaging the template

Now that we have a working template, we can follow the tutorial [Create a template package for dotnet new - .NET | Microsoft Learn](https://learn.microsoft.com/en-us/dotnet/core/tutorials/cli-templates-create-template-package) and create a nuget package for it. This will make it easy to share with others.

You'll need the `Microsoft.TemplateEngine.Authoring.Templates` template. If you don't lready have this, install it by runningthe following command in the repo root folder:
```
dotnet new install Microsoft.TemplateEngine.Authoring.Templates
```

Then create a templating project:
```
dotnet new templatepack -n "WildConsulting.Aspire.Templates"
```

If you have an existing `README.md` file, the command will overwrite it. Either rename the file before running the command, or add `--force` to allow the command to run. The new `README.md` file will be included in the nuget package, so make sure it has the correct details.

A new content folder with a SampleTemplate will be added - this can be deleted.

Update the details in the project. This is what I have:
```
  <PropertyGroup>
    <!-- The package metadata. -->
    <!-- Follow the instructions on https://learn.microsoft.com/en-us/nuget/create-packages/package-authoring-best-practices -->
    <PackageId>WildConsulting.Aspire.Templates</PackageId>
    <PackageVersion>1.0</PackageVersion>
    <Title>Aspire project templates | Wild Consulting</Title>
    <Authors>Mike Wild | Wild Consulting Limited</Authors>
    <Description>Contains dotnet template projects for Aspire.</Description>
    <PackageTags>dotnet-new;templates;aspire</PackageTags>
    <PackageProjectUrl>https://github.com/mikewild-wcl/dotnet-templates</PackageProjectUrl>
    <PackageLicenseExpression>MIT</PackageLicenseExpression>

    ...
  </PropertyGroup>
```

The default template project file includes a line telling it where the content is. If you used a different name, change it. The default is
```
<Content Include="content\**\*" Exclude="content\**\bin\**;content\**\obj\**" />
```
This solution uses a `templates` folder, so change it to
```
    <Content Include="templates\**\*" Exclude="templates\**\bin\**;templates\**\obj\**" />
```

Save the project file, then in the top-level repo folder (the *working* folder) run the following command to build the nuget package:
```
dotnet pack
```

The first time I ran this it had an error
```
  WildConsulting.Aspire.Templates net10.0 failed with 1 error(s) (3.0s) → bin\Release\net10.0\WildConsulting.Aspire.Templates.dll
    C:\Program Files\dotnet\sdk\10.0.301\NuGet.Build.Tasks.Pack.targets(222,5): error The process cannot access the file 'C:\mike\aspire\dotnet-templates\templates\aspire-empty-starter\.vs\Aspire.EmptyStarter.slnx\FileContentIndex\89274539-0584-44a9-81c6-bbdc68eb92eb.vsidx' because it is being used by another process.
```

I closed the solution in Visual Studio and ran the command again, and it worked.

The nuget package will be created in the `bin\Release` folder. You can install it locally to test it by running the following command in a new folder:
```
dotnet new install <path to the nuget package>
```

From the root of this repo, the command would be:
```
dotnet new install .\bin\release\WildConsulting.Aspire.Templates.1.0.0.nupkg
```

You can list available templates with 
```
dotnet new list aspire
```

To uninstall, you need to uninstall using the package name:
```
dotnet new uninstall WildConsulting.Aspire.Templates
```

## Installing from nuget

After uploading the package to nuget you can just install it by name:
```
dotnet new install WildConsulting.Aspire.Templates
```

(On laptop use `dotnet new install MikeWild.Aspire.Templates` since this is the deployed version)


## TODO

Add a pipeline to build and publish the nuget package.

Refer to [wild-helper-packages/azure-pipelines.yml at main · mikewild-wcl/wild-helper-packages](https://github.com/mikewild-wcl/wild-helper-packages/blob/main/azure-pipelines.yml) - some of the tasks in there are obsolete.

`NuGetCommand@2` is now deprecated, and you should use the `NuGetAuthenticate@1` task combined with the .NET CLI task 

[NuGetAuthenticate@1 - NuGet authenticate v1 task | Microsoft Learn](https://learn.microsoft.com/en-us/azure/devops/pipelines/tasks/reference/nuget-authenticate-v1?view=azure-pipelines)

[How to build &amp; publish NuGet packages with Azure Pipelines - James Croft](https://www.jamescroft.co.uk/how-to-build-publish-nuget-packages-with-azure-pipelines/)
 - pipeline is here: [gist.githubusercontent.com/jamesmcroft/29531481e04117db3f32fbe35741128a/raw/8652ed39072a2b5b1980129cd29aea9844b46fea/nuget-azure-pipelines.yml](https://gist.githubusercontent.com/jamesmcroft/29531481e04117db3f32fbe35741128a/raw/8652ed39072a2b5b1980129cd29aea9844b46fea/nuget-azure-pipelines.yml)

#### GitVersion

[How to continuously deploy NuGet packages with Azure DevOps and GitVersion](https://robwest.info/articles/how-to-continuously-deploy-nuget-packages-with-azure-devops-and-gitversion/). Code is [here](https://github.com/robertgregorywest/nuget-deployment-demo).

An older series Versioning NuGet packages in a continuous delivery world: - [part 1](https://devblogs.microsoft.com/devops/versioning-nuget-packages-cd-1/), [part 2](https://devblogs.microsoft.com/devops/versioning-nuget-packages-cd-2/), [part 3](https://devblogs.microsoft.com/devops/versioning-nuget-packages-cd-3/). 

### Sample

repo https://dev.azure.com/mwplayground/_git/nuget-deployment
git clone https://mwplayground@dev.azure.com/mwplayground/nuget-deployment/_git/nuget-deployment

https://github.com/sayedihashimi/template-sample/blob/main/src/Content/MyWebApp/.template.config/template.json

## CI/CD pipeline

The pipeline (`azure-pipelines.yml`) has two stages:

| Stage | Trigger | Purpose |
|---|---|---|
| **Build & Test** | Every push to `main`, every PR, every `v*` tag | Restore, version, and build |
| **Pack & Publish** | `v*` tags only | Pack and push to NuGet.org |

### Versioning

Versioning is handled automatically by [GitVersion](https://gitversion.net/) using the configuration in `GitVersion.yml`. The resolved `NuGetVersionV2` value is passed into both `dotnet build` and `dotnet pack` via `/p:Version=`.

To release a new version, push a tag in the format `v<semver>`:

```bash
git tag v1.2.3
git push origin v1.2.3
```

GitVersion derives the package version from the tag. The Publish stage only runs when the build is triggered by a `v*` tag.

### Path exclusions

The CI trigger ignores the following paths so documentation-only changes do not trigger a build:

- `README.md`, `AGENTS.md`, `CLAUDE.md`
- `docs/**`
- `.github/` Copilot instruction and skill files
- `.agents/skills/**`

### One-time Azure DevOps setup

#### GitTools extension

**Is this needed?** The ipeline now installs GitVersion directly.

The Build stage uses GitTools so this needs to be installed as an extension. Install it once for your organization:

1. Go to **Organization Settings → Extensions**
2. Click the **Browse marketplace** button
3. Search for GitTools
4. Install it

#### NuGet service connection

The Publish stage pushes to NuGet.org using a service connection. Create it once per Azure DevOps project:

1. Go to **Project Settings → Service connections → New service connection**
2. Choose **NuGet**
3. Set the feed URL to `https://api.nuget.org/v3/index.json`
4. Paste your [NuGet.org API key](https://www.nuget.org/account/apikeys) — scope it to the specific package ID rather than all packages
5. Name the connection **`NuGetOrg`** — this must match the `publishFeedCredentials` value in the pipeline
6. Grant access permission to the pipeline when prompted

**Alternative approach** - use a variable

You can use the API key directly as a pipeline variable in Azure DevOps:
- Pipelines > Edit pipeline > Variables
Add a variable called `NuGetApiKey` with the key value, and make it secret.


#### GitVersion extension

The pipeline uses the [GitTools Azure Pipelines extension](https://marketplace.visualstudio.com/items?itemName=gittools.gittools). Install it in your Azure DevOps organisation before running the pipeline for the first time.

### Azure DevOps Environment for NuGet Publishing Approval

This pipeline uses an Azure DevOps environment named `nuget-publish-approval` as a manual approval gate before the publish stage runs.

#### Steps to create the environment

1. Open your Azure DevOps project.
2. In the left navigation, go to `Pipelines` and then `Environments`.
3. Click `New environment`.
4. Enter the name:
   - `nuget-publish-approval`
5. Click `Create`.

#### Steps to add approval for the environment

1. Open the newly created environment.
2. Go to `Approvals and checks`.
3. Click `Add check`.
4. Select `Approval`.
5. Choose the user or group that should be allowed to approve deployments.
6. Save the approval check.

#### Notes

- The pipeline references this environment by name in the publish stage:
  - `environment: 'nuget-publish-approval'`
- When the publish stage starts, Azure DevOps will pause until the designated approver approves the deployment.
- If no approver is configured, the deployment will not have a required manual approval gate.

## Changes on laptop - 27/07/2026

#### editorconfig

Added line 286 under ServiceDefaults:
```
dotnet_diagnostic.S3241.severity = none # S3241: Change return type to 'void'; not a single caller uses the returned value.
```

#### global.json

Added `global.json` to the template solution and to the templated solution. At time of writing, the latest SDK varsion was 10.0.302 (July 2026 update).
```
{
  "sdk": {
    "version": "10.0.302",
    "rollForward": "latestFeature",
    "allowPrerelease": false
  }
}
```

#### Others
\src\templates\aspire-empty-starter\.template.config\template.json

 - under `"classifications": ` change `.NET Aspire` to just `Aspire`

Postactions

Post actions are listed in the [Post Action Registry](https://github.com/dotnet/templating/wiki/Post-Action-Registry/5dfff87cb809bd9e3fb3ea9a5a7fd3d4f0a2aa4d)

The post actions need the primary output to be set - see [Using Primary Outputs for Post Actions](https://github.com/dotnet/templating/wiki/Using-Primary-Outputs-for-Post-Actions).

Added to end of template.json:
```
  "primaryOutputs": [
    {
      "path": "./Aspire.EmptyStarter.Template.sln"
    }
  ],
  "postActions": [
    {
      "actionId": "210D431B-A78B-4D2F-B762-4ED3E3EA9025",
      "description": "Restore NuGet packages required by this solution.",
      "condition": "(!skipRestore)",
      "continueOnError": true,
      "manualInstructions": [
        {
          "text": "Run 'dotnet restore'"
        }
      ]
    }
  ]
```

This sets the primary output to the solution and the post-creation action to make sure nuget packages are restored.


Some learning from https://ghanavats.tech/blog/2026-06-14-pack_you_dotnet_project_as_template/ and follow-up https://ghanavats.tech/blog/2026-07-24-one_dotnet_template_for_many_configurations/. Most of this was too late for me because I'd already gone through the pain :)



## Next steps

Migrate from https://dev.azure.com/mwplayground/_git/nuget-deployment

Remove https://www.nuget.org/packages/MikeWild.Aspire.Templates

** rename templates foler to content, to be consistent.

Rebuild with the original name - this can be set in the pipeline:
```
  - name: templateProjectPath
    value: 'src/MikeWildAspire.Templates.csproj'
```

NuGet has changed the way authentication is done, and is recommending replacing API keys, which will be limited to 30 days, and moving to trusted publishing. See the announcement [Strengthening NuGet Supply Chain Security: Reducing API Key Lifetime](https://devblogs.microsoft.com/dotnet/strengthening-nuget-supply-chain-security-reducing-api-key-lifetime/) and setup instructions [Trusted Publishing on nuget.org](https://learn.microsoft.com/en-gb/nuget/nuget-org/trusted-publishing).

**NOTE**: This doesn't seem to be supported Azure DevOps pipelines yet.


## File templates and optional files in project

[How to create your own templates for dotnet new | Add optional content](https://devblogs.microsoft.com/dotnet/how-to-create-your-own-templates-for-dotnet-new/#add-optional-content)
[.NET Templates with Optional Content | Excluding Entire Folders Conditionally](https://knowyourtoolset.com/2024/11/templates-optional-content/#excluding-entire-folders-conditionally)
[Creating Custom Project and Item Templates in .NET](https://thomasngoswe.com/2026/03/11/dotnet-custom-templates/)
[Creating Project Templates for dotnet – Part 2 – Optional Files - .Net Ninja](https://dotnetninja.net/2021/03/creating-project-templates-for-dotnet-part-2-optional-files/)

You can bootstrap an empty template using this command: 
```
dotnet new projecttemplate -n MyTemplate.Sample -o MyTemplate.Sample
```
By default this creates an item template. To create a project template, add `--project`. This doesn't seem to do much other than creating a template project and `content` folder. 


Not template-related, but an article on an Aspire extension for setting logging config - [Using Shared Logging Levels with .NET Aspire](https://knowyourtoolset.com/2024/09/aspire-logging-levels/)


### Fixes 19/08/2026

Added optional `--application-name` parameter
- need to add to README
- files changed or added - copied:
  - AppHost.cs
  - Shared/ApplcationConstants.cs
  - docs/application-name-template-parameter.md
  - docs/pipeline-security
  - Add .gitattributes to template

DONE Add .gitattributes to template solution (src\templates\aspire-empty-starter)
     Avoids line-ending warnings in generated files

Added tagging to pipeline after nuget push. Had to change repository security in the devops project to make this work.
> Added a PowerShell step post-NuGet push to ensure a single .nupkg file, extract its version, and create/push a Git tag (v{version}) if it doesn't exist. Handles errors for multiple packages, version parsing, and tag operations, logging and exiting on failure.

DONE **CLI Bundle** - Add this to AppHost csproj:
```
  <AspireUseCliBundle>true</AspireUseCliBundle>
```