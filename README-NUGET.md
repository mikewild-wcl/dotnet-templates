# .NET Templates

# How this template project works

This package contains dotnet templates including

- a starter app for an empty aspire project with minimal files and structure needed to get started with aspire development.

## Installing the template

Install the template by running this command in the template root - `cd C:\dev\dotnet-templates\templates\aspire-empty-starter` (the path on your machine might differ, and remember to change the template name if you're using a different template:
```
dotnet new install ./
```

You can also do this using the full path:
```
dotnet new install C:\dev\dotnet-templates\templates\aspire-empty-starter
```

If the template already exists, you need to add --force to reinstall:
```
dotnet new install ./ --force
```

You can list available templates with 
```
dotnet new list aspire
```

To uninstall the template, run 
If the template already exists, you need to add --force to reinstall:
```
dotnet new uninstall aspire-empty-starter
```
This might need the full path to the folder if you aren't inside it.
```
dotnet new uninstall C:\dev\dotnet-templates\templates\aspire-empty-starter
```

## Using the template

> [!NOTE]
> This might not work if the original template project is open in Visual Studio. Close Visual Studio first if you have any problems. If you re-run the template it might have erros when replacing files; if that happens add `--force`.

Create a new project by running the following command:
```
dotnet new aspire-empty-starter -n My.Awesome.Project -o c:/dev/my-awesome-project
```

The -o parameter is optional - if you don't use it then the project will be created in a folder with the same name as the project.

This will create a new folder `My.Awesome.Project` with the aspire project structure.

Optional parameters:

- `--application-name` - The name of the application. This will be used as the default name displayed in the AppHost, and is added as a constant in the `.Shared\ApplicationConstants.cs.

