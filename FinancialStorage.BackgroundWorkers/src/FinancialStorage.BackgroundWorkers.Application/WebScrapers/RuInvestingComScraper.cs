using System.Globalization;
using FinancialStorage.BackgroundWorkers.Application.WebScrapers.Interfaces;
using FinancialStorage.BackgroundWorkers.Domain.Entities;
using FinancialStorage.BackgroundWorkers.Domain.Entities.InformationSources;
using FinancialStorage.BackgroundWorkers.Domain.Exceptions;
using HtmlAgilityPack;

namespace FinancialStorage.BackgroundWorkers.Application.WebScrapers;

public class RuInvestingComScraper : IRuInvestingComScraper
{
    private const string XPath = "substring-before(substring-after(//p[text()='Текущая ставка:']/../text(), '\n'), '%')";

    public async Task<KeyRate> ScrapAsync(Source<KeyRateSourceParams> source, CancellationToken ct)
    {
        var client = new HttpClient();
        client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0");
        var html = await client.GetStringAsync(source.Params.PageUrl, ct);

        var htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(html);

        var navigator = htmlDoc.DocumentNode.CreateNavigator()!;
        var expression = navigator.Compile(XPath);

        if (navigator.Evaluate(expression) is not string valueText)
        {
            throw new DomainException("Failed to find data");
        }

        if (!decimal.TryParse(valueText.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out var value))
        {
            throw new DomainException($"Failed to parse data: {valueText}");
        }

        var keyRate = new KeyRate
        {
            CountryKey = source.Params.CountryKey,
            SourceId = source.Id,
            Value = value,
            StartedAt = DateTimeOffset.UtcNow,
            LastConfirmedAt = DateTimeOffset.UtcNow,
        };

        return keyRate;
    }
}