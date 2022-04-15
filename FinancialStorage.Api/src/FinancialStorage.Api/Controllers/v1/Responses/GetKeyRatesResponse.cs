namespace FinancialStorage.Api.Controllers.v1.Responses;

public class GetKeyRatesResponse
{
    public IReadOnlyCollection<KeyRateResponseItem> Items { get; init; }
}