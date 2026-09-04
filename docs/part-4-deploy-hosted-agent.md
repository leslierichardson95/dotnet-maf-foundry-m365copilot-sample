# Part 4 — Deploy as a Foundry Hosted Agent

## Model deployment vs. agent deployment

Part 2 deploys or reuses a **model** in Foundry while the agent code runs
locally. Part 4 deploys the **agent code**—MAF instructions, tool wiring, and
runtime—as a cloud-hosted service. The model deployment remains the same.

## Why make the transition

- Microsoft 365 Copilot cannot call a developer's localhost process.
- The agent needs a stable, always-on HTTPS endpoint.
- The hosted runtime supplies identity, health, versioning, and telemetry.
- Evaluation and monitoring can reference a deployed agent version.

## Infrastructure source

`azure.yaml` is the human-readable source of truth. The checked-in `infra\`
folder was generated from this repository's manifest by the official command:

```powershell
azd ai agent init --infra=bicep --no-prompt
```

No external starter pack infrastructure was imported.

The manifest sets `AZURE_AI_MODEL_DEPLOYMENT_NAME` explicitly and requests 50
GlobalStandard capacity units. In validation, capacity 10 could rate-limit a
normal multi-tool response while evaluation setup was also using the model.

## Deploy

```powershell
azd provision --no-prompt
azd deploy it-helper --no-prompt
```

The service uses direct .NET code deployment (`codeConfiguration`) rather
than Docker/ACR.

## Verify

```powershell
azd ai agent show --output json
azd ai agent invoke "Is the VPN service down?" --protocol responses
```

Expect an active agent version and a response grounded in the
`CheckServiceStatus` tool output.

> **When to self-host instead:** use Container Apps, App Service, AKS, or your
> existing runtime when compliance, private-network topology, existing compute,
> portability, or team-owned CI/CD and observability outweigh the convenience
> of Foundry Hosted Agent.
