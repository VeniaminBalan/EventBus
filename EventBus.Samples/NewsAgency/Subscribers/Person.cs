using EventBus.Core.Attributes;
using EventBus.Samples.NewsAgency.Events;

namespace EventBus.Samples.NewsAgency.Subscribers;

public class Person
{
    private readonly string _name;
    private readonly HashSet<NewsCategory> _interests;

    public Person(string name, params NewsCategory[] interests)
    {
        _name = name;
        _interests = new HashSet<NewsCategory>(interests);
    }

    public void AddInterest(NewsCategory category)
    {
        _interests.Add(category);
        Console.WriteLine($"👤 {_name} is now interested in {category} news");
    }

    public void RemoveInterest(NewsCategory category)
    {
        _interests.Remove(category);
        Console.WriteLine($"👤 {_name} is no longer interested in {category} news");
    }

    [EventHandler]
    public void OnNewsArticle(NewsArticleEvent article)
    {
        if (_interests.Contains(article.Category))
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"👤 [{_name}] Read: {article}");
            Console.ResetColor();
        }
    }

    [EventHandler(Priority = 100)]
    public void OnBreakingNews(BreakingNewsEvent breaking)
    {
        if (_interests.Contains(breaking.Domain))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"👤 [{_name}] ALERT: {breaking}");
            Console.ResetColor();
        }
    }
}
