# Hiccups and Doc Opportunities

Running log of friction points hit while building this sample. Each entry is a candidate for a Microsoft Learn doc issue.

**Entry format**

```
### [Part X] Short title
**What tripped me up:** …
**Root cause / current state:** …
**Doc opportunity:** what doc, what topic, priority (P0/P1/P2)
**Suggested owner:** MAF / Foundry Docs / M365 Copilot Ext / DevRel
```

---

## Seed topics we expect to hit (to fill in during build)

- [ ] **[Setup]** .NET 10 preview + MAF/M.E.AI package compatibility — exact pins that work today
- [x] **[Setup]** `azd` CLI + extension version drift on Windows
- [x] **[Part 2]** Which MAF NuGet packages are the *minimum* for a console agent + tools?
- [x] **[Part 2]** MAF + Foundry SDK model client wiring in .NET (what's the recommended constructor / factory pattern?)
- [ ] **[Part 2]** `DefaultAzureCredential` behavior against Foundry endpoints — which auth chain step actually wins?
- [ ] **[Part 3]** File-based grounding via `Microsoft.Extensions.AI` retrieval — is there a canonical pattern in .NET?
- [x] **[Part 4]** Bicep API version + resource shape for a Foundry Hosted Agent
- [x] **[Part 4]** Managed identity role assignments: Hosted Agent → Foundry model
- [x] **[Part 4]** Region availability: model + Hosted Agent verified in `northcentralus`
- [x] **[Part 5]** Agent type decision: MAF + Foundry maps to a custom engine agent, not a declarative-agent action
- [x] **[Part 6b]** `FoundryEvals` .NET availability + portal-visible run
- [x] **[Part 6b]** Agent evaluation run and row-level result path in the Foundry portal

---

## Verified friction (populated during build)

_Entries added below as we hit them, most recent first._

### [Part 2] Omitting `AgentSession` makes every console turn stateless

**What tripped me up:** The first console loop called `agent.RunAsync(input)`
on every turn. The agent repeatedly asked for the user's email because each
call created an independent conversation.

**Root cause / current state:** MAF conversation history is associated with an
`AgentSession`. The console now creates one session, passes it to every
`RunAsync` call, and exposes `/reset` to start a new conversation. Session
history lasts only for that process; durable cross-session memory is a
separate design choice.

**Doc opportunity:** The first multi-turn MAF sample should show
`CreateSessionAsync` and explain session history versus durable memory.
**Priority: P0.**

**Suggested owner:** MAF Docs.

### [Setup] `azd` login can fail by probing unrelated guest tenants

**What tripped me up:** Browser authentication succeeded, but
`azd provision --preview` immediately reported reauthentication. Debug logs
showed `azd` requesting tokens for unrelated tenants where the guest account
was disabled or Conditional Access blocked Azure CLI. Device-code auth was
also blocked with error 530033.

**Root cause / current state:** The user belongs to many tenants, and `azd`
enumerated them during subscription lookup. Setting `AZURE_TENANT_ID` to the
subscription's home tenant constrained subsequent Foundry commands and
resolved the loop.

**Doc opportunity:** Add a multi-tenant troubleshooting section that shows
tenant-scoped browser login plus `azd env set AZURE_TENANT_ID ...`; do not
recommend device code where managed-device Conditional Access is enabled.
**Priority: P0.**

**Suggested owner:** Azure Developer CLI Docs / Foundry CLI.

### [Part 6b] Default `FoundryEvals` fails task-adherence validation for tool agents

**What tripped me up:** The local MAF checks passed, but the default cloud
`FoundryEvals` run failed with `EvalValidationFailed`. The service reported
that `builtin.task_adherence` required a `tool_definitions` data mapping that
the generated request omitted.

**Root cause / current state:** The current preview integration defaults to
relevance, coherence, and task adherence. For this tool-enabled agent, the
preview package generated an invalid task-adherence criterion. The sample now
uses relevance and coherence for the cloud judge while retaining explicit
local assertions for tool selection and safety.

**Doc opportunity:** Publish a .NET tool-agent example with explicit
evaluators and document which evaluator/data mappings the preview package
currently supports. **Priority: P0** because the documented default
constructor produces a server-side validation failure.

**Suggested owner:** MAF Docs / Foundry Evaluation SDK.

### [Part 6b] Negative phrase matching can fail a safe refusal

**What tripped me up:** The first credential-safety check rejected a correct
answer because it searched for `paste your MFA code`; the response safely said
`do not paste your MFA code`.

**Root cause / current state:** A substring check could not distinguish an
unsafe request from a refusal quoting the same phrase. The check now asserts
the expected refusal signals (`do not` plus `MFA code`), and the evaluation
runner returns a nonzero process exit code when any check fails.

**Doc opportunity:** Safety assertions should test the intended behavior, not
just words that may also appear inside a refusal. **Priority: P1.**

**Suggested owner:** MAF Evaluation Docs.

### [Part 4] Model environment output is not inferred from the deployment declaration

**What tripped me up:** The first Hosted Agent version deployed successfully,
but its `AZURE_AI_MODEL_DEPLOYMENT_NAME` environment variable was empty.
`azd provision` emitted the model list as `AI_PROJECT_DEPLOYMENTS`, but did not
emit the individual variable referenced by the service `env` block.

**Root cause / current state:** `${AZURE_AI_MODEL_DEPLOYMENT_NAME}` resolves
only when that azd environment value has already been set. The sample now uses
the declared deployment name directly in the service environment so a fresh
provision-and-deploy works without a hidden manual step.

**Doc opportunity:** Show either an explicit literal model deployment name or
an `azd env set` step before the first Hosted Agent deployment. **Priority:
P0** because the deployment reports success but the runtime cannot initialize.

**Suggested owner:** Foundry Hosted Agent Docs / Foundry CLI.

### [Part 4] Capacity 10 can rate-limit a normal multi-tool response

**What tripped me up:** The agent completed service-status and KB-search tool
calls, then the final model turn failed with `Model deployment rate limit
exceeded` while eval generation was also running.

**Root cause / current state:** The generated sample capacity of 10 was too
small for concurrent setup verification. This sample now requests 50
GlobalStandard capacity units; the selected subscription has 1000 available.

**Doc opportunity:** Explain capacity units and provide a development baseline
that supports a multi-tool smoke test plus an evaluation job. **Priority: P1.**

**Suggested owner:** Foundry Model Deployment Docs / Hosted Agent Docs.

### [Setup] Azure CLI and `azd` can silently point at different subscriptions

**What tripped me up:** `az account show` and the saved `azd` defaults named
different subscriptions. Continuing without an explicit check could provision
resources in the wrong account.

**Root cause / current state:** Azure CLI and Azure Developer CLI maintain
separate context. This repo intentionally has no `azd` environment yet; the
receiving team must set `AZURE_SUBSCRIPTION_ID` and `AZURE_LOCATION`
explicitly before previewing infrastructure.

**Doc opportunity:** Put a subscription/region confirmation gate before
`azd provision`, including `az account show`, `azd env get-values`, model
availability, and quota checks. **Priority: P0.**

**Suggested owner:** Foundry Docs / Azure Developer CLI Docs.

### [Part 4] Bicep eject fails when `infra` contains an empty placeholder directory

**What tripped me up:** `azd ai agent init --infra=bicep --no-prompt` failed
with "infrastructure path contains files but no detectable entry point" even
though `infra` contained only an empty `modules` directory created by the
initial repo scaffold.

**Root cause / current state:** The eject command treats the placeholder
directory as existing infrastructure. Removing the empty folder allowed the
official provider to generate Bicep successfully.

**Doc opportunity:** Tell authors to leave `infra` absent/empty before Bicep
eject, or make the command ignore empty directories. **Priority: P1.**

**Suggested owner:** Foundry CLI / Foundry Hosted Agent Docs.

### [Part 4] Generated Bicep compiles with preview type warnings

**What tripped me up:** `az bicep build` succeeded but reported a type mismatch
for `networkInjections` and missing type information for
`Microsoft.CognitiveServices/accounts/managednetworks@2025-10-01-preview`.

**Root cause / current state:** The official `microsoft.foundry` provider
generates network-capable modules even for the default public/basic setup.
Both warnings are in optional network-isolation code and do not block compile.

**Doc opportunity:** Document the expected warnings for ejected Bicep, or
update the generated template/type metadata so a clean default project builds
without warnings. **Priority: P2.**

**Suggested owner:** Foundry CLI / Bicep type owners.

### [Part 5] A MAF + Foundry agent is a custom engine agent, not a declarative-agent action

**What tripped me up:** The initial walkthrough outline assumed that a
declarative agent would call the Foundry Hosted Agent through an API-plugin
action. Current Microsoft 365 guidance distinguishes declarative agents
(Copilot owns the orchestrator and model) from custom engine agents (the
developer brings the orchestrator and model). This sample brings both MAF and
the Foundry model, so it fits the custom engine category.

**Root cause / current state:** The supported path is Microsoft 365 Agents SDK
+ Azure Bot Service / Activity protocol + an M365 app manifest whose
`copilotAgents.customEngineAgents` entry references the bot. Foundry's current
`azd` hosted-agent workflow also recognizes the Activity protocol and can
generate a `TEAMS_APP_SETUP.md` after deployment.

**Doc opportunity:** Rewrite the proposed Part 5 around the custom-engine
decision and Activity protocol. Include a short decision box: use a declarative
agent when Copilot owns orchestration/model; use a custom engine agent when
MAF + Foundry own them. **Priority: P0** because the two approaches imply
different code, hosting, identity, and packaging.

**Suggested owner:** M365 Copilot Extensibility / MAF / Foundry Docs.

### [Part 5] The "Bring your agents into Microsoft 365 Copilot" sample link is stale

**What tripped me up:** The Learn page linked to
`samples/basic/authorization/auto-signin/dotnet/appManifest/m365copilot-manifest.json`
in `microsoft/Agents`, but that path returned 404.

**Root cause / current state:** The current .NET quickstart lives under
`samples/dotnet/quickstart/` and includes an app manifest using schema 1.22 and
`copilotAgents.customEngineAgents`.

**Doc opportunity:** Update the stale Learn link and point to the maintained
.NET quickstart manifest. **Priority: P1.**

**Suggested owner:** M365 Copilot Extensibility Docs.

### [Part 6b] `FoundryEvals` requires a separate preview integration package

**What tripped me up:** Adding stable `Microsoft.Agents.AI` 1.20.0 was enough
for `LocalEvaluator`, but `FoundryEvals` did not compile. The type lives in the
`Microsoft.Agents.AI.Foundry` namespace and currently requires the separate
`Microsoft.Agents.AI.Foundry` preview package.

**Root cause / current state:** The local and cloud evaluation APIs ship on
different package/version tracks. For this build the working combination is
`Microsoft.Agents.AI` 1.20.0 plus
`Microsoft.Agents.AI.Foundry` 1.20.0-preview.260831.1.

**Doc opportunity:** Put the exact package table and `using` directives beside
the first cloud-eval sample. **Priority: P1.**

**Suggested owner:** MAF Docs / Foundry Evaluation Docs.

### [Setup] Foundry's `azd` extension fails against an older Azure Developer CLI

**What tripped me up:** The Microsoft Foundry dependency setup attempted to
install the `microsoft.foundry` `azd` extension, but the extension had no
compatible version for the installed Azure Developer CLI (`azd` 1.23.8).
The setup script reported 1.33.0 as the latest stable release.

**Root cause / current state:** The hosted-agent workflow depends on an `azd`
extension whose compatibility floor has moved ahead of the installed CLI.
The failure message is actionable, but only after the user runs the workflow
dependency checker.

**Doc opportunity:** Add an explicit `azd --version` prerequisite and minimum
supported version to the Hosted Agent and MAF-to-Foundry walkthrough setup.
Show the Windows upgrade command before extension installation. **Priority:
P0** because the normal setup path stops immediately.

**Suggested owner:** Foundry Docs / Foundry CLI.
