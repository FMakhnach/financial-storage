using Dapper;
using FinancialStorage.BackgroundWorkers.DataAccess.Infrastructure;
using FinancialStorage.BackgroundWorkers.Domain.Entities.InformationSources;
using FinancialStorage.BackgroundWorkers.Domain.Enums;
using Npgsql;

namespace FinancialStorage.BackgroundWorkers.DataAccess.Configuration;

public static class PostgresConfiguration
{
    public static void ConfigureDatabaseMappings()
    {
        DefaultTypeMap.MatchNamesWithUnderscores = true;
        NpgsqlConnection.GlobalTypeMapper.UseJsonNet();

        NpgsqlConnection.GlobalTypeMapper.MapEnum<Currency>("public.currency", new UpperCaseNameTranslator());
        NpgsqlConnection.GlobalTypeMapper.MapEnum<DividendFrequency>("public.dividend_frequency");
        NpgsqlConnection.GlobalTypeMapper.MapEnum<DividendStatus>("public.dividend_status");

        SqlMapper.AddTypeHandler(typeof(KeyRateSourceParams), new JsonTypeHandler<KeyRateSourceParams>());
        SqlMapper.AddTypeHandler(typeof(DividendSourceParams), new JsonTypeHandler<DividendSourceParams>());
    }
}