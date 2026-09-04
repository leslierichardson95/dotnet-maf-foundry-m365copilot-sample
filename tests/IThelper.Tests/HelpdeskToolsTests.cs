using System.Text.Json;
using ITHelper.Grounding;
using ITHelper.Tools;

namespace ITHelper.Tests;

public sealed class HelpdeskToolsTests
{
    [Fact]
    public void CheckServiceStatusReportsKnownVpnDegradation()
    {
        var tools = CreateTools();

        using var result = JsonDocument.Parse(tools.CheckServiceStatus("VPN"));

        Assert.Equal("Degraded", result.RootElement.GetProperty("status").GetString());
    }

    [Fact]
    public void LookupUserTicketsFindsSeedTicket()
    {
        var tools = CreateTools();

        using var result = JsonDocument.Parse(
            tools.LookupUserTickets("alex@example.com"));

        Assert.Equal(
            "INC-1042",
            result.RootElement[0].GetProperty("Id").GetString());
    }

    [Fact]
    public void EscalateRejectsUnknownTicket()
    {
        var tools = CreateTools();

        using var result = JsonDocument.Parse(
            tools.EscalateToOnCall("INC-9999", "Test impact"));

        Assert.False(result.RootElement.GetProperty("success").GetBoolean());
    }

    private static HelpdeskTools CreateTools() =>
        new(KnowledgeBase.Load(
            Path.Combine(AppContext.BaseDirectory, "data", "kb")));
}
