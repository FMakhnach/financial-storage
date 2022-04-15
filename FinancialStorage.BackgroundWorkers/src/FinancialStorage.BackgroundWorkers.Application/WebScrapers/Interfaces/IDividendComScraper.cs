﻿using FinancialStorage.BackgroundWorkers.Domain.Entities;
using FinancialStorage.BackgroundWorkers.Domain.Entities.InformationSources;

namespace FinancialStorage.BackgroundWorkers.Application.WebScrapers.Interfaces;

public interface IDividendComScraper
{
    Task<Dividend> ScrapAsync(Source<DividendSourceParams> source, CancellationToken ct);
}