using EventBus.Samples.NewsAgency.Events;

namespace EventBus.Samples.NewsAgency.Agencies;

public class CultureNewsAgency : BaseNewsAgency
{
    private readonly string[] _cultureHeadlines = new[]
    {
        "Art Exhibition Opens to Acclaim",
        "New Film Premieres at Festival",
        "Music Concert Series Announced",
        "Literary Award Winners Revealed",
        "Theatre Production Receives Standing Ovation",
        "Cultural Heritage Site Restored",
        "Artist Retrospective Exhibition",
        "Documentary Explores Cultural Identity"
    };

    public CultureNewsAgency(Core.EventBus eventBus) 
        : base("CultureBeat", eventBus)
    {
    }

    public override async Task StartPublishingAsync(CancellationToken cancellationToken)
    {
        IsPublishing = true;
        Console.WriteLine($"{AgencyName} started publishing cultural news");

        while (IsPublishing && !cancellationToken.IsCancellationRequested)
        {
            var headline = _cultureHeadlines[Random.Next(_cultureHeadlines.Length)];
            var content = $"Cultural coverage: {headline}. Reviews and interviews available...";
            
            PublishArticle(headline, content, NewsCategory.Culture);

            await Task.Delay(Random.Next(4000, 8000), cancellationToken);
        }
    }
}
