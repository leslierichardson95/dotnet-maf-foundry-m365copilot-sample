# Part 6b — Add evaluation before rollout

Use three progressive layers.

## 1. Validate the dataset offline

```powershell
dotnet run --project src\IThelper.Eval -- --validate-dataset
```

The JSONL rows include `query` and `expected_behavior` so the same data can
support local smoke checks and later custom Foundry rubrics.

## 2. Run local MAF checks

```powershell
.\scripts\run-eval.ps1
```

The current harness checks:

- service-status questions invoke `CheckServiceStatus`;
- VPN troubleshooting invokes `SearchKnowledgeBase`;
- responses do not request MFA codes.

Primary reference:

- [MAF agent evaluation](https://learn.microsoft.com/agent-framework/agents/evaluation)

## 3. Run Foundry cloud evaluation

```powershell
.\scripts\run-eval.ps1 -Foundry
```

This uses `FoundryEvals`. Current package split:

- `Microsoft.Agents.AI` 1.20.0 — local evaluation;
- `Microsoft.Agents.AI.Foundry` 1.20.0-preview.260831.1 — `FoundryEvals`.

Pass evaluators explicitly. The verified cloud baseline uses relevance and
coherence, while local MAF checks cover tool calls and credential safety. The
preview package's default evaluator set currently adds task adherence but can
omit its required `tool_definitions` mapping for tool-enabled agents, causing
`EvalValidationFailed`.

The command prints a `Report: https://ai.azure.com/...` link when the cloud run
completes. Open that link for the exact run, or open the Foundry project,
select **Evaluation**, select the latest run, and inspect:

1. the overall completed/passed status;
2. aggregate relevance, coherence, and tool-call-accuracy metrics;
3. each row's query, response, tool calls, metric values, and interpretation.

The command exits nonzero if any check fails, so the same script can be used as
a CI quality gate.

References:

- [Foundry agent evaluators](https://learn.microsoft.com/azure/foundry/concepts/evaluation-evaluators/agent-evaluators)
- [View evaluation results](https://learn.microsoft.com/azure/foundry/how-to/evaluate-results)

## Verified first gate

- all local tool-call and credential-safety checks pass;
- relevance and coherence pass for both cloud-evaluation prompts;
- tool-call accuracy passes for both cloud-evaluation prompts.

The sample reached 2/2 passing rows. Add task adherence and groundedness after
the .NET preview mapping issue is resolved or a custom evaluator is supplied.
