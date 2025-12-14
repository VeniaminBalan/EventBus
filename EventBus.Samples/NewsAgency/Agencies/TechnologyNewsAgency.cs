using EventBus.Samples.NewsAgency.Events;

namespace EventBus.Samples.NewsAgency.Agencies;

public class TechnologyNewsAgency : BaseNewsAgency
{
    private readonly string[] _techHeadlines =
    [
        "AI Breakthrough Announced",
        "New Smartphone Released",
        "Tech Giant Launches Innovation",
        "Cybersecurity Threat Detected",
        "Startup Receives Major Funding",
        "Software Update Revolutionizes Industry",
        "Quantum Computing Milestone Reached",
        "Green Technology Solution Unveiled"
    ];

    public TechnologyNewsAgency(Core.EventBus eventBus) 
        : base("TechNow", eventBus)
    {
    }

    public override async Task StartPublishingAsync(CancellationToken cancellationToken)
    {
        IsPublishing = true;
        Console.WriteLine($"{AgencyName} started publishing technology news");

        while (IsPublishing && !cancellationToken.IsCancellationRequested)
        {
            var headline = _techHeadlines[Random.Next(_techHeadlines.Length)];
            var content = $"Tech report: {headline}. Technical details and specifications...";
            
            PublishArticle(headline, content, NewsCategory.Technology);

            if (Random.NextDouble() < 0.25)
            {
                PublishBreakingNews(
                    "Tech Industry Disruption!",
                    "Revolutionary technology announcement shakes the market...",
                    NewsCategory.Technology,
                    7
                );
            }

            await Task.Delay(Random.Next(3500, 6500), cancellationToken);
        }
    }
}
