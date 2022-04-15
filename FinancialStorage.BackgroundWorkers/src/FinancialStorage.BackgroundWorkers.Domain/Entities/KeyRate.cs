namespace FinancialStorage.BackgroundWorkers.Domain.Entities;

public class KeyRate : IEquatable<KeyRate>
{
    public long Id { get; init; }

    public string CountryKey { get; init; }

    public long SourceId { get; init; }

    public DateTimeOffset StartedAt { get; init; }

    public DateTimeOffset LastConfirmedAt { get; init; }

    public decimal Value { get; init; }

    public bool Equals(KeyRate? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return CountryKey == other.CountryKey && SourceId == other.SourceId && Value == other.Value;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((KeyRate)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(CountryKey, SourceId, Value);
    }
}