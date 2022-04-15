using FinancialStorage.Api.Domain.Entities;
using FinancialStorage.Api.Domain.Models;

namespace FinancialStorage.Api.Domain.Repositories;

public interface IDividendRepository
{
    Task<IReadOnlyCollection<Dividend>> GetAtMomentAsync(
        string ticker,
        string? sourceName,
        DateTimeOffset moment,
        CancellationToken ct);

    Task<IReadOnlyCollection<Dividend>> SearchAsync(SearchDividendModel model, CancellationToken ct);

    Task ConfirmAsync(long dividendId, CancellationToken ct);

    Task UpdateAsync(Dividend dividend, CancellationToken ct);
}