using System.Globalization;
using FinancialStorage.BackgroundWorkers.Application.WebScrapers.Extensions;
using FinancialStorage.BackgroundWorkers.Application.WebScrapers.Interfaces;
using FinancialStorage.BackgroundWorkers.Domain.Entities;
using FinancialStorage.BackgroundWorkers.Domain.Entities.InformationSources;
using FinancialStorage.BackgroundWorkers.Domain.Enums;
using HtmlAgilityPack;
using JetBrains.Annotations;

namespace FinancialStorage.BackgroundWorkers.Application.WebScrapers;

[UsedImplicitly]
public class A2FinanceScraper : IA2FinanceScraper
{
    public async Task<Dividend> ScrapAsync(Source<DividendSourceParams> source, CancellationToken ct)
    {
        var client = new HttpClient();
        client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0");
        var html = await client.GetStringAsync(source.Params.PageUrl, ct);

        var htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(html);

        var navigator = htmlDoc.DocumentNode.CreateNavigator()!;

        const string xpath = "//tr[@class=' ']/td";

        var items = navigator.EvaluateAllOccurrences(xpath).Select(x => x.Value).ToArray();

        var dividend = new Dividend
        {
            Ticker = source.Params.Ticker,
            SourceId = source.Id,
            AmountPerShare = decimal.Parse(items[5][..^2].Replace(',', '.'), CultureInfo.InvariantCulture),
            Status = DividendStatus.Declared,
            AmountChangedPercent = null,
            SharePrice = null,
            Yield = decimal.Parse(items[6][..^1].Replace(',', '.'), CultureInfo.InvariantCulture),
            DecDate = items[0] == "-" ? null : DateTime.Parse(items[0], CultureInfo.InvariantCulture, DateTimeStyles.None),
            ExDate = items[0] == "-" ? null : DateTime.Parse(items[1], CultureInfo.InvariantCulture, DateTimeStyles.None),
            PayDate = DateTime.Parse(items[3], CultureInfo.InvariantCulture, DateTimeStyles.None),
            Frequency = Enum.Parse<DividendFrequency>(items[4]),
            StartedAt = DateTimeOffset.UtcNow,
            LastConfirmedAt = DateTimeOffset.UtcNow,
        };

        return dividend;
    }
}