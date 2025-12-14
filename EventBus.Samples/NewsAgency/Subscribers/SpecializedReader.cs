using EventBus.Core.Attributes;
using EventBus.Samples.NewsAgency.Events;

namespace EventBus.Samples.NewsAgency.Subscribers;

public class SpecializedReader
{
    private readonly string _name;
    private readonly NewsCategory _specialization;

    public SpecializedReader(string name, NewsCategory specialization)
    {
        _name = name;
        _specialization = specialization;
    }

    [EventHandler]
    public void OnNewsArticle(NewsArticleEvent article)
    {
        if (article.Category == _specialization)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine($"📖 [{_name} - {_specialization} Specialist] Analyzing: {article.Headline}");
            Console.ResetColor();
        }
    }

    [EventHandler(Priority = 90)]
    public void OnBreakingNews(BreakingNewsEvent breaking)
    {
        if (breaking.Domain == _specialization)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"📖 [{_name} - {_specialization} Specialist] URGENT ANALYSIS: {breaking.Headline}");
            Console.ResetColor();
        }
    }
}
