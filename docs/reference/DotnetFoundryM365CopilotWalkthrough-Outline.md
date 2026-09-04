# .NET × MAF × Foundry → Microsoft 365 Copilot Walkthrough — Outline

**Status:** Draft outline, updated with findings from the working sample.
**Audience:** .NET developers who want a MAF-based Foundry agent to show up inside Microsoft 365 Copilot.
**Relationship to bigger series:** This is a narrower, ship-first slice of the full 8-part .NET Foundry Learn series (`DotnetFoundryLearnSeries-Outline.md`). It corresponds roughly to Series Parts 1 + 3 (optional) + 6 + 6.5, tightened to a single end-user surface.

---

## Goals

- Give a .NET dev a linear, ~60–90 minute path from empty folder to "my agent answers inside M365 Copilot."
- Anchor on **MAF** (framework) + **Foundry Agent Service / Hosted Agents**
  (hosting) + the **Microsoft 365 custom engine agent** path (Copilot
  registration).
- Prove the E2E pattern with the smallest number of moving parts; layer grounding, eval, and offline dev as **optional**.

## Design principles

- **Ship the "hello world" fast.** Getting a MAF agent to speak inside M365 Copilot is the win; everything else is an upgrade.
- **One primary surface: M365 Copilot chat.** Teams-specific customization, Copilot Studio integration, and other end-user surfaces are out of scope — link out.
- **No Aspire, no Semantic Kernel** in the main path. Aspire may appear as a "for larger topologies" callout only.
- **Foundry Local and Foundry evaluators are optional add-ons**, not required steps.
- **Grounding is an optional upgrade**, not a prerequisite — see reasoning below.
- **Every hard prereq is called out before Part 1**, especially the M365 Copilot license + tenant sideload permission gate.

## Running scenario

**Internal assistant for HR/IT/policy Q&A + light action triggering** inside M365 Copilot.

- The MAF agent handles reasoning, tools, and optionally grounded answers.
- M365 Copilot handles the chat UX, identity, and Graph-based context.
- Optional grounding path: a small file-based knowledge base in the walkthrough;
  Foundry IQ is the production-oriented follow-up.

Why this scenario: canonical M365 Copilot fit, demoable without customer data, and it justifies both the "just reasoning" version (Parts 1–5) and the "grounded + evaluated" version (optional Part 3, optional Part 6b).

---

## Prereqs (called out **before** Part 1)

- .NET 10 SDK, VS 2026 or VS Code with C# Dev Kit
- Azure subscription with a Foundry project (or ability to create one)
- **M365 tenant with a Microsoft 365 Copilot license** and permission to
  sideload custom engine agents
- M365 Agents Toolkit (VS / VS Code extension)
- Foundry Toolkit (VS / VS Code extension)

> ⚠️ **The tenant + license gate is the #1 reason readers will bounce.** Surface it in a red callout at the top of the overview page with a link to a "no Copilot license? try this instead" fallback (probably the generic hosted-agent quickstart).

---

## Summary table

| # | Title | Kind | Required? | Approx. words |
|---|-------|------|-----------|---------------|
| 1 | Overview: MAF agents in M365 Copilot | Concept | Required | ~1200 |
| 2 | Build a MAF agent locally against a Foundry model | Quickstart | Required | ~1000 |
| 2-appx | Iterate offline with Foundry Local *(callout, not its own page)* | Callout | Optional | ~300 |
| 3 | Add file-based grounding; introduce Foundry IQ as the next step | How-to | **Optional** | ~1500 |
| 4 | Deploy as a Foundry Hosted Agent | How-to | Required | ~1500 |
| 5 | Register the agent with M365 Copilot (custom engine agent) | Tutorial | Required | ~2500 |
| 6a | Publish org-wide | How-to | Required | ~800 |
| 6b | Add evaluation before rollout | How-to | **Optional** | ~1200 |
| — | Troubleshooting appendix | Reference | Optional | ~800 |

Total required path: ~7000 words / ~60 min. Full path with optionals: ~10,600 words / ~90 min.

---

## Per-part detail

### Part 1 — Overview: MAF agents in M365 Copilot (Concept, ~1200 words)

- What you're going to build (diagram: MAF agent → Foundry Hosted Agent →
  Activity protocol / Microsoft 365 Agents SDK → M365 Copilot chat).
