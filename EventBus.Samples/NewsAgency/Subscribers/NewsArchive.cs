using EventBus.Core.Attributes;
using EventBus.Samples.NewsAgency.Events;

namespace EventBus.Samples.NewsAgency.Subscribers;

public class NewsArchive
{
    private readonly List<NewsArticleEvent> _archivedArticles = new();
    private readonly List<BreakingNewsEvent> _archivedBreaking = new();

    [EventHandler]
    public void OnNewsArticle(NewsArticleEvent article)
    {
        _archivedArticles.Add(article);
    }

    [EventHandler]
    public void OnBreakingNews(BreakingNewsEvent breaking)
    {
        _archivedBreaking.Add(breaking);
    }

    public void DisplayArchiveSummary()
    {
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine($"\n📚 [Archive] Total archived: {_archivedArticles.Count} articles, {_archivedBreaking.Count} breaking news");
        Console.ResetColor();
    }

    public IEnumerable<NewsArticleEvent> SearchByCategory(NewsCategory category)
    {
        return _archivedArticles.Where(a => a.Category == category);
    }

    public IEnumerable<NewsArticleEvent> SearchByAgency(string agency)
    {
        return _archivedArticles.Where(a => a.Agency == agency);
    }
}
