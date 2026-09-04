# dotnet-maf-foundry-m365copilot-sample

> **A .NET agent using Microsoft Agent Framework (MAF), deployed to Microsoft Foundry, that shows up inside Microsoft 365 Copilot.**

This repo is the runnable companion to a proposed Microsoft Learn walkthrough for .NET developers who want a MAF-based Foundry agent to be callable inside M365 Copilot. It is intentionally scoped as a **starter kit** — a proven-working baseline for the walkthrough's docs and code, not a polished product sample.

## What it demonstrates

A minimal **IT helpdesk triage assistant** built with:

- **Microsoft Agent Framework (MAF)** — agent + tool authoring in C#
- **Microsoft Foundry** — model deployment + hosted agent
- **Microsoft 365 Copilot** — custom engine agent scaffold using the M365 app model and Activity protocol
- **`Microsoft.Extensions.AI.Evaluation`** + **Foundry evaluators** — local + portal-visible eval

## Quickstart

See [`docs/QUICKSTART.md`](docs/QUICKSTART.md).

After choosing a subscription and region:

```powershell
azd auth login
azd env new dev
azd provision
azd deploy it-helper
.\scripts\run-chat.ps1
```

You need: an Azure subscription with Foundry access, .NET 10, `azd` CLI, and
(for Part 5) an M365 tenant with Copilot licensing and permission to sideload
custom engine agents.

## What runs today vs. what doesn't

See [`HANDOFF.md`](HANDOFF.md). Short version:

- ✅ .NET 10 build, deterministic helpdesk tools, file-based grounding, and the 10-row eval dataset are verified offline.
- ✅ Foundry model, active Hosted Agent version, multi-turn session retention,
  remote tool invocation, and a
  portal-visible cloud evaluation have been verified in a development
  environment.
- ⚠️ M365 Copilot custom-engine registration (Part 5) is scaffolded and needs a suitable tenant to test.
- 📄 Org-wide publish (Part 6a) — docs only.

## Docs in this repo

- [`HANDOFF.md`](HANDOFF.md) — **read this first** if you're picking this up as a handoff
- [`BUILD-GUIDE.md`](BUILD-GUIDE.md) — walkthrough-style rebuild recipe
- [`HICCUPS-AND-DOC-OPPORTUNITIES.md`](HICCUPS-AND-DOC-OPPORTUNITIES.md) — friction log + ready-to-write doc issues
- [`DotnetFoundryM365CopilotWalkthrough-Outline.md`](docs/reference/DotnetFoundryM365CopilotWalkthrough-Outline.md) — proposed staged Microsoft Learn walkthrough that this sample implements
- [`docs/QUICKSTART.md`](docs/QUICKSTART.md), [`docs/AUTH-AND-CONFIG.md`](docs/AUTH-AND-CONFIG.md), and per-part docs in [`docs/`](docs/)

## License

MIT — see [`LICENSE`](LICENSE).
