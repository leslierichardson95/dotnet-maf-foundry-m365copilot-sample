# .NET Agentic Apps on Foundry — End-to-End Learn Series Outline

**Owner:** Leslie Richardson · **Date:** 2026-08-12
**Status:** Draft outline for Foundry Docs team discussion

## Goals

- Give .NET developers **one authoritative walkthrough** that takes them from an empty project to a production-hosted agent on Foundry
- Anchor the story on **three Foundry tools** — Foundry SDK, Foundry Local, Foundry Agent Service — and reference other Foundry surfaces only where they naturally slot in
- Use **Microsoft Agent Framework (MAF)** as the framework (successor to Semantic Kernel)
- **No Aspire in the default path.** Any deployment/orchestration step uses the recommended core-stack option; Aspire appears only as a callout for devs already invested in it
- Solve the **discovery problem** by making this series the linear entry point that ties together the currently fragmented .NET agentic content on Learn

## Design principles

1. **Progressive complexity** — each part adds one new concern (tools → grounding → eval → observability → deploy → operate). No part introduces two new things at once.
2. **Every part is independently useful** — a dev landing on Part 4 (evaluation) shouldn't need to have done Parts 1–3.
3. **One running scenario across the series** — the app evolves; devs aren't context-switching between mini-apps.
4. **Runnable at every step** — each part ends with a working `dotnet run`, either against Foundry Local or a Foundry model.
5. **Foundry surface map baked in** — a "which Foundry tool does what" callout appears in Part 0 and is linked from every subsequent part.

## Recommended running scenario

**A product-support triage assistant** for a fictional company (e.g., a small e-commerce shop). Reasons:

- Simple enough to introduce in Part 1 without heavy domain setup
- Naturally justifies tool calls (order lookup, refund policy), grounding (product catalog), multi-turn conversation, and eval (correctness on canned support scenarios)
- Maps cleanly onto DevRel's *"customer support triage agent"* Agent Factory Kit example, so the content can flow into that program
- Adjacent to enterprise use cases without being industry-specific

---

## Proposed series structure

Learn doc types: **[C]** concept · **[Q]** quickstart · **[H]** how-to · **[T]** tutorial · **[R]** reference

| # | Title | Type | ~Length | Foundry tools featured |
|---|---|---|---|---|
| 0 | The .NET agentic app on Foundry — overview & Foundry surface map | C | ~1500 words | All (map) |
| 1 | Quickstart: your first .NET agent with MAF + Foundry SDK | Q | ~800 words | Foundry SDK, Foundry Models |
| 2 | Run your agent offline with Foundry Local | H | ~1200 words | Foundry Local, Foundry SDK |
| 3 | Add tools and grounding to your MAF agent | T | ~2000 words | Foundry SDK, Foundry IQ (optional) |
| 3.5 | *(Optional)* Attach an MCP server to your agent | H | ~1200 words | MAF MCP client, Foundry Toolkit (local), Container Apps / hosted MCP |
| 4 | Evaluate your agent's quality and safety | T | ~1800 words | M.E.AI.Evaluation, MAF evaluators, Foundry evaluators |
| 5 | Add observability with OpenTelemetry and Foundry tracing | T | ~1500 words | Foundry observability / tracing |
| 6 | Deploy as a Foundry Hosted Agent on Foundry Agent Service | T | ~2500 words | Foundry Agent Service, Hosted Agents, Foundry Toolkit |
| 6.5 | *(Optional)* Other publishing options — self-host and end-user surfaces | C+H | ~1500 words | Foundry SDK (client), Container Apps, App Service, Teams, M365 Copilot, Copilot Studio |
| 7 | Operate at scale — fleet, evals, tracing in production | H | ~1500 words | Fleet management, Foundry portal, Foundry observability |
| 8 | (Optional) Multi-agent orchestration patterns | T | Learning-path link | MAF workflows |

Roughly **~1 landing + 8 pages**. Compact enough for a single Learning Path; each page independently useful.

---

## Part-by-part detail

### Part 0 — Overview & Foundry surface map (concept)

**User question answered:** *"What is the recommended .NET agentic path on Foundry, and which Foundry tool does what?"*

