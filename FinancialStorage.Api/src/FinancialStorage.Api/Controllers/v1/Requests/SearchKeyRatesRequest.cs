namespace FinancialStorage.Api.Controllers.v1.Requests;

public class SearchKeyRatesRequest
{
    /// <summary>
    /// 3-char country keys (ISO 4217).
    /// </summary>
    public IReadOnlyCollection<string> Countries { get; init; }
    
    /// <summary>
    /// Information source names.
    /// </summary>
    public IReadOnlyCollection<string>? Sources { get; init; }

    /// <summary>
    /// Start of the interval. Counts as Start Of The Time if empty.
    /// </summary>
    public DateTimeOffset? Start { get; init; }

    /// <summary>
    /// End of the interval. Counts as now if empty.
    /// </summary>
    public DateTimeOffset? End { get; init; }
}