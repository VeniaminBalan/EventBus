using EventBus.Samples.NewsAgency.Events;

namespace EventBus.Samples.NewsAgency.Agencies;

public abstract class BaseNewsAgency
{
    protected string AgencyName { get; set; }
    protected Core.EventBus EventBus { get; set; }
    protected Random Random { get; set; }
    protected bool IsPublishing { get; set; }

    protected BaseNewsAgency(string agencyName, Core.EventBus eventBus)
    {
        AgencyName = agencyName;
        EventBus = eventBus;
        Random = new Random();
        IsPublishing = false;
    }

    public abstract Task StartPublishingAsync(CancellationToken cancellationToken);

    public void StopPublishing()
    {
        IsPublishing = false;
    }

    protected void PublishArticle(string headline, string content, NewsCategory category, int priority = 0)
    {
        var article = new NewsArticleEvent(headline, content, AgencyName, category, priority);
        EventBus.Publish(article);
        Console.WriteLine($"{AgencyName} published: {headline}");
    }

    protected void PublishBreakingNews(string headline, string content, NewsCategory domain, int urgency = 10)
    {
        var breaking = new BreakingNewsEvent(headline, content, AgencyName, domain, urgency);
        EventBus.Publish(breaking);
        Console.WriteLine($"{AgencyName} BREAKING: {headline}");
    }
}
