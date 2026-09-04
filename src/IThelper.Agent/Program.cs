using Azure.AI.AgentServer.Core;
using Azure.AI.Projects;
using Azure.Core;
using Azure.Identity;
using ITHelper;
using ITHelper.Agents;
using ITHelper.Grounding;
using ITHelper.Tools;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Foundry.Hosting;

var knowledgeBase = KnowledgeBase.Load(
    Path.Combine(AppContext.BaseDirectory, "data", "kb"));
var tools = new HelpdeskTools(knowledgeBase);

if (args.Contains("--self-test", StringComparer.OrdinalIgnoreCase))
{
    RunSelfTest(tools);
    return;
}

var settings = FoundrySettings.FromEnvironment();
TokenCredential credential = new DefaultAzureCredential();

AIAgent agent = ITHelperAgent.Create(
    new AIProjectClient(settings.ProjectEndpoint, credential),
    settings.ModelDeploymentName,
    tools);

if (args.Contains("--chat", StringComparer.OrdinalIgnoreCase))
{
    await RunChatAsync(agent);
    return;
}

var builder = AgentHost.CreateBuilder(args);
builder.Services.AddFoundryResponses(agent);
builder.RegisterProtocol("responses", endpoints => endpoints.MapFoundryResponses());

var app = builder.Build();
app.Run();

static async Task RunChatAsync(AIAgent agent)
{
    var session = await agent.CreateSessionAsync();

    Console.WriteLine("IT Helper is ready. Type '/reset' for a new conversation or 'exit' to quit.");

    while (true)
    {
        Console.Write("\nYou: ");
        var input = Console.ReadLine();

        if (input is null)
        {
            break;
        }

        if (string.IsNullOrWhiteSpace(input))
        {
            continue;
        }

        if (input.Equals("exit", StringComparison.OrdinalIgnoreCase))
        {
            break;
        }

        if (input.Equals("/reset", StringComparison.OrdinalIgnoreCase))
        {
            session = await agent.CreateSessionAsync();
            Console.WriteLine("\nConversation reset.");
            continue;
        }

        var response = await agent.RunAsync(input, session);
        Console.WriteLine($"\nIT Helper: {response}");
    }
}

static void RunSelfTest(HelpdeskTools tools)
{
    Console.WriteLine("Service status:");
    Console.WriteLine(tools.CheckServiceStatus("VPN"));

    Console.WriteLine("\nKnowledge-base search:");
    Console.WriteLine(tools.SearchKnowledgeBase("VPN keeps disconnecting"));

    Console.WriteLine("\nExisting tickets:");
    Console.WriteLine(tools.LookupUserTickets("alex@example.com"));
}
