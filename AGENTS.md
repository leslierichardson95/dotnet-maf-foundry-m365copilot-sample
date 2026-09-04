# Agent instructions

This project was built with the `microsoft-foundry` skill. Before working on
or answering questions about Foundry agents, read the `microsoft-foundry`
skill first.

## Project constraints

- Target .NET 10.
- Use Microsoft Agent Framework, not Semantic Kernel.
- Keep Aspire out of the core path.
- Use `azd` and the official Microsoft Foundry provider for provisioning and
  hosted-agent deployment.
- Never commit tenant IDs, subscription IDs, resource IDs, endpoints, keys, or
  `.azure` environment state.
- Keep the sample runnable with synthetic IT helpdesk data only.
