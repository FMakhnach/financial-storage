using System.ComponentModel;
using System.Transactions;
using FinancialStorage.BackgroundWorkers.Application.WebScrapers.Interfaces;
using FinancialStorage.BackgroundWorkers.Domain.Entities;
using FinancialStorage.BackgroundWorkers.Domain.Entities.InformationSources;
using FinancialStorage.BackgroundWorkers.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace FinancialStorage.BackgroundWorkers.Application.Jobs.KeyRates;

public class RuInvestingComKeyRateScrapJob : IRecurringJob
{
    private readonly ILogger<RuInvestingComKeyRateScrapJob> _logger;
    private readonly IKeyRateRepository _keyRateRepository;
    private readonly IInformationSourceRepository _informationSourceRepository;
    private readonly IRuInvestingComScraper _scraper;

    private const string SourceName = "ru.investing.com";

    public RuInvestingComKeyRateScrapJob(
        ILogger<RuInvestingComKeyRateScrapJob> logger,
        IKeyRateRepository keyRateRepository,
        IInformationSourceRepository informationSourceRepository,
        IRuInvestingComScraper scraper)
    {
        _logger = logger;
        _keyRateRepository = keyRateRepository;
        _informationSourceRepository = informationSourceRepository;
        _scraper = scraper;
    }

    public string JobId => "key-rates-scrap-ru-investing-com";

    public string Cron => "0 * * * *";

    [DisplayName("Scrap key rates for countries from ru.investing.com")]
    public async Task ProcessAsync(CancellationToken ct)
    {
        var keyRateSources = await _informationSourceRepository.GetSourcesAsync<KeyRateSourceParams>(SourceName, ct);

        foreach (var keyRateSource in keyRateSources)
        {
            try
            {
                var keyRate = await _scraper.ScrapAsync(keyRateSource, ct);

                await UpdateKeyRateAsync(keyRate, ct);
            }
            catch (Exception e)
            {
                _logger.LogError(e,
                    "Error on scraping key rate (country: {CountryKey}, source: {SourceName})",
                    keyRateSource.Params.CountryKey,
                    keyRateSource.SourceName);
            }
        }
    }

    private async Task UpdateKeyRateAsync(KeyRate keyRate, CancellationToken ct)
    {
        using var scope = new TransactionScope(
            TransactionScopeOption.Required,
            new TransactionOptions
            {
                IsolationLevel = IsolationLevel.ReadCommitted,
            }, 
            TransactionScopeAsyncFlowOption.Enabled);

        var lastKeyRate = await _keyRateRepository.GetLatestAsync(keyRate.CountryKey, keyRate.SourceId, ct);

        if (lastKeyRate is not null)
        {
            await _keyRateRepository.ConfirmAsync(lastKeyRate.Id, ct);
        }

        if (!keyRate.Equals(lastKeyRate))
        {
            await _keyRateRepository.UpdateAsync(keyRate, ct);
        }

        scope.Complete();
    }
}