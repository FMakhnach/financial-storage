using FinancialStorage.Api.Domain.Entities;
using FinancialStorage.Api.Domain.Models;

namespace FinancialStorage.Api.Domain.Repositories;

public interface IKeyRateRepository
{
    Task<IReadOnlyCollection<KeyRate>> GetAtMomentAsync(
        string countryKey,
        string? sourceName,
        DateTimeOffset moment,
        CancellationToken ct);

    Task<IReadOnlyCollection<KeyRate>> SearchAsync(SearchKeyRateModel model, CancellationToken ct);

    Task ConfirmAsync(long keyRateId, CancellationToken ct);

    Task UpdateAsync(KeyRate keyRate, CancellationToken ct);
}