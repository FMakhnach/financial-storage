using Dapper;
using FinancialStorage.Api.Domain.Enums;
using FinancialStorage.BackgroundWorkers.DataAccess.Infrastructure;
using Npgsql;

namespace FinancialStorage.Api.DataAccess.Configuration;

public static class PostgresConfiguration
{
    public static void ConfigureDatabaseMappings()
    {
        DefaultTypeMap.MatchNamesWithUnderscores = true;

        NpgsqlConnection.GlobalTypeMapper.MapEnum<Currency>("public.currency", new UpperCaseNameTranslator());
        NpgsqlConnection.GlobalTypeMapper.MapEnum<DividendFrequency>("public.dividend_frequency");
        NpgsqlConnection.GlobalTypeMapper.MapEnum<DividendStatus>("public.dividend_status");
    }
}