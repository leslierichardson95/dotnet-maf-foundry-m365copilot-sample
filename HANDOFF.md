# HANDOFF — Read this first

**For:** Microsoft Foundry Docs + MAF teams inheriting this repo.
**From:** Leslie Richardson (last day at Microsoft: shortly after this repo was created).
**Purpose:** Give you a verified starter kit for the proposed .NET × MAF ×
Foundry × M365 Copilot walkthrough on Microsoft Learn, with unfinished tenant
integration work clearly identified.

## Why this exists

The proposed walkthrough
([`DotnetFoundryM365CopilotWalkthrough-Outline.md`](docs/reference/DotnetFoundryM365CopilotWalkthrough-Outline.md))
needs a runnable sample so readers can follow along. This repo is that sample,
verified end-to-end where possible.

## The scenario

A minimal **IT helpdesk triage assistant**:

- **Tools:** `CheckServiceStatus`, `LookupUserTickets`, `CreateTicket`, `EscalateToOnCall`
- **Backends:** in-memory only (seeded in code) — no real ITSM connection
- **Optional grounding:** 10 Markdown KB articles under
  `src\IThelper.Agent\data\kb`

## What runs vs. what doesn't

| Walkthrough part | State | Notes |
|---|---|---|
| 1 — Overview | 📄 Docs | Prose page |
| 2 — MAF agent locally | ✅ Verified | Builds and runs against the provisioned Foundry model |
| 2 sidebar — Foundry Local | 📄 Docs | Not exercised in code |
| 3 — Grounding (file-based) | ✅ Verified offline | 10 KB articles + in-memory retrieval |
| 3 alt — Foundry IQ | 📄 Docs | Not exercised |
| 4 — Hosted Agent deploy | ✅ Verified | Version 5 is active; multi-turn identity reuse and tool-grounded responses pass |
| 5 — M365 Copilot registration | ⚠️ Scaffold | Custom engine agent via Activity protocol + M365 app manifest; sideload untested |
| 6a — Org-wide publish | 📄 Docs | Requires tenant admin approval flow |
| 6b — Evaluation | ✅ Verified | Local checks passed and a Foundry cloud evaluation completed and appears in the portal |

## Where to start

1. Read [`README.md`](README.md) for orientation.
2. Read [`docs/QUICKSTART.md`](docs/QUICKSTART.md) and complete the Azure confirmation/provisioning gate.
3. Read [`HICCUPS-AND-DOC-OPPORTUNITIES.md`](HICCUPS-AND-DOC-OPPORTUNITIES.md) — this is your doc backlog, drawn from real friction Leslie hit while building.
4. Use [`BUILD-GUIDE.md`](BUILD-GUIDE.md) as the shape for the Learn walkthrough drafts.

## Recommended continuation sequence

Treat the repository as progressive stages; do not introduce every layer in
the first article.

1. **Preserve the beginner path:** teach the local console agent, one tool,
   and `AgentSession` first.
2. **Add deterministic grounding:** introduce the Markdown knowledge base and
   citations without adding another Azure dependency.
3. **Teach cloud deployment:** provision the Foundry project/model and deploy
   the same code as a Hosted Agent.
4. **Complete the remaining integration:** implement and validate the M365
   custom-engine Activity-protocol adapter, app registration, and tenant
   sideload.
5. **Turn the verified sample into Learn content:** use `BUILD-GUIDE.md` for
   page boundaries and `HICCUPS-AND-DOC-OPPORTUNITIES.md` for required
   troubleshooting callouts.
6. **Productionize only as follow-up work:** add Foundry IQ, real ITSM
   backends, durable user memory, CI/CD quality gates, and org-wide publishing
   after the core walkthrough is stable.

## Open questions to resolve

- **Repo home.** Suggested publish target: a new public repo under a joint MAF
  / Foundry Docs owner (candidates: `Azure-Samples`, `microsoft`, `Azure`).
  Named owner is still to be decided.
- **M365 Copilot integration validation.** Current docs identify this as a
  custom engine agent and point to Microsoft 365 Agents SDK + Activity
  protocol. The receiving team still needs to validate the generated channel
  setup and tenant sideload end-to-end.
- **Foundry IQ .NET follow-up.** Keep file-based grounding in the beginner
  walkthrough. Identify the best current Foundry IQ article and .NET
  authorization pattern for a production-upgrade callout.

## What this repo is NOT

- Not a polished product sample. It's a starter kit that proves the walkthrough works.
- Not tenant-, subscription-, or endpoint-specific. All Azure values come from `azd` env / `DefaultAzureCredential`.
- Not tested against M365 Copilot (no shared tenant during authoring).
- Not a substitute for the walkthrough outline itself — a copy is preserved under `docs\reference\`.

## Contact

Leslie Richardson is offline after handoff. Route follow-ups through the MAF team + Foundry Docs team leads.
