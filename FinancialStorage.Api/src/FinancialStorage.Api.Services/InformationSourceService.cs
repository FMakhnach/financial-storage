using System.Transactions;
using FinancialStorage.Api.Domain.Entities;
using FinancialStorage.Api.Domain.Repositories;
using FinancialStorage.Api.Services.Interfaces;
using JetBrains.Annotations;

namespace FinancialStorage.Api.Services;

[UsedImplicitly]
public class InformationSourceService : IInformationSourceService
{
    private readonly IInformationSourceRepository _informationSourceRepository;

    public InformationSourceService(IInformationSourceRepository informationSourceRepository)
    {
        _informationSourceRepository = informationSourceRepository;
    }

    public async Task<InformationSource> GetOrCreateInformationSourceAsync(string sourceName, CancellationToken ct)
    {
        using var scope = new TransactionScope(
            TransactionScopeOption.Required,
            new TransactionOptions
            {
                IsolationLevel = IsolationLevel.ReadCommitted
            }, 
            TransactionScopeAsyncFlowOption.Enabled);

        var existingSource = await _informationSourceRepository.GetByNameAsync(sourceName, ct);

        if (existingSource is not null)
        {
            return existingSource;
        }

        var newSource = await _informationSourceRepository.CreateAsync(sourceName, ct);

        scope.Complete();

        return newSource;
    }
}