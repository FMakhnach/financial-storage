namespace FinancialStorage.Api.Controllers.v1.Responses;

public class GetDividendsResponse
{
    public IReadOnlyCollection<DividendResponseItem> Items { get; init; }
}