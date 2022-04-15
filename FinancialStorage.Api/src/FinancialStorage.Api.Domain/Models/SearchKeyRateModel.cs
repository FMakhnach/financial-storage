namespace FinancialStorage.Api.Domain.Models;

public class SearchKeyRateModel
{
    public IReadOnlyCollection<string> Countries { get; }

    public IReadOnlyCollection<string>? Sources { get; }

    public DateTimeOffset? Start { get; }

    public DateTimeOffset? End { get; }

    public SearchKeyRateModel(
        IReadOnlyCollection<string> countries,
        IReadOnlyCollection<string>? sources,
        DateTimeOffset? start,
        DateTimeOffset? end)
    {
        Countries = countries;
        Sources = sources is { Count: > 0 } ? sources : null;
        Start = start;
        End = end;
    }
}