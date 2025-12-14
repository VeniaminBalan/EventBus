using EventBus.Core.Attributes;
using EventBus.Samples.NewsAgency.Events;

namespace EventBus.Samples.NewsAgency.Subscribers;

public class NewsAggregator
{
    private readonly string _name;
    private readonly Dictionary<NewsCategory, int> _categoryCount = new();
    private int _totalArticles = 0;

    public NewsAggregator(string name)
    {
        _name = name;
        foreach (var category in Enum.GetValues<NewsCategory>())
        {
            _categoryCount[category] = 0;
        }
    }

    [EventHandler]
    public void OnNewsArticle(NewsArticleEvent article)
    {
        _categoryCount[article.Category]++;
        _totalArticles++;

        if (_totalArticles % 10 == 0)
        {
            DisplayStatistics();
        }
    }

    [EventHandler]
    public void OnBreakingNews(BreakingNewsEvent breaking)
    {
        _categoryCount[breaking.Domain]++;
        _totalArticles++;
    }

    private void DisplayStatistics()
    {
        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.WriteLine($"\n[{_name}] News Statistics (Total: {_totalArticles}):");
        foreach (var category in _categoryCount.OrderByDescending(x => x.Value))
        {
            if (category.Value > 0)
            {
                Console.WriteLine($"   {category.Key}: {category.Value} articles");
            }
        }
        Console.WriteLine();
        Console.ResetColor();
    }
}
