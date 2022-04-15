using Dapper;
using FinancialStorage.BackgroundWorkers.DataAccess.Configuration;
using FinancialStorage.BackgroundWorkers.Domain.Entities;
using FinancialStorage.BackgroundWorkers.Domain.Repositories;

namespace FinancialStorage.BackgroundWorkers.DataAccess.Repositories;

public class KeyRateRepository : IKeyRateRepository
{
    private readonly IConnectionProvider _connectionProvider;

    public KeyRateRepository(IConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider;
    }

    public async Task<KeyRate?> GetLatestAsync(string countryKey, long sourceId, CancellationToken ct)
    {
        const string query = @"
            SELECT id,
                   country_key,
                   source_id,
                   started_at,
                   last_confirmed_at,
                   value
            FROM public.key_rates
            WHERE country_key = :countryKey::text
              AND source_id = :sourceId::bigint
            ORDER BY last_confirmed_at;
        ";

        var parameters = new
        {
            countryKey,
            sourceId,
        };

        var command = new CommandDefinition(query, parameters, cancellationToken: ct);

        await using var connection = _connectionProvider.GetConnection();

        var keyRate = await connection.QueryFirstOrDefaultAsync<KeyRate>(command);

        return keyRate;
    }

    public async Task ConfirmAsync(long keyRateId, CancellationToken ct)
    {
        const string query = @"
            UPDATE public.key_rates
            SET last_confirmed_at = :lastConfirmedAt::timestamptz
            WHERE id = :id;
        ";

        var parameters = new
        {
            id = keyRateId,
            lastConfirmedAt = DateTimeOffset.UtcNow,
        };

        var command = new CommandDefinition(query, parameters, cancellationToken: ct);

        await using var connection = _connectionProvider.GetConnection();

        await connection.ExecuteAsync(command);
    }

    public async Task UpdateAsync(KeyRate keyRate, CancellationToken ct)
    {
        const string query = @"
            INSERT INTO public.key_rates (country_key,
                                          source_id,
                                          started_at,
                                          last_confirmed_at,
                                          value)
            VALUES (:countryKey::text,
                    :sourceId::bigint,
                    :startedAt::timestamptz,
                    :lastConfirmedAt::timestamptz,
                    :value::decimal);
        ";

        var parameters = new
        {
            countryKey = keyRate.CountryKey,
            sourceId = keyRate.SourceId,
            startedAt = keyRate.StartedAt,
            lastConfirmedAt = keyRate.LastConfirmedAt,
            value = keyRate.Value,
        };

        var command = new CommandDefinition(query, parameters, cancellationToken: ct);

        await using var connection = _connectionProvider.GetConnection();

        await connection.ExecuteAsync(command);
    }
}