# Azure Pipelines repository permissions for release tagging

When the publish pipeline pushes a Git tag (for example `v1.0.3`) after NuGet release, the pipeline identity must have repository write permissions.

## Symptom

Tag push fails with a 403 and a message similar to:

`TF401027: You need the Git 'GenericContribute' permission to perform this action.`

## Required repository permissions

In Azure DevOps, grant these permissions on the `nuget-deployment` repository to the build service identity used by the pipeline:

- **Contribute** (maps to `GenericContribute`)
- **Create tag**

If a parent group has **Deny** for Contribute, remove that deny. Deny overrides Allow.

## How to grant permissions

1. Open **Project settings** in Azure DevOps.
2. Go to **Repositories -> nuget-deployment -> Security**.
3. Find or add the pipeline identity (typically one of):
   - `<ProjectName> Build Service (<Org>)`
   - `Project Collection Build Service (<Org>)`
4. Set **Contribute** = **Allow** and **Create tag** = **Allow**.
5. Save and rerun the pipeline.

## Pipeline YAML prerequisite

The checkout step in the deployment job should keep credentials so `git push` can authenticate:

```yaml
- checkout: self
  persistCredentials: true
```
