using FinancialStorage.BackgroundWorkers.Domain.Enums;

namespace FinancialStorage.BackgroundWorkers.Domain.Entities;

public class Dividend : IEquatable<Dividend>
{
    public long Id { get; init; }

    public string Ticker { get; init; }

    public long SourceId { get; init; }

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

    public bool Equals(Dividend? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Ticker == other.Ticker
               && SourceId == other.SourceId
               && AmountPerShare == other.AmountPerShare
               && Status == other.Status
               && AmountChangedPercent == other.AmountChangedPercent
               && SharePrice == other.SharePrice
               && Yield == other.Yield
               && Nullable.Equals(DecDate, other.DecDate)
               && Nullable.Equals(ExDate, other.ExDate)
               && Nullable.Equals(PayDate, other.PayDate)
               && Nullable.Equals(Frequency, other.Frequency);
    }

    public override bool Equals(object? other)
    {
        return Equals(other as Dividend);
    }

    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        hashCode.Add(Ticker);
        hashCode.Add(SourceId);
        hashCode.Add(AmountPerShare);
        hashCode.Add((int)Status);
        hashCode.Add(AmountChangedPercent);
        hashCode.Add(SharePrice);
        hashCode.Add(Yield);
        hashCode.Add(DecDate);
        hashCode.Add(ExDate);
        hashCode.Add(PayDate);
        hashCode.Add((int?)Frequency);
        return hashCode.ToHashCode();
    }
}