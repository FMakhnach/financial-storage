namespace FinancialStorage.Api.Controllers.v1.Responses;

public class SearchKeyRatesResponse
{
    public IReadOnlyCollection<KeyRateResponseItem> Items { get; init; }
}