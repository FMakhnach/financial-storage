namespace FinancialStorage.Api.Domain.Models;

public class UpdateKeyRateModel
{
    public string Country { get; init; }

    public DateTimeOffset Moment { get; init; }

    public string InformationSource { get; init; }

    public decimal Value { get; init; }
}