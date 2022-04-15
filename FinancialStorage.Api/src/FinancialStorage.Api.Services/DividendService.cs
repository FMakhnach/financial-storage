using System.Transactions;
using FinancialStorage.Api.Domain.Entities;
using FinancialStorage.Api.Domain.Models;
using FinancialStorage.Api.Domain.Repositories;
using FinancialStorage.Api.Services.Interfaces;
using JetBrains.Annotations;

namespace FinancialStorage.Api.Services;

[UsedImplicitly]
public class DividendService : IDividendService
{
    private readonly IInformationSourceService _informationSourceService;

    private readonly IDividendRepository _dividendRepository;

    public DividendService(IInformationSourceService informationSourceService, IDividendRepository dividendRepository)
    {
        _informationSourceService = informationSourceService;
        _dividendRepository = dividendRepository;
    }

    public Task<IReadOnlyCollection<Dividend>> GetAsync(string ticker, DateTimeOffset moment, CancellationToken ct)
    {
        return _dividendRepository.GetAtMomentAsync(ticker, null, moment, ct);
    }

    public Task<IReadOnlyCollection<Dividend>> SearchAsync(SearchDividendModel model, CancellationToken ct)
    {
        return _dividendRepository.SearchAsync(model, ct);
    }

    public async Task UpdateAsync(IReadOnlyCollection<UpdateDividendModel> models, CancellationToken ct)
    {
        foreach (var model in models)
        {
            var infoSource = await _informationSourceService.GetOrCreateInformationSourceAsync(model.InformationSource, ct);

            var dividend = new Dividend
            {
                Ticker = model.Ticker,
                SourceId = infoSource.Id,
                SourceName = infoSource.Name,
                StartedAt = model.Moment,
                LastConfirmedAt = model.Moment,
                AmountPerShare = model.AmountPerShare,
                Status = model.Status,
                AmountChangedPercent = model.AmountChangedPercent,
                SharePrice = model.SharePrice,
                Yield = model.Yield,
                DecDate = model.DecDate,
                ExDate = model.ExDate,
                PayDate = model.PayDate,
                Frequency = model.Frequency,
            };

            await UpdateAsync(dividend, ct);
        }
    }

    private async Task UpdateAsync(Dividend keyRate, CancellationToken ct)
    {
        using var scope = new TransactionScope(
            TransactionScopeOption.Required,
            new TransactionOptions
            {
                IsolationLevel = IsolationLevel.ReadCommitted
            },
            TransactionScopeAsyncFlowOption.Enabled);

        var keyRates = await _dividendRepository.GetAtMomentAsync(keyRate.Ticker, keyRate.SourceName, DateTimeOffset.UtcNow, ct);
        var lastKeyRate = keyRates.SingleOrDefault();

        if (lastKeyRate is not null)
        {
            await _dividendRepository.ConfirmAsync(lastKeyRate.Id, ct);
        }

        if (!keyRate.Equals(lastKeyRate))
        {
            await _dividendRepository.UpdateAsync(keyRate, ct);
        }

        scope.Complete();
    }
}