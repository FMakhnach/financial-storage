using FinancialStorage.Api.Domain.Entities;

namespace FinancialStorage.Api.Domain.Repositories;

public interface IInformationSourceRepository
{
    Task<InformationSource?> GetByNameAsync(string sourceName, CancellationToken ct);

    Task<InformationSource> CreateAsync(string sourceName, CancellationToken ct);
}