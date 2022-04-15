using System.Transactions;
using FinancialStorage.Api.Domain.Entities;
using FinancialStorage.Api.Domain.Models;
using FinancialStorage.Api.Domain.Repositories;
using FinancialStorage.Api.Services.Interfaces;

namespace FinancialStorage.Api.Services;

public class KeyRateService : IKeyRateService
{
    private readonly IInformationSourceService _informationSourceService;

    private readonly IKeyRateRepository _keyRateRepository;

    public KeyRateService(IInformationSourceService informationSourceService, IKeyRateRepository keyRateRepository)
    {
        _informationSourceService = informationSourceService;
        _keyRateRepository = keyRateRepository;
    }

    public Task<IReadOnlyCollection<KeyRate>> GetAsync(string country, DateTimeOffset moment, CancellationToken ct)
    {
        return _keyRateRepository.GetAtMomentAsync(country, null, moment, ct);
    }

    public Task<IReadOnlyCollection<KeyRate>> SearchAsync(SearchKeyRateModel model, CancellationToken ct)
    {
        return _keyRateRepository.SearchAsync(model, ct);
    }

    public async Task UpdateAsync(IReadOnlyCollection<UpdateKeyRateModel> models, CancellationToken ct)
    {
        foreach (var model in models)
        {
            var infoSource = await _informationSourceService.GetOrCreateInformationSourceAsync(model.InformationSource, ct);

            var keyRate = new KeyRate
            {
                CountryKey = model.Country,
                SourceId = infoSource.Id,
                SourceName = infoSource.Name,
                StartedAt = model.Moment,
                LastConfirmedAt = model.Moment,
                Value = model.Value,
            };

            await UpdateAsync(keyRate, ct);
        }
    }

    private async Task UpdateAsync(KeyRate keyRate, CancellationToken ct)
    {
        using var scope = new TransactionScope(
            TransactionScopeOption.Required,
            new TransactionOptions
            {
                IsolationLevel = IsolationLevel.ReadCommitted
            }, 
            TransactionScopeAsyncFlowOption.Enabled);

        var keyRates = await _keyRateRepository.GetAtMomentAsync(keyRate.CountryKey, keyRate.SourceName, DateTimeOffset.UtcNow, ct);
        var lastKeyRate = keyRates.SingleOrDefault();        
        
        if (lastKeyRate is not null)
        {
            await _keyRateRepository.ConfirmAsync(lastKeyRate.Id, ct);
        }

        if (!keyRate.Equals(lastKeyRate))
        {
            await _keyRateRepository.UpdateAsync(keyRate, ct);
        }

        scope.Complete();
    }
}