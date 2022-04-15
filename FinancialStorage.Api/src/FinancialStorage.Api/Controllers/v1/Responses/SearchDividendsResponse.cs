namespace FinancialStorage.Api.Controllers.v1.Responses;

public class SearchDividendsResponse
{
    public IReadOnlyCollection<DividendResponseItem> Items { get; init; }
}