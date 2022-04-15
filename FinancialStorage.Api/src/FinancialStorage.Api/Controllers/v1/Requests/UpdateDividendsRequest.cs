using FinancialStorage.Api.Domain.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace FinancialStorage.Api.Controllers.v1.Requests;

public class UpdateDividendsRequestItem
{
    public string Ticker { get; init; }

    public DateTimeOffset Moment { get; init; }

    public string InformationSource { get; init; }

    public decimal AmountPerShare { get; init; }

    public DividendStatus Status { get; init; }

    public decimal? AmountChangedPercent { get; init; }

    public decimal? SharePrice { get; init; }

    public decimal? Yield { get; init; }

    public DateTime? DecDate { get; init; }

    public DateTime? ExDate { get; init; }

    public DateTime? PayDate { get; init; }

    [JsonConverter(typeof(StringEnumConverter))]
    public DividendFrequency? Frequency { get; init; }
}

public class UpdateDividendsRequest
{
    public IReadOnlyCollection<UpdateDividendsRequestItem> Items { get; init; }
}