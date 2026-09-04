using ITHelper.Grounding;

namespace ITHelper.Tests;

public sealed class KnowledgeBaseTests
{
    [Fact]
    public void SearchRanksVpnTroubleshootingForDisconnectQuery()
    {
        var knowledgeBase = LoadKnowledgeBase();

        var results = knowledgeBase.Search("VPN keeps disconnecting");

        Assert.NotEmpty(results);
        Assert.Equal("vpn-troubleshooting.md", results[0].FileName);
        Assert.Equal("Troubleshoot frequent VPN disconnects", results[0].Title);
        Assert.True(results[0].Score > 0);
    }

    [Fact]
    public void SearchReturnsNoResultsForUnrelatedQuery()
    {
        var knowledgeBase = LoadKnowledgeBase();

        var results = knowledgeBase.Search("cafeteria menu");

        Assert.Empty(results);
    }

    private static KnowledgeBase LoadKnowledgeBase() =>
        KnowledgeBase.Load(
            Path.Combine(AppContext.BaseDirectory, "data", "kb"));
}
