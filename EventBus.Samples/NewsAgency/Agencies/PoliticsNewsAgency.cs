using EventBus.Samples.NewsAgency.Events;

namespace EventBus.Samples.NewsAgency.Agencies;

public class PoliticsNewsAgency : BaseNewsAgency
{
    private readonly string[] _politicsHeadlines = new[]
    {
        "Parliament Passes New Legislation",
        "Election Campaign Intensifies",
        "International Summit Concludes",
        "Policy Reform Announced",
        "Political Leader Makes Statement",
        "Budget Proposal Under Review",
        "Coalition Agreement Reached",
        "Diplomatic Relations Strengthened"
    };

    public PoliticsNewsAgency(Core.EventBus eventBus) 
        : base("PoliTimes", eventBus)
    {
    }

    public override async Task StartPublishingAsync(CancellationToken cancellationToken)
    {
        IsPublishing = true;
        Console.WriteLine($"{AgencyName} started publishing political news");

        while (IsPublishing && !cancellationToken.IsCancellationRequested)
        {
            var headline = _politicsHeadlines[Random.Next(_politicsHeadlines.Length)];
            var content = $"Political analysis: {headline}. Expert commentary available...";
            
            PublishArticle(headline, content, NewsCategory.Politics, priority: 5);

            // Breaking political news
            if (Random.NextDouble() < 0.15)
            {
                PublishBreakingNews(
                    "Major Political Development",
                    "Urgent political update with significant implications...",
                    NewsCategory.Politics,
                    9
                );
            }

            await Task.Delay(Random.Next(4000, 7000), cancellationToken);
        }
    }
}
