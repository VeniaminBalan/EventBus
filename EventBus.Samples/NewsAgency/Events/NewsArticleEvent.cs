namespace EventBus.Samples.NewsAgency.Events;

public class NewsArticleEvent
{
    public string Headline { get; set; }
    public string Content { get; set; }
    public string Agency { get; set; }
    public NewsCategory Category { get; set; }
    public DateTime Timestamp { get; set; }
    public int Priority { get; set; }

    public NewsArticleEvent(string headline, string content, string agency, NewsCategory category, int priority = 0)
    {
        Headline = headline;
        Content = content;
        Agency = agency;
        Category = category;
        Timestamp = DateTime.Now;
        Priority = priority;
    }

    public override string ToString()
    {
        return $"[{Timestamp:HH:mm:ss}] [{Category}] {Headline} - {Agency}";
    }
}
