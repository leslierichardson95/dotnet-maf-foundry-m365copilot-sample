# Part 5 — Bring the custom engine agent into Microsoft 365 Copilot

## Important architecture correction

This sample is a **custom engine agent**, because MAF supplies the orchestrator
and Foundry supplies the chosen model and hosted runtime. It is not a
declarative agent: declarative agents use Copilot's orchestrator and model.

## Supported path to validate

Current guidance is:

1. Add Microsoft 365 Agents SDK Activity handling to the existing agent.
2. Expose the Hosted Agent through the Activity protocol.
3. Create an app registration and Azure Bot Service record.
4. Package an M365 app manifest with
   `copilotAgents.customEngineAgents`.
5. Sideload it with Microsoft 365 Agents Toolkit.

Start with:

- [Bring your agents into Microsoft 365 Copilot](https://learn.microsoft.com/microsoft-365/copilot/extensibility/bring-agents-to-copilot)
- [Microsoft 365 Agents SDK .NET quickstart](https://github.com/microsoft/Agents/tree/main/samples/dotnet/quickstart)
- [`m365\manifest.json`](../m365/manifest.json)

## Why this handoff stops short

A licensed development tenant and permission to create/configure the bot
channel and sideload the package were not available during the build. The
manifest is therefore a starting point, not a verified package.

## Verification checklist for the receiving team

- [ ] Confirm the current Foundry .NET Activity-protocol adapter.
- [ ] Add `protocol: activity` to `azure.yaml`.
- [ ] Deploy and inspect generated `TEAMS_APP_SETUP.md`.
- [ ] Configure the app registration and Azure Bot Service.
- [ ] Add valid 192x192 color and 32x32 outline icons.
- [ ] Sideload the package in Microsoft 365 Copilot Chat.
- [ ] Verify the message reaches the same MAF agent and invokes a tool.
