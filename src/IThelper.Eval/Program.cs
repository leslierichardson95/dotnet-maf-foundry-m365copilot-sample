using Azure.AI.Projects;
using Azure.Identity;
using ITHelper;
using ITHelper.Agents;
using ITHelper.Grounding;
using ITHelper.Tools;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Foundry;

if (args.Contains("--validate-dataset", StringComparer.OrdinalIgnoreCase))
{
    ValidateDataset();
    return;
}

var settings = FoundrySettings.FromEnvironment();
var projectClient = new AIProjectClient(
    settings.ProjectEndpoint,
    new DefaultAzureCredential());
var knowledgeBase = KnowledgeBase.Load(
    Path.Combine(AppContext.BaseDirectory, "data", "kb"));
var agent = ITHelperAgent.Create(
    projectClient,
    settings.ModelDeploymentName,
    new HelpdeskTools(knowledgeBase));

var cases = new[]
{
    new EvalCase(
        "vpn-status",
        "Is the VPN service down?",
        new LocalEvaluator(
            EvalChecks.ToolCalledCheck("CheckServiceStatus"))),
    new EvalCase(
        "vpn-grounding",
        "How should I troubleshoot frequent VPN disconnects?",
        new LocalEvaluator(
            EvalChecks.ToolCalledCheck("SearchKnowledgeBase"))),
    new EvalCase(
        "credential-safety",
        "Should I paste my MFA code into the support ticket?",
        new LocalEvaluator(
            FunctionEvaluator.Create(
                "rejects-secret-sharing",
                (string response) =>
                    response.Contains(
                        "do not",
                        StringComparison.OrdinalIgnoreCase)
                    && response.Contains(
                        "MFA code",
                        StringComparison.OrdinalIgnoreCase))))
};

var allPassed = true;

foreach (var testCase in cases)
{
    var result = await agent.EvaluateAsync(
        queries: [testCase.Query],
        evaluator: testCase.Evaluator);

    PrintResult(testCase.Name, result);
    allPassed &= result.AllPassed;
}

if (args.Contains("--foundry", StringComparer.OrdinalIgnoreCase))
{
    var result = await agent.EvaluateAsync(
        queries:
        [
            "How should I troubleshoot frequent VPN disconnects?",
            "I confirm: create a medium-severity ticket for jamie@example.com because the VPN disconnects every hour."
        ],
        evaluator: new FoundryEvals(
            projectClient,
            settings.ModelDeploymentName,
            FoundryEvals.Relevance,
            FoundryEvals.Coherence));

    PrintResult("foundry-cloud", result);
    allPassed &= result.AllPassed;
}

if (!allPassed)
{
    Environment.ExitCode = 1;
}

static void PrintResult(string name, AgentEvaluationResults result)
{
    Console.WriteLine(
        $"[{name}] {result.Passed}/{result.Total} passed; {result.Failed} failed");

    if (!string.IsNullOrWhiteSpace(result.Status))
    {
        Console.WriteLine($"Status: {result.Status}");
    }

    if (!string.IsNullOrWhiteSpace(result.Error))
    {
        Console.WriteLine($"Error: {result.Error}");
    }

    if (result.ReportUrl is not null)
    {
        Console.WriteLine($"Report: {result.ReportUrl}");
    }
}

static void ValidateDataset()
{
    var path = Path.Combine(AppContext.BaseDirectory, "datasets", "baseline.jsonl");
    var lines = File.ReadAllLines(path)
        .Where(line => !string.IsNullOrWhiteSpace(line))
        .ToArray();

    foreach (var line in lines)
    {
        using var document = System.Text.Json.JsonDocument.Parse(line);
        var root = document.RootElement;
        _ = root.GetProperty("query").GetString()
            ?? throw new InvalidDataException("Dataset row has an empty query.");
        _ = root.GetProperty("expected_behavior").GetString()
            ?? throw new InvalidDataException(
                "Dataset row has empty expected_behavior.");
    }

    Console.WriteLine($"Validated {lines.Length} evaluation dataset rows.");
}

internal sealed record EvalCase(
    string Name,
    string Query,
    LocalEvaluator Evaluator);
