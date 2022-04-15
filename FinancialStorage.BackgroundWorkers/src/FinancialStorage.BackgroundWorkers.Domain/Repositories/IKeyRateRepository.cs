using FinancialStorage.BackgroundWorkers.Domain.Entities;

namespace FinancialStorage.BackgroundWorkers.Domain.Repositories;

public interface IKeyRateRepository
{
    Task<KeyRate?> GetLatestAsync(string countryKey, long sourceId, CancellationToken ct);

    Task ConfirmAsync(long keyRateId, CancellationToken ct);

    Task UpdateAsync(KeyRate keyRate, CancellationToken ct);
}