using FinancialStorage.Api.Domain.Entities;
using FinancialStorage.Api.Domain.Models;

namespace FinancialStorage.Api.Services.Interfaces;

public interface IKeyRateService
{
    Task<IReadOnlyCollection<KeyRate>> GetAsync(string country, DateTimeOffset moment, CancellationToken ct);

    Task<IReadOnlyCollection<KeyRate>> SearchAsync(SearchKeyRateModel model, CancellationToken ct);

    Task UpdateAsync(IReadOnlyCollection<UpdateKeyRateModel> models, CancellationToken ct);
}