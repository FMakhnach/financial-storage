using Dapper;
using FinancialStorage.Api.DataAccess.Configuration;
using FinancialStorage.Api.DataAccess.Extensions;
using FinancialStorage.Api.Domain.Entities;
using FinancialStorage.Api.Domain.Models;
using FinancialStorage.Api.Domain.Repositories;

namespace FinancialStorage.Api.DataAccess.Repositories;

public class KeyRateRepository : IKeyRateRepository
{
    private readonly IConnectionProvider _connectionProvider;

    public KeyRateRepository(IConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider;
    }

    public async Task<IReadOnlyCollection<KeyRate>> GetAtMomentAsync(
        string countryKey,
        string? sourceName,
        DateTimeOffset moment,
        CancellationToken ct)
    {
        const string query = @"
            SELECT ranked.id,
                   ranked.country_key,
                   ranked.source_id,
                   ranked.source_name,
                   ranked.started_at,
                   ranked.last_confirmed_at,
                   ranked.value
            FROM (SELECT kr.id,
                         kr.country_key,
                         kr.source_id,
                         inf.source_name,
                         kr.started_at,
                         kr.last_confirmed_at,
                         kr.value,
                         RANK() OVER (PARTITION BY kr.source_id ORDER BY kr.started_at DESC) AS rank
                  FROM public.key_rates AS kr
                           JOIN public.information_sources AS inf ON inf.id = kr.source_id
                  WHERE kr.country_key = :countryKey::text
                    AND (:sourceName IS NULL OR inf.source_name = :sourceName::text)
                    AND kr.started_at <= :moment) AS ranked
            WHERE ranked.rank = 1;
        ";

        var parameters = new
        {
            countryKey,
            sourceName,
            moment,
        };

        var command = new CommandDefinition(query, parameters, cancellationToken: ct);

        await using var connection = _connectionProvider.GetConnection();

        var keyRate = await connection.QueryAsync<KeyRate>(command);

        return keyRate.CastToReadOnlyList();
    }

    public async Task<IReadOnlyCollection<KeyRate>> SearchAsync(SearchKeyRateModel model, CancellationToken ct)
    {
        //language=sql
        const string query = @"
            WITH filtered_by_countries_and_sources AS (
                SELECT kr.*,
                       inf.source_name
                FROM public.key_rates AS kr
                    JOIN public.information_sources AS inf ON inf.id = kr.source_id
                WHERE (:countries IS NULL OR kr.country_key = ANY(:countries))
                  AND (:sources IS NULL OR inf.source_name = ANY(:sources)) -- Only requested countries and sources
            ), in_interval AS (
                SELECT *
                FROM filtered_by_countries_and_sources
                WHERE (:end IS NULL OR started_at <= :end::timestamptz)
                  AND (:start IS NULL OR last_confirmed_at > :start::timestamptz)) -- Filtering by time
            SELECT in_interval.id,
                   in_interval.country_key,
                   in_interval.source_id,
                   in_interval.source_name,
                   in_interval.started_at,
                   in_interval.last_confirmed_at,
                   in_interval.value
            FROM in_interval
            UNION
            SELECT remainders.id,
                   remainders.country_key,
                   remainders.source_id,
                   remainders.source_name,
                   remainders.started_at,
                   remainders.last_confirmed_at,
                   remainders.value
            FROM (SELECT fv.*, RANK() OVER (PARTITION BY fv.country_key, fv.source_id ORDER BY fv.last_confirmed_at DESC) AS rank
                  FROM filtered_by_countries_and_sources fv
                  WHERE NOT EXISTS(SELECT 1 FROM in_interval itr WHERE itr.source_id = fv.source_id AND itr.country_key = fv.country_key))
                AS remainders -- If for some pair (country_key, source_id) there are no rows in `in_interval`, taking the last inserted row
                    WHERE remainders.rank = 1;
        ";

        var parameters = new
        {
            countries = model.Countries,
            sources = model.Sources,
            start = model.Start,
            end = model.End,
        };

        var command = new CommandDefinition(query, parameters, cancellationToken: ct);

        await using var connection = _connectionProvider.GetConnection();

        var keyRates = await connection.QueryAsync<KeyRate>(command);

        return keyRates.CastToReadOnlyList();
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
                                          value,
                                          started_at,
                                          last_confirmed_at)
            VALUES (:countryKey::text,
                    :sourceId::integer,
                    :value::decimal,
                    :startedAt::timestamptz,
                    :lastConfirmedAt::timestamptz);
        ";

        var parameters = new
        {
            countryKey = keyRate.CountryKey,
            sourceId = keyRate.SourceId,
            value = keyRate.Value,
            startedAt = keyRate.StartedAt,
            lastConfirmedAt = keyRate.LastConfirmedAt,
        };

        var command = new CommandDefinition(query, parameters, cancellationToken: ct);

        await using var connection = _connectionProvider.GetConnection();

        await connection.ExecuteAsync(command);
    }
}