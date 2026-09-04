# Azure Deployment Plan

**Status:** Validated

## Objective

Build a public-safe .NET 10 sample and documentation handoff for an IT helpdesk
agent authored with Microsoft Agent Framework, backed by Microsoft Foundry, and
prepared for exposure through Microsoft 365 Copilot.

## Mode

New application and infrastructure.

## Recipe

`azd`

## Architecture

- .NET 10 console client for local authoring and verification.
- Microsoft Agent Framework agent with IT helpdesk tools.
- Microsoft Foundry project and model deployment.
- Foundry-hosted agent deployment, if the currently documented resource and
  deployment APIs support repeatable infrastructure-as-code provisioning.
- File-based grounding sample with synthetic Markdown knowledge-base content.
- Local and Foundry evaluation harnesses.
- Microsoft 365 custom-engine app artifacts prepared for tenant-specific
  Activity-protocol validation and sideloading by the receiving team.

## Provisioning approach

- Azure Developer CLI (`azd`) orchestrates provisioning.
- The official `microsoft.foundry` `azd` provider is the source of truth.
- If checked-in Bicep is needed for the documentation handoff, it is ejected
  from the project's own `azure.yaml` with `azd ai agent init --infra=bicep`;
  no external starter pack or sample infrastructure is imported.
- Azure values come from `azd` environment variables and Bicep outputs.
- Runtime authentication uses Microsoft Entra ID and
  `DefaultAzureCredential`; no credentials are committed.
- Azure deployment is interactive. Subscription and region must be confirmed
  before validation and deployment.

## Planned Azure resources

- Resource group (managed by `azd`).
- Microsoft Foundry account/project resources required by current APIs.
- One inexpensive, tool-calling-capable model deployment.
- Identity and least-privilege role assignments.
- Hosted-agent resource/deployment if supported by the current documented
  provisioning surface.

## Walkthrough mapping

| Part | Deliverable |
|---|---|
| 1 | Overview and prerequisites documentation |
| 2 | Working MAF agent against a Foundry model |
| 3 | Working optional file-based grounding; Foundry IQ documented separately |
| 4 | Hosted-agent deployment and verification |
| 5 | M365 Copilot custom-engine/Activity-protocol scaffold; tenant sideload remains unverified |
| 6a | Organization publishing documentation |
| 6b | Local and Foundry evaluation flows |

## Security

- No keys, tenant IDs, subscription IDs, resource IDs, or live endpoints in
  committed files.
- `.azure` runtime state remains ignored except for this plan.
- Managed identity and least-privilege role assignments are preferred.
- A repository-wide secret and identifier sweep is required before handoff.

## Validation

- Restore and build all .NET projects with .NET 10.
- Run the local agent and verify at least one tool call.
- Verify grounded responses against synthetic KB content.
- Validate Bicep and `azure.yaml`.
- Invoke `azure-validate` before any deployment.
- After validation, use `azure-deploy` for interactive provisioning.
- Verify a Foundry evaluation run appears in the portal where supported.

### All validation checks pass

- [x] 1. AZD Installation
- [x] 2. Schema Validation
- [x] 3. Environment Setup
- [x] 4. Authentication Check
- [x] 5. Subscription/Location Check
- [x] 6. Aspire Pre-Provisioning Checks (not applicable)
- [x] 7. Provision Preview
- [x] 8. Build Verification
- [x] 9. Docker Build Context Validation (not applicable; direct code deployment)
- [x] 10. Package Validation
- [x] 11. Azure Policy Validation
- [x] 12. Aspire Post-Provisioning Checks (not applicable)

## Validation Proof

Validated on 2026-09-03 PDT:

- `azd version`: 1.33.0.
- `azd auth login --check-status`: authenticated as the intended user.
- Environment `ithelper-dev` targets the selected development subscription in
  `northcentralus`, with the tenant and developer principal explicitly set in
  ignored azd environment state.
- `gpt-5.4-mini` version `2026-03-17` supports `GlobalStandard` in
  `northcentralus`; 1000 units are available and this deployment requests 50.
- `azd provision --preview --no-prompt -e ithelper-dev`: succeeded and plans
  only the resource group, Foundry account/project, and model deployment.
- `dotnet build dotnet-maf-foundry-m365copilot-sample.sln`: succeeded with no
  warnings or errors.
- `azd package --no-prompt -e ithelper-dev`: direct-code package succeeded.
- Subscription policy assignments were reviewed; the custom policies apply
  only to virtual network membership, and this public/basic deployment creates
  no VNet.

## Role Assignment Verification

- **Status:** Verified for the current basic/direct-code architecture.
- **Identities checked:** Foundry account system identity, Foundry project
  system identity, and the local developer user.
- **Roles confirmed:** The local developer receives **Cognitive Services
  User** at the Foundry project scope for model and agent data-plane access.
  The role is scoped to the project rather than the resource group or
  subscription.
- **Runtime access:** The hosted agent uses the Foundry project identity and
  platform-managed model access within the same project. No external storage,
  search, Key Vault, network, or ACR data-plane roles are required by this
  sample.
- **Issues:** None for the declared resources. If external grounding or
  connections are added later, their service-specific data-plane roles must be
  added before deployment.

## Known constraints

- M365 Copilot sideload and organization publishing require a suitable tenant,
  license, and administrator permissions; those steps may remain scaffolded.
- Foundry hosted-agent infrastructure APIs may differ from portal or SDK
  surfaces. Any mismatch will be recorded as a documentation opportunity.
- Resource names, model choice, SKU, region, subscription, and teardown timing
  will be selected interactively.

## Execution checklist

- [x] Overall plan approved by user.
- [x] Research current MAF, Foundry, Foundry evaluation, and `azd` APIs.
- [x] Scaffold .NET solution and synthetic data.
- [x] Author and locally validate Azure artifacts.
- [x] Build and locally verify the application.
- [x] Update status to `Ready for Validation`.
- [x] Run `azure-validate`.
- [x] Confirm Azure subscription and region.
- [x] Run deployment through `azure-deploy`.
- [x] Verify cloud paths and record hiccups.
- [x] Perform security sweep.
- [ ] Tear down resources after user confirmation.
- [x] Finalize offline handoff documentation.

## Deployment result

- Environment: local `azd` development environment
- Resource group: generated in the selected developer subscription
- Region: `northcentralus`
- Model: `gpt-5.4-mini`, GlobalStandard capacity 50
- Hosted Agent: `it-helper`, active version
- Remote Responses invocation: completed with `CheckServiceStatus`
- Evaluation: three local checks passed; the final Foundry cloud run passed
  both test items across relevance, coherence, and tool-call accuracy
- Remaining gate: Microsoft 365 custom-engine/Activity-protocol tenant
  integration
