using FinancialStorage.BackgroundWorkers.Domain.Enums;

namespace FinancialStorage.BackgroundWorkers.Domain.Mappers;

public static class EnumMappers
{
    public static Currency ToCurrency(this char symbol)
    {
        return symbol switch
        {
            '$' => Currency.USD,
            '€' => Currency.EUR,
            '₽' => Currency.RUB,
            '¥' => Currency.CNY,
            _ => throw new ArgumentOutOfRangeException(nameof(symbol), symbol, null)
        };
    }
}