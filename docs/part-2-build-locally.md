# Part 2 — Build and run the MAF agent locally

## Goal

Create a .NET 10 MAF agent that uses a Foundry model while the agent code runs
on the developer's machine.

## What this sample contains

- `ITHelperAgent.Create` — instructions, model binding, and tool registration.
- `HelpdeskTools` — service status, ticket lookup/create/escalate, and KB search.
- `FoundrySettings` — environment-variable validation.
- `Program.cs` — `--chat`, `--self-test`, and Hosted Agent server modes.

## Build

```powershell
dotnet restore
dotnet build
```

## Verify without Azure

This verifies the deterministic tools and file-based retrieval:

```powershell
dotnet run --project src\IThelper.Agent -- --self-test
```

## Verify with a Foundry model

Provision first, then:

```powershell
.\scripts\run-chat.ps1
```

The helper imports the selected `azd` environment into the process before
starting .NET. The model is remote, but the MAF orchestration and tools execute
locally.

The console keeps one MAF `AgentSession` for the life of the chat, so details
such as an email address remain available on later turns. Enter `/reset` to
start a fresh conversation or `exit` to close the app. This is conversation
history, not durable memory across process restarts.

## Optional: Foundry Local

Foundry Local could replace the cloud model during the inner loop if the
selected local model supports the required tool-calling behavior. This build
did not validate that endpoint swap. Treat it as a follow-up experiment rather
than a prerequisite.
