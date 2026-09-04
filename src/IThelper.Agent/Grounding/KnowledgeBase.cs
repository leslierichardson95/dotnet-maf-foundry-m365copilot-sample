using System.Text.RegularExpressions;

namespace ITHelper.Grounding;

public sealed class KnowledgeBase
{
    private readonly IReadOnlyList<KnowledgeArticle> _articles;

    private KnowledgeBase(IReadOnlyList<KnowledgeArticle> articles)
    {
        _articles = articles;
    }

    public static KnowledgeBase Load(string directory)
    {
        if (!Directory.Exists(directory))
        {
            throw new DirectoryNotFoundException(
                $"Knowledge-base directory was not found: {directory}");
        }

        var articles = Directory
            .EnumerateFiles(directory, "*.md", SearchOption.AllDirectories)
            .Select(path => KnowledgeArticle.FromFile(path))
            .ToArray();

        if (articles.Length == 0)
        {
            throw new InvalidOperationException(
                $"No Markdown knowledge-base articles were found in {directory}.");
        }

        return new KnowledgeBase(articles);
    }

    public IReadOnlyList<KnowledgeSearchResult> Search(string query, int maxResults = 3)
    {
        var queryTerms = Tokenize(query).ToHashSet(StringComparer.OrdinalIgnoreCase);

        return _articles
            .Select(article => new
            {
                Article = article,
                Score = Tokenize($"{article.Title} {article.Content}")
                    .Count(queryTerms.Contains)
            })
            .Where(match => match.Score > 0)
            .OrderByDescending(match => match.Score)
            .ThenBy(match => match.Article.Title, StringComparer.OrdinalIgnoreCase)
            .Take(Math.Clamp(maxResults, 1, 5))
            .Select(match => new KnowledgeSearchResult(
                match.Article.Title,
                match.Article.FileName,
                match.Article.Content,
                match.Score))
            .ToArray();
    }

    private static IEnumerable<string> Tokenize(string value) =>
        Regex.Matches(value.ToLowerInvariant(), "[a-z0-9]+")
            .Select(match => match.Value)
            .Where(token => token.Length > 2);
}

internal sealed record KnowledgeArticle(string Title, string FileName, string Content)
{
    public static KnowledgeArticle FromFile(string path)
    {
        var content = File.ReadAllText(path).Trim();
        var title = content
            .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
            .FirstOrDefault()?
            .TrimStart('#', ' ')
            ?? Path.GetFileNameWithoutExtension(path);

        return new KnowledgeArticle(title, Path.GetFileName(path), content);
    }
}

public sealed record KnowledgeSearchResult(
    string Title,
    string FileName,
    string Content,
    int Score);
