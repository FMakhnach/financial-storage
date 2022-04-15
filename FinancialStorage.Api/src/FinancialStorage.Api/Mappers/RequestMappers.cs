using FinancialStorage.Api.Controllers.v1.Requests;
using FinancialStorage.Api.Domain.Models;

namespace FinancialStorage.Api.Mappers;

public static class RequestMappers
{
    public static SearchKeyRateModel ToSearchModel(this SearchKeyRatesRequest request)
    {
        return new SearchKeyRateModel(request.Countries, request.Sources, request.Start, request.End);
    }

    public static UpdateKeyRateModel ToUpdateModel(this UpdateKeyRatesRequestItem item)
    {
        return new UpdateKeyRateModel
        {
            Country = item.Country,
            InformationSource = item.InformationSource,
            Moment = item.Moment,
            Value = item.Value,
        };
    }

    public static SearchDividendModel ToSearchModel(this SearchDividendsRequest request)
    {
        return new SearchDividendModel(request.Tickers, request.Sources, request.Start, request.End);
    }

    public static UpdateDividendModel ToUpdateModel(this UpdateDividendsRequestItem item)
    {
        return new UpdateDividendModel
        {
            Ticker = item.Ticker,
            Moment = item.Moment,
            InformationSource = item.InformationSource,
            AmountPerShare = item.AmountPerShare,
            Status = item.Status,
            AmountChangedPercent = item.AmountChangedPercent,
            SharePrice = item.SharePrice,
            Yield = item.Yield,
            DecDate = item.DecDate,
            ExDate = item.ExDate,
            PayDate = item.PayDate,
            Frequency = item.Frequency,
        };
    }
}