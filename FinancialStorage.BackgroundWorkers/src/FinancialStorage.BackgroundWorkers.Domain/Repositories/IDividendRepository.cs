using FinancialStorage.BackgroundWorkers.Domain.Entities;

namespace FinancialStorage.BackgroundWorkers.Domain.Repositories;

public interface IDividendRepository
{
    Task<Dividend?> GetLatestAsync(string ticker, long sourceId, CancellationToken ct);

    Task ConfirmAsync(long dividendId, CancellationToken ct);

    Task UpdateAsync(Dividend dividend, CancellationToken ct);
}