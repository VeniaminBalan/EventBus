namespace EventBus.Samples.NewsAgency.Events;

public class BreakingNewsEvent
{
    public string Headline { get; set; }
    public string Content { get; set; }
    public string Agency { get; set; }
    public NewsCategory Domain { get; set; }
    public int Urgency { get; set; }
    public DateTime Timestamp { get; set; }

    public BreakingNewsEvent(string headline, string content, string agency, NewsCategory domain, int urgency = 10)
    {
        Headline = headline;
        Content = content;
        Agency = agency;
        Domain = domain;
        Urgency = urgency;
        Timestamp = DateTime.Now;
    }

    public override string ToString()
    {
        return $"BREAKING [{Timestamp:HH:mm:ss}] [{Domain}] {Headline} - {Agency} (Urgency: {Urgency}/10)";
    }
}
