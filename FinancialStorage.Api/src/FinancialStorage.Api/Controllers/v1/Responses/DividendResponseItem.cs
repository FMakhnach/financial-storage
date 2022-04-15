using FinancialStorage.Api.Domain.Enums;

namespace FinancialStorage.Api.Controllers.v1.Responses;

public class DividendResponseItem
{
    public string Ticker { get; init; }

    public string SourceName { get; init; }

    public DateTimeOffset StartedAt { get; init; }

    public DateTimeOffset LastConfirmedAt { get; init; }

    public decimal AmountPerShare { get; init; }

    public DividendStatus Status { get; init; }

    public decimal? AmountChangedPercent { get; init; }

    public decimal? SharePrice { get; init; }

    public decimal? Yield { get; init; }

    public DateTime? DecDate { get; init; }

    public DateTime? ExDate { get; init; }

    public DateTime? PayDate { get; init; }

    public DividendFrequency? Frequency { get; init; }
}