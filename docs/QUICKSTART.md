# Quickstart

This quickstart creates your own Foundry project and model deployment, runs
the .NET 10 MAF agent locally, and then deploys it as a Hosted Agent.

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Azure CLI](https://learn.microsoft.com/cli/azure/install-azure-cli)
- [Azure Developer CLI](https://learn.microsoft.com/azure/developer/azure-developer-cli/install-azd) 1.33.0 or later
- Azure subscription where you can create Microsoft Foundry resources and role assignments

## 1. Install the Foundry `azd` extension

```powershell
azd extension install microsoft.foundry
```

The extension installs its `azure.ai.*` dependencies.

## 2. Sign in

Run these yourself so you control which identity is used:

```powershell
az login
azd auth login
```

## 3. Create an environment

From the repository root:

```powershell
azd env new dev --no-prompt
azd env set AZURE_SUBSCRIPTION_ID "<your-subscription-id>"
azd env set AZURE_LOCATION "northcentralus"
azd env set AZURE_TENANT_ID "$(az account show --query tenantId -o tsv)"
azd env set AZURE_PRINCIPAL_ID "$(az ad signed-in-user show --query id -o tsv)"
azd env set AZURE_PRINCIPAL_TYPE "User"
azd env set AZURE_AI_MODEL_DEPLOYMENT_NAME "gpt-5.4-mini"
```

`AZURE_TENANT_ID` keeps `azd` from probing unrelated guest tenants, and
`AZURE_PRINCIPAL_ID` grants the signed-in developer the project-scoped
**Cognitive Services User** role needed for local chat and evaluation.

Do not rely on the Azure CLI and `azd` defaults matching. Verify:

```powershell
azd env get-values
```

## 4. Preview and provision

```powershell
azd provision --preview --no-prompt
azd provision --no-prompt
```

After provisioning:

```powershell
azd env get-values
```

You should have `FOUNDRY_PROJECT_ENDPOINT`, `AZURE_AI_PROJECT_ID`,
`AZURE_AI_MODEL_DEPLOYMENT_NAME`, and `AZURE_RESOURCE_GROUP`. The model name
was set explicitly above because the Foundry provisioning provider emits the
deployment collection but does not currently create that individual variable.

## 5. Run locally

```powershell
azd ai agent run --no-client
```

For the simple console loop:

```powershell
.\scripts\run-chat.ps1
```

Try:

> How should I troubleshoot frequent VPN disconnects?

The response should use the service-status and knowledge-base tools and cite
`vpn-troubleshooting.md`.

## 6. Deploy the Hosted Agent

```powershell
azd deploy it-helper --no-prompt
azd ai agent show --output json
azd ai agent invoke "Is the VPN service down?" --protocol responses
```

## 7. Run evaluation

Validate the local dataset without cloud calls:

```powershell
dotnet run --project src\IThelper.Eval -- --validate-dataset
```

Run MAF local checks against the model:

```powershell
.\scripts\run-eval.ps1
```

Add `--foundry` to submit the cloud evaluator path:

```powershell
.\scripts\run-eval.ps1 -Foundry
```

See [Part 6b](part-6b-add-eval.md) for the portal flow.

## 8. Clean up

`azd down` deletes the environment's resources and evaluation telemetry. Ask
your resource owner before running it:

```powershell
azd down
```
