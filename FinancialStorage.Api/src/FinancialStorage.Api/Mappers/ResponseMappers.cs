using FinancialStorage.Api.Controllers.v1.Responses;
using FinancialStorage.Api.Domain.Entities;

namespace FinancialStorage.Api.Mappers;

public static class ResponseMappers
{
    public static KeyRateResponseItem ToResponseItem(this KeyRate keyRate)
    {
        return new KeyRateResponseItem
        {
            Country = keyRate.CountryKey,
            SourceName = keyRate.SourceName,
            StartedAt = keyRate.StartedAt,
            LastConfirmedAt = keyRate.LastConfirmedAt,
            Value = keyRate.Value,
        };
    }

    public static DividendResponseItem ToResponseItem(this Dividend keyRate)
    {
        return new DividendResponseItem
        {
            Ticker = keyRate.Ticker,
            SourceName = keyRate.SourceName,
            StartedAt = keyRate.StartedAt,
            LastConfirmedAt = keyRate.LastConfirmedAt,
            AmountPerShare = keyRate.AmountPerShare,
            Status = keyRate.Status,
            AmountChangedPercent = keyRate.AmountChangedPercent,
            SharePrice = keyRate.SharePrice,
            Yield = keyRate.Yield,
            DecDate = keyRate.DecDate,
            ExDate = keyRate.ExDate,
            PayDate = keyRate.PayDate,
            Frequency = keyRate.Frequency,
        };
    }
}