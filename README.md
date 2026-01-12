# .NET Templates

# How this template project works

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