- Why this stack: MAF for authoring, Foundry Agent Service for hosting, M365 Agents Toolkit for the Copilot surface.
- **Prereqs and the tenant/license gate** (repeated from the top, in-line here for people who deep-link).
- What's optional vs. required in this series and why.
- Where this fits in the bigger .NET Foundry Learn series (link).
- What's *out of scope* (Teams-specific bots, Copilot Studio authoring flow, multi-agent orchestration, non-Copilot end-user surfaces).

### Part 2 — Build a MAF agent locally against a Foundry model (Quickstart, ~1000 words)

- `dotnet new` + MAF NuGet packages.
- Configure a Foundry model endpoint + key/entra auth.
- Minimal agent: system prompt + one tool (e.g. `LookupPolicy(topic)` stub).
- Run and chat with it from a console test host.
- Create one MAF `AgentSession` and reuse it across turns; explain that this
  provides conversation history, not durable memory across restarts.
- **Verify:** you can send a prompt and get a response that calls the tool.

**Callout inside Part 2 — "Iterate offline with Foundry Local" (~300 words, optional):**

- One-line: swap the endpoint to a Foundry Local model for faster inner loops or air-gapped dev.
- Cost: no cloud spend during iteration; caveat: you'll swap back before Part 4.
- Link out to the Foundry Local install docs; don't re-teach setup here.
- Not a full section — this is a "did you know" sidebar so readers don't feel forced into cloud from minute one.

### Part 3 — Add file-based grounding; introduce Foundry IQ *(Optional, ~1500 words)*

**Why optional:**
- The Part 2 → Part 4 → Part 5 path already produces a working M365 Copilot agent — that's the "hello world" win.
- Grounding adds real setup friction (Foundry IQ provisioning + data source connect, or index build).
- M365 Copilot already grounds against Microsoft Graph; some readers may prefer that + MAF for reasoning/actions only.
- Skipping this part means the agent answers from model + tools only; still useful for "reasoning + action-triggering" scenarios.

**When to do this part:**
- Your scenario needs the agent to answer from documents Copilot can't see (SharePoint sites outside its scope, third-party knowledge bases, internal wikis, etc.).
- You want the demo to feel like a real knowledge assistant, not a chat wrapper.

**Content:**
- Walkthrough path: transparent Markdown/file retrieval exposed as a MAF tool.
- Production upgrade: Foundry IQ + a connected enterprise knowledge source.
- How the MAF agent wires the grounding source (tool vs. retriever).
- **Verify:** ask a question that only your grounded content can answer; confirm the source is cited.

### Part 4 — Deploy as a Foundry Hosted Agent (How-to, ~1500 words)

- Use `azd provision` and direct-code `azd deploy` to deploy the MAF agent as
  a Foundry Hosted Agent.
- Set the model deployment name explicitly in the hosted-agent environment;
  do not assume the model declaration creates a same-named azd variable.
- Start with enough model capacity for multi-tool responses and evaluation;
  the sample required 50 GlobalStandard capacity units rather than 10.
- Identity + endpoint auth (managed identity recommended).
- Confirm the deployed endpoint responds to a raw HTTP/SDK call from a test client.
- **Verify:** you can hit the hosted endpoint and get the same behavior as Part 2.

> **Sidebar: when to self-host the agent instead.** Hosted Agent is the right pick for this walkthrough because it gives us a cloud-reachable endpoint with managed identity, built-in Foundry observability, and a clean org-publish story — everything Part 5 and Part 6 assume. Self-host the MAF agent on Container Apps / App Service / AKS / your own runtime instead when you have compliance or data-residency requirements the hosted runtime doesn't satisfy, need to run inside an existing service topology or private network, want to reuse compute you already pay for, need multi-cloud / hybrid / edge portability, or your team already owns the CI/CD and observability plane. For the full topology-choice discussion (hosting axis and end-user-surface axis), see the bigger series' Part 6.5.

### Part 5 — Register the custom engine agent with M365 Copilot (Tutorial, ~2500 words) **← the heart of the walkthrough**

- Explain the decision: MAF + Foundry bring their own orchestrator and model,
  so this is a **custom engine agent**, not a declarative agent action.
- Use the Microsoft 365 Agents SDK / Activity protocol and an app manifest
  with `copilotAgents.customEngineAgents`.
- Wire Azure Bot Service/app registration and the supported identity flow.
- Local debug: sideload the app to a dev tenant and chat with it in M365 Copilot.
- **Verify:** you can @-mention the agent in M365 Copilot and get a response that came from your MAF agent.
- Troubleshooting: the top 3–5 failures readers actually hit (manifest schema, sideload permissions, endpoint auth, CORS/allowed hosts).

