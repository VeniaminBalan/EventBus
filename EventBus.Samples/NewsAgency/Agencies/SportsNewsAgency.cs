using EventBus.Samples.NewsAgency.Events;

namespace EventBus.Samples.NewsAgency.Agencies;

public class SportsNewsAgency : BaseNewsAgency
{
    private readonly string[] _sportsHeadlines = new[]
    {
        "Local Team Wins Championship",
        "Star Player Signs Record Contract",
        "Olympic Athlete Breaks World Record",
        "Underdog Team Upsets League Leaders",
        "Controversial Referee Decision Sparks Debate",
        "New Stadium Construction Begins",
        "Player Retires After Legendary Career",
        "Youth Academy Produces Rising Stars"
    };

    public SportsNewsAgency(Core.EventBus eventBus) 
        : base("SportsWire", eventBus)
    {
    }

    public override async Task StartPublishingAsync(CancellationToken cancellationToken)
    {
        IsPublishing = true;
        Console.WriteLine($"{AgencyName} started publishing sports news");

        while (IsPublishing && !cancellationToken.IsCancellationRequested)
        {
            var headline = _sportsHeadlines[Random.Next(_sportsHeadlines.Length)];
            var content = $"Full coverage of: {headline}. More details to follow...";
            
            PublishArticle(headline, content, NewsCategory.Sports);

            // Occasionally publish breaking sports news
            if (Random.NextDouble() < 0.2)
            {
                PublishBreakingNews(
                    "Major Sports Event Happening Now!",
                    "Live coverage of unprecedented sporting event...",
                    NewsCategory.Sports,
                    8
                );
            }

            await Task.Delay(Random.Next(3000, 6000), cancellationToken);
        }
    }
}
