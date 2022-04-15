using FinancialStorage.Api.Domain.Entities;

namespace FinancialStorage.Api.Services.Interfaces;

public interface IInformationSourceService
{
    Task<InformationSource> GetOrCreateInformationSourceAsync(string sourceName, CancellationToken ct);
}