**Content:**
- The recommended stack in one sentence: **MAF (framework) + `Microsoft.Extensions.AI` (primitives) + Foundry SDK / Foundry Local (models & runtime) + Foundry Agent Service (hosting)**
- **Foundry surface map** — one-screen visual showing every major Foundry surface (Foundry Models, Foundry Agent Service / Hosted Agents, Foundry Local, Foundry SDK, Foundry Toolkit, Foundry IQ, Foundry observability, Foundry Portal, Foundry Control Plane) with a one-line description of what each one does and when a .NET dev would use it
- **When to use what** decision panel — the compact version of the standalone decision guide
- **Model deployment vs. agent deployment on Foundry** — a dedicated callout panel that names them as two distinct operations:
  - *Deploying a model* = provisioning a hosted inference endpoint for a specific model (GPT-4o, Phi, etc.). This is what your app or agent **calls**. One-time infra step per model/region.
  - *Deploying an agent (Hosted Agent)* = packaging your MAF agent code + config + identity as a Foundry-managed runtime that scales, has an identity, and is itself invocable as an endpoint. This is what your app or other agents **invoke**.
  - **Do both** when you want a hosted agent running in the cloud that calls a hosted model.
  - **Model-only** when your .NET app hosts its own agent code (Container Apps, App Service, on-prem) and just needs a cloud model to call.
  - **Agent-only** when Foundry can source the model for your agent (routed / catalog-selected) without a per-project deployment. *(Confirm current shipping story with Foundry Docs before this ships — see open question #7.)*
  - **Neither** when running fully against Foundry Local (Part 2 territory).
- Callouts:
  - *"Migrating from Semantic Kernel?"* → SK-to-MAF migration guide
  - *"Already using .NET Aspire?"* → Aspire integration doc (kept as a deployment option, not the default)
  - *"Building on GitHub Copilot SDK?"* → Copilot SDK path

**Reuses:** existing MAF overview, Foundry portal GA overview, `Microsoft.Extensions.AI` overview

---

### Part 1 — Quickstart: your first .NET agent (quickstart)

**User question answered:** *"How do I get a MAF agent running against a Foundry model in five minutes?"*

**Content:**
- Prereqs: .NET 10 SDK, an Azure sub with a Foundry project + one deployed model
- **Sidebar:** *"Why do I need to deploy a model?"* — one-paragraph explanation that a model deployment is a hosted inference endpoint your code calls; this is infra, not agent code. Link to Part 0's model-vs-agent-deployment callout and to the Foundry Models catalog doc.
- `dotnet new console` → add `Microsoft.Agents.AI` + Foundry SDK client packages
- ~20 lines of C# that instantiates a `ChatClientAgent`, wires it to the Foundry model via Foundry SDK, prints one response
- Add a short multi-turn step that creates one `AgentSession`, reuses it across
  turns, and distinguishes conversation history from durable memory.
- Explanation of what each piece did
- "Next step" → Part 2 (run offline) or Part 3 (add tools)

**Foundry tools featured:** Foundry SDK, Foundry Models catalog

**Reuses:** existing MAF C# quickstart content; existing Foundry SDK auth guidance

---

### Part 2 — Run offline with Foundry Local (how-to)

**User question answered:** *"How do I run my agent against a local model without changing the code?"*

**Content:**
- Install Foundry Local, pick a model
- Change one line: swap the Foundry SDK endpoint for Foundry Local's local endpoint (same OpenAI-compatible surface)
- Verify hardware/perf caveats (Justin Yoo's July concerns become an honest "what to expect" section here)
- When to prefer Foundry Local vs. cloud: dev-loop, cost, offline demos, privacy
- Not for: production traffic (yet)

**Foundry tools featured:** Foundry Local, Foundry SDK (same client)

**Timing note:** if Foundry Local's .NET story stabilizes on a specific timeline, this doc may need coordination with Lee Stott's team

---

### Part 3 — Add tools and grounding (tutorial)

**User question answered:** *"How do I let my agent do things and reference real data?"*

**Content:**
- Define MAF tool functions in C# (order lookup, refund policy)
- Add them to the agent, watch them get called
- Grounding: two options
  - Simple — provide a product catalog file as context
  - Foundry IQ — connect the agent to a Foundry-managed knowledge source (optional callout)
- Talk about tool schema, when the model chooses to call, error handling

**Foundry tools featured:** Foundry SDK; **Foundry IQ** callout

**Reuses:** existing MAF tools/functions doc; Foundry IQ "connect from an agent" doc

---

### Part 3.5 (optional) — Attach an MCP server to your agent (how-to)

**User question answered:** *"How do I let my agent use tools from an MCP server — either one I run locally or one hosted somewhere?"*

**Content:**
- Why MCP: reusable tool surfaces across agents, apps, and IDEs; separation between "who provides the tool" and "who calls it"
- The MAF MCP client story in .NET — connect an agent to an MCP server as a tool source, and it appears alongside native tool functions
- **Two flavors of MCP server, one page:**
  - **Local stdio MCP server** — for dev-loop; wire it in, run it as a child process, exercise it from the support-triage sample
  - **Remote / hosted MCP server** — HTTP/SSE endpoint; auth considerations; when to prefer over local
- **Deploying a companion MCP server on Foundry / Azure:**
  - Foundry Toolkit hosted-agent workflow can co-deploy an MCP server alongside your agent (this is what the starter pack demonstrates)
  - Or deploy the MCP server independently to Azure Container Apps and reference it by URL — the more general pattern
- Auth: managed identity between your agent and a hosted MCP server; secret-less scopes
- Debug: how to see MCP tool calls in the traces you set up in Part 5

**Foundry tools featured:** Foundry Toolkit (for the co-deploy path), Foundry Agent Service (as the caller); the MCP server itself may live on Container Apps or another host

**Reuses:** MAF MCP integration docs (verify exact page name with Foundry Docs team); the starter pack's `resources-mcp` folder as the runnable "hosted MCP alongside hosted agent" reference

**Caveat to raise with Foundry Docs team:** confirm whether Foundry Agent Service natively hosts MCP servers today, or whether "companion MCP server" always means "separate Azure resource." That decision determines whether the page shows one deploy flow or two.

**Placement rationale:** kept optional and after Part 3 (tools) so readers who don't need MCP can skip straight to eval; readers who do need it have the tool foundation from Part 3 to compare against.

---

### Part 4 — Evaluate quality and safety (tutorial)

**User question answered:** *"How do I know if my agent is any good — and stays good?"*

**Content:**
- Local eval loop with `Microsoft.Extensions.AI.Evaluation` — build a small test set of expected support scenarios, run quality evaluators
- MAF evaluators (`IAgentEvaluator`, `LocalEvaluator`) for agent-specific behavior
- `FoundryEvals` from the separate preview `Microsoft.Agents.AI.Foundry`
  package; select evaluators explicitly for tool agents until the default task
  adherence mapping issue is resolved
- Safety evaluators — hate/violence/etc. via Foundry-hosted evaluators
- CI recipe: run eval as part of `dotnet test`
- When to use local eval vs. Foundry cloud eval

**Foundry tools featured:** Foundry evaluators (agent evaluators, risk-safety evaluators, built-in evaluators)

**Reuses:** existing MAF evaluation (C#) doc; `Microsoft.Extensions.AI.Evaluation` libraries page; Foundry agent evaluators concepts

**Discovery win:** this page becomes the canonical "how to evaluate a .NET agent" hub, linking to the four scattered existing pages

---

### Part 5 — Add observability (tutorial)

**User question answered:** *"How do I trace what my agent did in production?"*

**Content:**
- Add OpenTelemetry to the .NET app
- Client-side tracing to Application Insights via the Azure Monitor exporter
- Reading traces in Foundry Portal's observability view
- Sensible sample rates, PII considerations

**Foundry tools featured:** Foundry observability / tracing

**Reuses:** existing Foundry client-side tracing (.NET) doc; Foundry hosted-agent tracing quickstart; Foundry observability concept

**Discovery win:** this page ties Foundry tracing + Foundry observability + Application Insights into one story a .NET dev can follow (matches the discovery bullet from earlier)

---

### Part 6 — Deploy as a Foundry Hosted Agent (tutorial)

**User question answered:** *"How do I take my working local agent and host it on Foundry Agent Service?"*

**Content:**
- **Framing (opens the page):** *"In Part 1 you deployed the model your agent calls. Now we're deploying the agent itself — a different operation on a different Foundry surface. See the model-vs-agent-deployment callout in Part 0 if you want the recap."*
- The Hosted Agents concept — deployment, scaling, identity, managed runtime
- Two deploy paths:
  - **Foundry Toolkit for VS Code** (recommended default) — the "click deploy" flow
  - **CLI / declarative** — `azure.yaml`, `azd provision`, and direct-code
    `azd deploy` for CI/CD
- Set the hosted model deployment name explicitly and size model capacity for
  multi-tool responses plus evaluation traffic
- Wire up managed identity for the agent's model + tool auth
- Verify it runs from the Foundry Portal
- Callout: *"If you use .NET Aspire"* → link to the Aspire-based path (starter pack repo) as an alternative

**Foundry tools featured:** Foundry Agent Service, Hosted Agents, Foundry Toolkit

**Reuses:** existing "What are hosted agents?" doc; "What is Foundry Agent Service?" doc; Foundry Toolkit VS Code hosted-agent workflow docs (both csharp and python variants exist today)

---

### Part 6.5 (optional) — Other publishing options (concept + how-to)

**User question answered:** *"What if I don't want Foundry Agent Service to host my agent, and how do end users actually reach my agent once it's published?"*

**"Publishing" covers two independent axes** — this page names them and shows the options for each.

**Axis 1 — Hosting topology** (where the agent code runs)

| Option | When to pick | Notes |
|---|---|---|
| **Foundry Hosted Agent** (Part 6) | Default. Enterprise, managed identity, Foundry-scale fleet | Recommended in the DevRel strategy |
| **Self-host on Azure Container Apps** | Your team already runs ACA; you want more infra control | Use Foundry SDK from inside the container; agent is invocable via your own HTTP surface |
| **Self-host on App Service / AKS / VM** | Existing platform commitments | Same pattern as ACA; call Foundry model from your code |
| **.NET Aspire orchestration** | Your team is Aspire-invested | See starter pack; `PublishAsHostedAgent` targets Foundry, other resources deploy elsewhere |
| **Foundry Local** | Dev-loop, offline demos, privacy-sensitive prototypes | Not for production (see Part 2 caveats) |

**Axis 2 — End-user surface** (how humans and other systems reach the agent)

| Surface | When to pick | Notes |
|---|---|---|
| **Direct HTTP/SDK invocation** | Server-to-server, backends, tests | The invocation model Part 6 leaves you with |
| **Custom web chat UI** | You own the front-end experience | `Microsoft.Extensions.AI` chat primitives; wire the agent as the chat backend |
| **Microsoft Teams** | Enterprise chat surface | Teams AI Library + Foundry Agent Service; managed identity for the connection |
| **Microsoft 365 Copilot** | Bring a MAF + Foundry orchestrator into Copilot | Publish as a custom engine agent over the Activity protocol |
| **Copilot Studio** | No-code/low-code teams build the surface, you own the agent | Copilot Studio can reference a Foundry Hosted Agent as a skill/tool |
| **API-first (Foundry SDK from external clients)** | Third-party developers integrating your agent | Rate limits, auth, and versioning become first-class concerns |

**Content:**
- Decision panel (~1 paragraph per axis) with links to option-specific how-tos
- One end-to-end example the reader can actually run: publish the support-triage agent to a minimal ASP.NET Core web front-end (proves Axis 2 without requiring Teams/M365 tenant setup)
- Callout with links to Teams / M365 Copilot / Copilot Studio-specific docs for readers who need those surfaces
- **Cost & compliance callouts** — self-hosting shifts operational cost and compliance burden onto the reader's team; Hosted Agent absorbs it

**Foundry tools featured:** Foundry SDK (client-side, from any host), Foundry Agent Service (as one option among many)

**Non-goals:** deep-dive on Teams / M365 / Copilot Studio publishing — those have their own doc families; this page routes to them

**Placement rationale:** kept optional and *after* Part 6 so readers who take the golden path can skip it, but readers who need alternatives find them right where they'd look ("what else can I do with this deployed agent?").

---

### Part 7 — Operate at scale (how-to)

**User question answered:** *"My agent is deployed — now what?"*

**Content:**
- Foundry Portal fleet view — health, compliance, performance across projects
- Continuous eval — schedule evaluators against production traffic
- Trace correlation — from a Foundry Portal alert back to a specific span in Application Insights
- Governance basics — Control Plane RBAC for who can update the agent
- Notification Center — set up alerts

**Foundry tools featured:** Fleet management, Foundry portal, Foundry Control Plane, Foundry observability, Notification Center

**Reuses:** existing "Monitor agent health and performance across your fleet" doc; "Manage agents at scale in Foundry Control Plane" doc; Portal GA overview

---

### Part 8 (optional) — Multi-agent orchestration (tutorial / learning-path link)

**User question answered:** *"How do I coordinate multiple agents?"*

**Content:**
- Point at the existing `Azure-Samples/multi-agent-orchestration-workshop` and the four MAF orchestration patterns (Sequential, Concurrent, Handoff, Group Chat)
- Note the Magentic pattern is coming to .NET later
- **Caveat to raise with Foundry Docs team:** workshop is Aspire-first. Either (a) build a non-Aspire variant, or (b) mark Aspire as one deployment style with a small callout, or (c) defer this part until the workshop is de-Aspire'd

**Foundry tools featured:** MAF workflows (framework-side)

---

## What this series *doesn't* cover (intentional scope cuts)

- Business decision-maker content — this is dev docs
- Comparison to non-Microsoft frameworks (LangChain, LlamaIndex) — those go in the standalone decision guide
- Model fine-tuning / custom models — separate series
- Aspire-first deployment — kept as a callout / alternative path in Parts 6 and 8
- Copilot SDK path — separate content family
- Enterprise networking / private endpoints — link out to Foundry Control Plane docs

## Discoverability plan

- Land the series under `learn.microsoft.com/dotnet/ai/agents/` (new top-level `/dotnet/ai/agents/` node in the TOC)
- Cross-link from existing `/agent-framework/` C# pages (MAF observability, MAF eval) → Parts 4 and 5
- aka.ms shortlinks: `aka.ms/dotnet-foundry-agent-quickstart`, `aka.ms/dotnet-foundry-agent-eval`, `aka.ms/dotnet-foundry-agent-deploy`
- Package as a **Microsoft Learn Learning Path** (Parts 0–7) so devs get a linear guided experience
- The Foundry surface map from Part 0 gets its own aka.ms link (`aka.ms/foundry-map`) so DevRel can share it independently

## Runnable sample strategy

- Build **one canonical support-triage sample repo** that evolves across Parts 1–7, each part = a branch or a folder
- Non-Aspire from the ground up (uses plain hosted service or minimal API where needed)
- Owned by whoever the Foundry Docs team designates (avoid the "solo owner" problem of the current starter-pack + workshop)
- Existing starter pack (`microsoft-agent-framework-foundry-starter-pack-net`) is referenced from Part 6 as "the Aspire-based alternative"
- Existing workshop referenced from Part 8

## Open questions to bring to the Foundry Docs team meeting

1. **TOC home:** does this live under `/dotnet/ai/` (Microsoft.Extensions.AI's current hub), `/agent-framework/` (MAF's current hub), or a new consolidated `/foundry/dotnet/`? Root-cause fix for discovery is probably the third option.
2. **Foundry Local timing:** is Part 2 blocked by Foundry Local's .NET readiness, or is it OK to ship documenting a known-preview experience?
3. **Sample repo ownership:** who commits to maintaining the canonical support-triage sample across MAF releases (monthly)?
4. **Aspire posture:** confirm "Aspire is a callout, not the default" is aligned with Amy Boyd's DevRel strategy and Amanda Silver's simplification direction.
5. **Magentic and Part 8:** ship the series with Part 8 deferred, or wait for .NET Magentic support so the workshop reference is complete?
6. **Foundry surface map:** does a standalone "surface map" page already exist somewhere on Learn we should link to rather than write? (I couldn't find one in Learn search — worth confirming with the docs team.)
7. **Model-vs-agent-deployment matrix:** the "agent-only when Foundry can source the model without a per-project deployment" scenario (Model Router / catalog-selected models) — confirm the current shipping story with the Foundry Docs team before Part 0's callout and Part 6 ship, since this has been shifting.
8. **MCP hosting story (Part 3.5):** does Foundry Agent Service natively host MCP servers today, or is "companion MCP server" always a separate Azure resource (typically Container Apps)? Determines whether Part 3.5 shows one deploy flow or two.
9. **Scope of Part 6.5:** confirm which end-user surfaces (Teams / M365 Copilot / Copilot Studio) belong in the series vs. get routed to owner docs. Also confirm the recommended Copilot Studio → Foundry Hosted Agent integration story, which has been shifting.

## Bottom line

Eight pages, one running scenario, three anchor Foundry tools, no Aspire in the default path, and every existing scattered piece of .NET agentic content gets a home in the series. This is the artifact under Amy Boyd's "One Golden Path" content bet and directly addresses the discovery + fragmentation problems in the Bill Wagner summary.