### Part 6a — Publish org-wide (How-to, ~800 words)

- From dev-tenant sideload to org-wide publish.
- Admin approval flow, availability policies, discoverability inside Copilot.
- Basic runtime health/monitoring pointers (link to Foundry observability).

### Part 6b — Add evaluation before rollout *(Optional, ~1200 words)*

**Why optional but strongly recommended:**
- You're shipping to end users inside their productivity app — quality matters more, not less.
- But eval setup is a distinct skill and can be skipped for a first demo or internal-only pilot.

Structured as **local → cloud → gate** — progressively more valuable, all optional.

**Step 1 — Local checks (fast, no cloud).** Use `LocalEvaluator` from [MAF agents — Evaluation (C# pivot)](https://learn.microsoft.com/en-us/agent-framework/agents/evaluation) with built-in `EvalChecks.KeywordCheck` / `EvalChecks.ToolCalledCheck` and `FunctionEvaluator.Create()` for custom checks. Ideal for inner-loop dev and CI smoke tests. For richer assertions (quality, safety, NLP, reporting), point at [The Microsoft.Extensions.AI.Evaluation libraries](https://learn.microsoft.com/en-us/dotnet/ai/evaluation/libraries) — `.Quality`, `.Safety`, `.NLP`, `.Reporting(.Azure)`.

**Step 2 — Cloud runs that show up in the Foundry portal.** Use
`FoundryEvals` from the separate preview `Microsoft.Agents.AI.Foundry`
package. The verified baseline uses explicit relevance and coherence
evaluators, while local checks assert tool use and credential safety. The
current preview default that includes task adherence can fail for tool agents
because its request omits the required `tool_definitions` mapping. Runs execute
against Foundry and results land in the portal. Reference samples to steal from:
- [`Evaluation_FoundryQuality`](https://github.com/microsoft/agent-framework/tree/main/dotnet/samples/05-end-to-end/Evaluation/Evaluation_FoundryQuality) — built-in quality evaluators in C#.
- [`Evaluation_FoundryRubric`](https://github.com/microsoft/agent-framework/tree/main/dotnet/samples/05-end-to-end/Evaluation/Evaluation_FoundryRubric) — custom rubric evaluators authored in the portal, referenced by name/version.

Underlying evaluator reference (what each one measures): [Foundry agent evaluators](https://learn.microsoft.com/en-us/azure/foundry/concepts/evaluation-evaluators/agent-evaluators) — intent resolution, tool-call accuracy, task adherence, plus general quality/safety.

**Viewing results in the Foundry portal** — 5 concrete steps to include in the walkthrough (source: [View evaluation results in the Foundry portal](https://learn.microsoft.com/en-us/azure/foundry/how-to/evaluate-results)):

1. Open your Foundry project → **Evaluation** in the left pane.
2. Pick the run you just kicked off. Landing page shows name, target (your hosted agent), dataset, status, tokens consumed, and aggregate scores per evaluator.
3. Click the run name for row-level detail: query, agent response, ground truth (optional), per-evaluator score + reasoning.
4. Select 2+ runs → **Compare** for a side-by-side view (portal uses statistical t-testing, so run-to-run diffs are meaningful).
5. Screenshot the aggregate scores view — good asset for the walkthrough.

**Step 3 — Set a gate before org rollout (ties back to Part 6a).** Start with
the verified baseline: local tool-call and credential-safety checks plus cloud
relevance, coherence, and tool-call accuracy. Add task adherence and
groundedness after the preview task-adherence mapping issue is resolved or a
custom evaluator is supplied. Require the selected gate to pass before
flipping org-wide availability.

- Give a minimal "run these 3 evaluators against your grounded agent" recipe using the running scenario (groundedness + task adherence + safety), taken directly from `Evaluation_FoundryQuality`.
- Don't re-teach eval concepts — the linked docs are the eval hub for this walkthrough.
- **Verify:** you can see the run in the Foundry portal, the aggregate scores clear your gate, and a Compare view against a prior run shows no regression.

### Appendix — Troubleshooting *(Optional, ~800 words)*

- Sideload denied / license missing.
- Custom engine app-manifest / bot registration errors.
- Hosted agent 401/403 from the Copilot action.
- Grounding returns no citations.
- Foundry Local endpoint mismatch after switching back to cloud.

---

## Scope cuts (call out explicitly on the overview page)

- Teams-specific bot customization → link to Teams platform docs.
- Copilot Studio authoring / Copilot Studio → Foundry integration → link to Copilot Studio docs; note as an alternative path.
- Multi-agent orchestration (Magentic, group chat, sequential) → link to bigger series' multi-agent part.
- Non-Copilot surfaces (web chat, HTTP-only, custom UI) → link to bigger series' Part 6.5.
- Semantic Kernel → migration content lives elsewhere; don't mention SK in this walkthrough at all.
- Aspire → single callout in Part 4 only, for larger topologies.

## Runnable sample strategy — new repo

Recommendation: **create a new dedicated sample repo alongside the walkthrough.** Do not extend the existing starter pack.

Rationale:
- The M365 Copilot slice has a distinct shape: Activity protocol, Microsoft
  365 custom-engine app manifest, bot registration, and dev-tenant sideload
  flow. None of that belongs in a generic hosted-agent starter.
- The audiences differ: the starter pack targets .NET agentic devs broadly; this repo targets .NET devs already invested in the M365 ecosystem.
- Mixing them would either force starter-pack users to see Copilot-specific artifacts they don't need, or force this walkthrough into starter-pack conventions (Aspire-first) that we've explicitly moved away from.

**Proposed repo:** `dotnet-maf-foundry-m365copilot-sample` (or similar; final name TBD with DevRel).

**Suggested branches / tags aligned to the walkthrough:**
- `part-2-local-agent` — MAF agent + Foundry model, no grounding, no deploy
- `part-3-grounded` — adds file-based grounding; points to Foundry IQ as an upgrade
- `part-4-hosted` — deployed as Foundry Hosted Agent
- `part-5-copilot` — full custom engine agent + M365 Copilot registration
- `main` — final state (`part-5-copilot` + optional Part 6b eval scaffolding)

**Keep cross-links to** the starter pack (`Azure/microsoft-agent-framework-foundry-starter-pack-net`) as the "generic hosted-agent anchor" and the workshop repo (`Azure-Samples/multi-agent-orchestration-workshop`) as the "multi-agent orchestration deep dive."

## Discoverability

- Land under `/dotnet/ai/` with strong cross-links from `/agent-framework/`, `/azure/foundry/`, and `/microsoft-365-copilot/extensibility/`.
- `aka.ms` shortlink, e.g. `aka.ms/dotnet-agent-in-m365copilot`.
- Add to the .NET agentic Learning Path (once created — see bigger series discoverability plan).
- Cross-link from the bigger series' Part 6.5 (end-user surfaces) as the deep-dive for the M365 Copilot surface.

---

## Open questions to confirm before drafting

1. **Activity adapter.** What is the current supported .NET adapter and
   generated setup flow for exposing a Foundry Hosted Agent over the Activity
   protocol? (Owner: M365 Copilot Extensibility + Foundry.)
2. **Auth model.** Confirm the production identity flow across Microsoft 365,
   Azure Bot Service, Activity protocol, and the Hosted Agent.
3. **Foundry IQ follow-up.** Which existing Foundry IQ article should the
   file-based grounding page link to as the production upgrade?
4. **Tenant/license minimums.** What is the *minimum* combination of M365 Copilot license + admin role needed to complete the walkthrough end-to-end? This must be exact in the prereqs.
5. **Sample repo strategy.** Confirmed direction is a new dedicated repo — needs owner assignment, naming approval, and repo home (Azure vs. Azure-Samples vs. dotnet). Coordinate with DevRel and .NET docs.
6. **Foundry Local placement.** Should the offline-iteration callout in Part 2 be an inline sidebar, its own optional appendix page, or dropped entirely? Depends on how much friction the endpoint-swap adds in practice.
7. **Eval scoping.** Which 2–3 evaluators are the right "minimum viable eval baseline" for an M365 Copilot–surfaced agent — groundedness + relevance + safety, or a different set?

---

## Bottom line

- Ship this narrower walkthrough first as the MVP proof-point for the .NET → Foundry → M365 Copilot path.
- Required path is 6 pages / ~7000 words / ~60 min.
- Grounding, Foundry Local, and Foundry eval are optional layers so readers get the "hello world" win before investing in setup. File-based grounding is the runnable path; Foundry IQ is the next-step callout.
- Ship a **new dedicated sample repo** alongside; do not overload the existing starter pack.
- Once this ships and we've validated the pattern, invest in the full 8-part series with the running product-support-triage scenario.
