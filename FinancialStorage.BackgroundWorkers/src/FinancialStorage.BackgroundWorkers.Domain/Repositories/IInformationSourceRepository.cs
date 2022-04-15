using FinancialStorage.BackgroundWorkers.Domain.Entities.InformationSources;

namespace FinancialStorage.BackgroundWorkers.Domain.Repositories;

public interface IInformationSourceRepository
{
    Task<IReadOnlyList<Source<T>>> GetSourcesAsync<T>(string sourceName, CancellationToken ct);
}