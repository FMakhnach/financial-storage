namespace FinancialStorage.Api.Domain.Models;

public class SearchDividendModel
{
    public IReadOnlyCollection<string> Tickers { get; }

    public IReadOnlyCollection<string>? Sources { get; }

    public DateTimeOffset? Start { get; }

    public DateTimeOffset? End { get; }

    public SearchDividendModel(
        IReadOnlyCollection<string> tickers,
        IReadOnlyCollection<string>? sources,
        DateTimeOffset? start,
        DateTimeOffset? end)
    {
        Tickers = tickers;
        Sources = sources is { Count: > 0 } ? sources : null;
        Start = start;
        End = end;
    }
}