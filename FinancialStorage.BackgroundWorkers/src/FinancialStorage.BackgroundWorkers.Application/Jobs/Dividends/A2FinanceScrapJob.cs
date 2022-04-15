using System.ComponentModel;
using System.Transactions;
using FinancialStorage.BackgroundWorkers.Application.WebScrapers.Interfaces;
using FinancialStorage.BackgroundWorkers.Domain.Entities;
using FinancialStorage.BackgroundWorkers.Domain.Entities.InformationSources;
using FinancialStorage.BackgroundWorkers.Domain.Repositories;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace FinancialStorage.BackgroundWorkers.Application.Jobs.Dividends;

[UsedImplicitly]
public class A2FinanceScrapJob : IRecurringJob
{
    private readonly ILogger<A2FinanceScrapJob> _logger;
    private readonly IDividendRepository _dividendRepository;
    private readonly IInformationSourceRepository _informationSourceRepository;
    private readonly IA2FinanceScraper _scraper;

    private const string SourceName = "a2-finance.com";

    public A2FinanceScrapJob(
        ILogger<A2FinanceScrapJob> logger,
        IDividendRepository dividendRepository,
        IInformationSourceRepository informationSourceRepository,
        IA2FinanceScraper scraper)
    {
        _logger = logger;
        _dividendRepository = dividendRepository;
        _informationSourceRepository = informationSourceRepository;
        _scraper = scraper;
    }

    public string JobId => "dividend-scrap-a2-finance-com";

    public string Cron => "5 * * * *";

    [DisplayName("Scrap dividends for companies from a2-finance.com")]
    public async Task ProcessAsync(CancellationToken ct)
    {
        var dividendSources = await _informationSourceRepository.GetSourcesAsync<DividendSourceParams>(SourceName, ct);

        foreach (var dividendSource in dividendSources)
        {
            try
            {
                var dividend = await _scraper.ScrapAsync(dividendSource, ct);

                await UpdateDividendAsync(dividend, ct);
            }
            catch (Exception e)
            {
                _logger.LogError(e,
                    "Error on scraping dividend (company: {Ticker}, source: {SourceName})",
                    dividendSource.Params.Ticker,
                    dividendSource.SourceName);
            }
        }
    }

    private async Task UpdateDividendAsync(Dividend dividend, CancellationToken ct)
    {
        using var scope = new TransactionScope(
            TransactionScopeOption.Required,
            new TransactionOptions
            {
                IsolationLevel = IsolationLevel.ReadCommitted
            }, 
            TransactionScopeAsyncFlowOption.Enabled);

        var lastDividend = await _dividendRepository.GetLatestAsync(dividend.Ticker, dividend.SourceId, ct);

        if (lastDividend is not null)
        {
            await _dividendRepository.ConfirmAsync(lastDividend.Id, ct);
        }

        if (!dividend.Equals(lastDividend))
        {
            await _dividendRepository.UpdateAsync(dividend, ct);
        }

        scope.Complete();
    }
}