namespace FinancialStorage.Api.Controllers.v1.Responses;

public class KeyRateResponseItem
{
    public string Country { get; init; }

    public string SourceName { get; init; }

    public DateTimeOffset StartedAt { get; init; }

    public DateTimeOffset LastConfirmedAt { get; init; }

    public decimal Value { get; init; }
}