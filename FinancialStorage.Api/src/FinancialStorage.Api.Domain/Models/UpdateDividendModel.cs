using FinancialStorage.Api.Domain.Enums;

namespace FinancialStorage.Api.Domain.Models;

public class UpdateDividendModel
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

    public DividendFrequency? Frequency { get; init; }
}