using System.Globalization;
using System.Xml.XPath;
using FinancialStorage.BackgroundWorkers.Application.WebScrapers.Extensions;
using FinancialStorage.BackgroundWorkers.Application.WebScrapers.Interfaces;
using FinancialStorage.BackgroundWorkers.Domain.Entities;
using FinancialStorage.BackgroundWorkers.Domain.Entities.InformationSources;
using FinancialStorage.BackgroundWorkers.Domain.Enums;
using FinancialStorage.BackgroundWorkers.Domain.Exceptions;
using HtmlAgilityPack;
using JetBrains.Annotations;

namespace FinancialStorage.BackgroundWorkers.Application.WebScrapers;

[UsedImplicitly]
public class DividendComScraper : IDividendComScraper
{
    public async Task<Dividend> ScrapAsync(Source<DividendSourceParams> source, CancellationToken ct)
    {
        var client = new HttpClient();
        client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0");
        var html = await client.GetStringAsync(source.Params.PageUrl, ct);

        var htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(html);

        var navigator = htmlDoc.DocumentNode.CreateNavigator()!;

        var dividend = new Dividend
        {
            Ticker = source.Params.Ticker,
            SourceId = source.Id,
            AmountPerShare = GetAmountPerShare(navigator),
            Status = GetStatus(navigator),
            AmountChangedPercent = GetAmountChangePercent(navigator),
            SharePrice = GetSharePrice(navigator),
            Yield = GetYield(navigator),
            DecDate = null,
            ExDate = GetExDate(navigator),
            PayDate = GetPayDate(navigator),
            Frequency = GetFrequency(navigator),
            StartedAt = DateTimeOffset.UtcNow,
            LastConfirmedAt = DateTimeOffset.UtcNow,
        };

        return dividend;
    }

    private static decimal GetAmountPerShare(XPathNavigator navigator)
    {
        const string xpath = "//div[text()='\nDividend (Fwd)\n']/following-sibling::div";

        var rawText = navigator.EvaluateFirstText(xpath);

        if (rawText is null || !decimal.TryParse(rawText[2..^1], NumberStyles.Any, CultureInfo.InvariantCulture, out var value))
        {
            throw new DomainException($"Failed to parse data: {rawText}");
        }

        return value;
    }

    private static DividendStatus GetStatus(XPathNavigator navigator)
    {
        const string xpath = "//div[@class='t-text-black t-font-semibold t-text-xs']";

        var rawText = navigator.EvaluateFirstText(xpath);

        if (rawText is null || !Enum.TryParse<DividendStatus>(rawText, out var value))
        {
            return DividendStatus.Estimated;
        }

        return value;
    }

    private static decimal? GetAmountChangePercent(XPathNavigator navigator)
    {
        const string xpath = "//div[@class='t-flex t-text-3xl t-font-semibold t-leading-none md:t-w-28']";

        var rawText = navigator.EvaluateFirstText(xpath);

        if (rawText is null || !decimal.TryParse(rawText[..^1], NumberStyles.Any, CultureInfo.InvariantCulture, out var value))
        {
            return null;
        }

        return value;
    }

    private static decimal? GetSharePrice(XPathNavigator navigator)
    {
        const string xpath = "//div[@class='t-text-xl t-font-semibold t-flex t-flex-row t-items-center t-h-full t-leading-tighter']/text()";

        var rawText = navigator.EvaluateFirstText(xpath);

        if (rawText is null || !decimal.TryParse(rawText[2..^1], NumberStyles.Any, CultureInfo.InvariantCulture, out var value))
        {
            return null;
        }

        return value;
    }

    private static decimal? GetYield(XPathNavigator navigator)
    {
        const string xpath = "//div[text()='\nYield (Fwd)\n']/following-sibling::div";

        var rawText = navigator.EvaluateFirstText(xpath);

        if (rawText is null || !decimal.TryParse(rawText[1..^2], NumberStyles.Any, CultureInfo.InvariantCulture, out var value))
        {
            return null;
        }

        return value;
    }

    private static DateTime? GetExDate(XPathNavigator navigator)
    {
        const string xpath = "//div[@class='shotclock t-my-3 xl:t-my-0']/@data-ex-date";

        var rawText = navigator.EvaluateFirstText(xpath);

        if (rawText is null || !DateTime.TryParse(rawText, CultureInfo.InvariantCulture, DateTimeStyles.None, out var value))
        {
            return null;
        }

        return value;
    }

    private static DateTime? GetPayDate(XPathNavigator navigator)
    {
        const string xpath = "//div[text()='Next Pay Date']/following-sibling::div";

        var rawText = navigator.EvaluateFirstText(xpath);

        if (rawText is null || !DateTime.TryParse(rawText, CultureInfo.InvariantCulture, DateTimeStyles.None, out var value))
        {
            return null;
        }

        return value;
    }

    private static DividendFrequency? GetFrequency(XPathNavigator navigator)
    {
        const string xpath = "//div[text()='Dividend Frequency']/following-sibling::div";

        var rawText = navigator.EvaluateFirstText(xpath);

        if (rawText is null || !Enum.TryParse<DividendFrequency>(rawText, out var value))
        {
            return null;
        }

        return value;
    }
}