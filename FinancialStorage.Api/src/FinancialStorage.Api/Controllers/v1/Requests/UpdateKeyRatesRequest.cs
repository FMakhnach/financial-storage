namespace FinancialStorage.Api.Controllers.v1.Requests;

public class UpdateKeyRatesRequestItem
{
    public string Country { get; init; }

    public DateTimeOffset Moment { get; init; }

    public string InformationSource { get; init; }

    public decimal Value { get; init; }
}

public class UpdateKeyRatesRequest
{
    public IReadOnlyCollection<UpdateKeyRatesRequestItem> Items { get; init; }
}