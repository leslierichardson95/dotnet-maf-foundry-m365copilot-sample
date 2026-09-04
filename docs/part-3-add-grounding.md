# Part 3 — Add optional file-based grounding

## Goal

Answer company-specific IT questions from synthetic Markdown articles and
cite the article used.

## How it works

`KnowledgeBase` loads `data\kb\*.md`, tokenizes the query and articles, and
returns the highest-overlap results. `SearchKnowledgeBase` exposes those
results as a MAF function tool.

This deliberately uses a transparent in-memory algorithm. It teaches the
retrieval/tool boundary without requiring another Azure service.

## Verify

```powershell
dotnet run --project src\IThelper.Agent -- --self-test
```

The output should rank `vpn-troubleshooting.md` for a VPN-disconnect query.

With a model configured, ask:

> What should I do if the VPN disconnects every hour?

The agent instructions require a citation when the KB supplied the answer.

## Production follow-up

Replace this in-memory search with Foundry IQ or another enterprise retrieval
service when the receiving team has confirmed the preferred .NET integration
and authorization pattern.
