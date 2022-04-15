using Npgsql;

namespace FinancialStorage.BackgroundWorkers.DataAccess.Extensions;

public static class SqlMappers
{
    public static string ToPgsqlEnumString<TEnum>(this TEnum enumValue) where TEnum : Enum
    {
        return NpgsqlConnection.GlobalTypeMapper.DefaultNameTranslator.TranslateMemberName(enumValue.ToString());
    }
}