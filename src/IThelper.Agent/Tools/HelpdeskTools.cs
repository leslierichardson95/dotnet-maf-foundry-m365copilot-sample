using System.ComponentModel;
using System.Text.Json;
using ITHelper.Grounding;

namespace ITHelper.Tools;

public sealed class HelpdeskTools
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true
    };

    private readonly KnowledgeBase _knowledgeBase;
    private readonly List<Ticket> _tickets =
    [
        new("INC-1042", "alex@example.com", "VPN disconnects every hour", "Open"),
        new("REQ-2088", "jamie@example.com", "Request access to Finance SharePoint", "Waiting for approval")
    ];

    public HelpdeskTools(KnowledgeBase knowledgeBase)
    {
        _knowledgeBase = knowledgeBase;
    }

    [Description("Checks the current health of an internal IT service.")]
    public string CheckServiceStatus(
        [Description("Service name, such as VPN, Microsoft 365, or identity")] string serviceName)
    {
        var normalized = serviceName.Trim().ToLowerInvariant();
        var result = normalized switch
        {
            "vpn" => new { service = "VPN", status = "Degraded", detail = "Intermittent disconnects are under investigation." },
            "microsoft 365" or "m365" => new { service = "Microsoft 365", status = "Operational", detail = "No active incident." },
            "identity" or "entra" or "mfa" => new { service = "Identity", status = "Operational", detail = "No active incident." },
            _ => new { service = serviceName, status = "Unknown", detail = "Service is not in the sample status catalog." }
        };

        return JsonSerializer.Serialize(result, JsonOptions);
    }

    [Description("Lists existing helpdesk tickets for a user.")]
    public string LookupUserTickets(
        [Description("User email address")] string userEmail)
    {
        var matches = _tickets
            .Where(ticket => ticket.UserEmail.Equals(userEmail, StringComparison.OrdinalIgnoreCase))
            .ToArray();

        return JsonSerializer.Serialize(matches, JsonOptions);
    }

    [Description("Creates a new helpdesk ticket. Call only after the user confirms.")]
    public string CreateTicket(
        [Description("User email address")] string userEmail,
        [Description("Short problem summary")] string summary,
        [Description("Severity: low, medium, high, or critical")] string severity = "medium")
    {
        var id = $"INC-{Random.Shared.Next(3000, 9999)}";
        var ticket = new Ticket(id, userEmail, summary, $"Open ({severity})");
        _tickets.Add(ticket);

        return JsonSerializer.Serialize(ticket, JsonOptions);
    }

    [Description("Escalates a confirmed, high-impact incident to the sample on-call queue.")]
    public string EscalateToOnCall(
        [Description("Existing incident ID")] string ticketId,
        [Description("Business impact summary")] string impact)
    {
        var ticket = _tickets.FirstOrDefault(
            item => item.Id.Equals(ticketId, StringComparison.OrdinalIgnoreCase));

        if (ticket is null)
        {
            return JsonSerializer.Serialize(
                new { success = false, error = $"Ticket {ticketId} was not found." },
                JsonOptions);
        }

        return JsonSerializer.Serialize(
            new
            {
                success = true,
                ticketId,
                queue = "Sample IT On-Call",
                impact,
                message = "Escalation recorded in the in-memory sample only."
            },
            JsonOptions);
    }

    [Description("Searches internal IT knowledge-base articles.")]
    public string SearchKnowledgeBase(
        [Description("The IT setup, troubleshooting, or policy question")] string query)
    {
        var results = _knowledgeBase.Search(query)
            .Select(result => new
            {
                result.Title,
                citation = result.FileName,
                content = result.Content,
                result.Score
            });

        return JsonSerializer.Serialize(results, JsonOptions);
    }

    private sealed record Ticket(
        string Id,
        string UserEmail,
        string Summary,
        string Status);
}
