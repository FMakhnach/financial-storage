using FinancialStorage.BackgroundWorkers.Domain.Entities;
using FinancialStorage.BackgroundWorkers.Domain.Entities.InformationSources;

namespace FinancialStorage.BackgroundWorkers.Application.WebScrapers.Interfaces;

public interface IRuInvestingComScraper
{
    Task<KeyRate> ScrapAsync(Source<KeyRateSourceParams> source, CancellationToken ct);
}