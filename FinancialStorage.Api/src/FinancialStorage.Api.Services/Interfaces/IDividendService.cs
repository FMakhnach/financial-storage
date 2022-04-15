using FinancialStorage.Api.Domain.Entities;
using FinancialStorage.Api.Domain.Models;

namespace FinancialStorage.Api.Services.Interfaces;

public interface IDividendService
{
    Task<IReadOnlyCollection<Dividend>> GetAsync(string ticker, DateTimeOffset moment, CancellationToken ct);

    Task<IReadOnlyCollection<Dividend>> SearchAsync(SearchDividendModel model, CancellationToken ct);

    Task UpdateAsync(IReadOnlyCollection<UpdateDividendModel> models, CancellationToken ct);
}