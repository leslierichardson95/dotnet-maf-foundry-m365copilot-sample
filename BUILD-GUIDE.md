# BUILD-GUIDE — Rebuilding this sample from scratch

This document mirrors the proposed M365 Copilot walkthrough on Microsoft Learn. Each section maps to one walkthrough part. Use this as the draft shape for the Learn pages.

> **Status:** Offline and Foundry cloud paths are verified. Microsoft 365
> tenant registration remains scaffolded and untested.

## Prereqs

- .NET 10 SDK
- Azure CLI (`az`) + Azure Developer CLI (`azd`)
- Azure subscription with permission to create Foundry projects + model deployments
- (For Part 5) M365 tenant with Copilot license + permission to sideload a custom engine agent
- (For Part 5) [M365 Agents Toolkit](https://learn.microsoft.com/en-us/microsoft-365/copilot/extensibility/) — VS or VS Code extension

## Part 2 — Build a MAF agent locally

**Goal.** A console MAF agent that calls a Foundry-deployed model and can invoke IT-helpdesk tools.

**Steps.**

1. Provision the `ai-project` service declared in `azure.yaml`.
2. The .NET project constructs `AIProjectClient` with
   `DefaultAzureCredential`.
3. `ITHelperAgent.Create` binds the configured model and five function tools.
4. Run `.\scripts\run-chat.ps1`; the helper imports the selected `azd`
   environment before starting the console loop.

**Verify.** Ask whether VPN is down. The agent must call
`CheckServiceStatus`. Offline, run `--self-test` to verify the deterministic
tool and KB plumbing.

**Doc gaps found.** See [`HICCUPS-AND-DOC-OPPORTUNITIES.md`](HICCUPS-AND-DOC-OPPORTUNITIES.md) — `[Part 2]` entries.

## Part 3 — Add grounding

**Goal.** Add file-based grounding from a small KB so the agent can answer policy / setup questions.

**Steps.**

1. Add Markdown articles under `src\IThelper.Agent\data\kb`.
2. Load them with `KnowledgeBase.Load`.
3. Register `SearchKnowledgeBase` as an `AIFunction`.
4. Tell the agent to cite the article title when the tool supplied its answer.

**Verify.** `--self-test` ranks `vpn-troubleshooting.md` first for a
VPN-disconnect query.

## Part 4 — Deploy as a Foundry Hosted Agent

**Goal.** Reproduce Part 2's behavior against a cloud-reachable Hosted Agent endpoint.

**Steps.**

1. Keep the model deployment under `services.ai-project.deployments` in
   `azure.yaml`.
2. Define `it-helper` with `host: azure.ai.agent` and .NET 10 direct code
   deployment.
3. Generate the checked-in Bicep from the project's manifest with
   `azd ai agent init --infra=bicep --no-prompt`.
4. Preview/provision, then `azd deploy it-helper --no-prompt`.

**Verify.** `azd ai agent show --output json` reports an active version and
`azd ai agent invoke ... --protocol responses` returns a tool-grounded answer.

## Part 5 — Register with M365 Copilot

**Goal.** Surface the MAF + Foundry Hosted Agent in a development tenant as a
custom engine agent over the Activity protocol.

**Steps.** Use the custom-engine/Activity-protocol flow in
[`docs/part-5-register-with-m365copilot.md`](docs/part-5-register-with-m365copilot.md).
This is intentionally marked untested.

**Verify.** A sideloaded app in Copilot Chat reaches the MAF agent and a test
message produces a tool call.

## Part 6a — Publish org-wide

**Goal.** Move from dev-tenant sideload to org-wide availability.

Docs only in this handoff — see [`docs/part-6a-publish-org-wide.md`](docs/part-6a-publish-org-wide.md).

## Part 6b — Add evaluation

**Goal.** Add local checks + cloud eval, with results visible in the Foundry portal.

**Steps.**

1. Validate `baseline.jsonl` offline.
2. Run MAF `LocalEvaluator` checks with `.\scripts\run-eval.ps1`.
3. Run `FoundryEvals` with `.\scripts\run-eval.ps1 -Foundry`.
4. Inspect the run under **Evaluation** in the Foundry portal.

**Verify.** The dataset has ten valid rows, local checks complete, and the
cloud run appears in the portal.
