using Dapper;
using FinancialStorage.Api.DataAccess.Configuration;
using FinancialStorage.Api.DataAccess.Extensions;
using FinancialStorage.Api.Domain.Entities;
using FinancialStorage.Api.Domain.Models;
using FinancialStorage.Api.Domain.Repositories;
using JetBrains.Annotations;

namespace FinancialStorage.Api.DataAccess.Repositories;

[UsedImplicitly]
public class DividendRepository : IDividendRepository
{
    private readonly IConnectionProvider _connectionProvider;

    public DividendRepository(IConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider;
    }

    public async Task<IReadOnlyCollection<Dividend>> GetAtMomentAsync(string ticker, string? sourceName, DateTimeOffset moment, CancellationToken ct)
    {
        const string query = @"
            SELECT ranked.id,
                   ranked.ticker,
                   ranked.source_id,
                   ranked.source_name,
                   ranked.started_at,
                   ranked.last_confirmed_at,
                   ranked.amount_per_share,
                   ranked.status,
                   ranked.amount_changed_percent,
                   ranked.share_price,
                   ranked.yield,
                   ranked.dec_date,
                   ranked.ex_date,
                   ranked.pay_date,
                   ranked.frequency
            FROM (SELECT div.*,
                         inf.source_name,
                         RANK() OVER (PARTITION BY div.source_id ORDER BY div.started_at DESC) AS rank
                  FROM public.dividends AS div
                           JOIN public.information_sources AS inf ON inf.id = div.source_id
                  WHERE div.ticker = :ticker::text
                    AND (:sourceName IS NULL OR inf.source_name = :sourceName::text)
                    AND div.started_at <= :moment::timestamptz) AS ranked
            WHERE ranked.rank = 1;
        ";

        var parameters = new
        {
            ticker,
            sourceName,
            moment,
        };

        var command = new CommandDefinition(query, parameters, cancellationToken: ct);

        await using var connection = _connectionProvider.GetConnection();

        var keyRate = await connection.QueryAsync<Dividend>(command);

        return keyRate.CastToReadOnlyList();
    }

    public async Task<IReadOnlyCollection<Dividend>> SearchAsync(SearchDividendModel model, CancellationToken ct)
    {
        //language=sql
        const string query = @"
            WITH filtered_by_tickers_and_sources AS (
                SELECT div.*,
                       inf.source_name
                FROM public.dividends AS div
                    JOIN public.information_sources AS inf ON inf.id = div.source_id
                WHERE (:tickers::text[] IS NULL OR div.ticker = ANY(:tickers::text[]))
                  AND (:sources::text[] IS NULL OR inf.source_name = ANY(:sources::text[])) -- Only requested tickers and sources
            ), in_interval AS (
                SELECT *
                FROM filtered_by_tickers_and_sources
                WHERE (:end::timestamptz IS NULL OR started_at <= :end::timestamptz)
                  AND (:start::timestamptz IS NULL OR last_confirmed_at > :start::timestamptz)) -- Filtering by time
            SELECT in_interval.id,
                   in_interval.ticker,
                   in_interval.source_id,
                   in_interval.source_name,
                   in_interval.started_at,
                   in_interval.last_confirmed_at,
                   in_interval.amount_per_share,
                   in_interval.status,
                   in_interval.amount_changed_percent,
                   in_interval.share_price,
                   in_interval.yield,
                   in_interval.dec_date,
                   in_interval.ex_date,
                   in_interval.pay_date,
                   in_interval.frequency
            FROM in_interval
            UNION
            SELECT remainders.id,
                   remainders.ticker,
                   remainders.source_id,
                   remainders.source_name,
                   remainders.started_at,
                   remainders.last_confirmed_at,
                   remainders.amount_per_share,
                   remainders.status,
                   remainders.amount_changed_percent,
                   remainders.share_price,
                   remainders.yield,
                   remainders.dec_date,
                   remainders.ex_date,
                   remainders.pay_date,
                   remainders.frequency
            FROM (SELECT fv.*, RANK() OVER (PARTITION BY fv.ticker, fv.source_id ORDER BY fv.last_confirmed_at DESC) AS rank
                  FROM filtered_by_tickers_and_sources fv
                  WHERE NOT EXISTS(SELECT 1 FROM in_interval itr WHERE itr.source_id = fv.source_id AND itr.ticker = fv.ticker))
                AS remainders -- If for some pair (ticker, source_id) there are no rows in `in_interval`, taking the last inserted row
                    WHERE remainders.rank = 1;
        ";

        var parameters = new
        {
            tickers = model.Tickers,
            sources = model.Sources,
            start = model.Start,
            end = model.End,
        };

        var command = new CommandDefinition(query, parameters, cancellationToken: ct);

        await using var connection = _connectionProvider.GetConnection();

        var keyRates = await connection.QueryAsync<Dividend>(command);

        return keyRates.CastToReadOnlyList();
    }

    public async Task ConfirmAsync(long dividendId, CancellationToken ct)
    {
        const string query = @"
            UPDATE public.dividends
            SET last_confirmed_at = :lastConfirmedAt::timestamptz
            WHERE id = :id;
        ";

        var parameters = new
        {
            id = dividendId,
            lastConfirmedAt = DateTimeOffset.UtcNow,
        };

        var command = new CommandDefinition(query, parameters, cancellationToken: ct);

        await using var connection = _connectionProvider.GetConnection();

        await connection.ExecuteAsync(command);
    }

    public async Task UpdateAsync(Dividend dividend, CancellationToken ct)
    {
        const string query = @"
            INSERT INTO public.dividends (ticker,
                                          source_id,
                                          started_at,
                                          last_confirmed_at,
                                          amount_per_share,
                                          status,
                                          amount_changed_percent,
                                          share_price,
                                          yield,
                                          dec_date,
                                          ex_date,
                                          pay_date,
                                          frequency)
            VALUES (:ticker::text,
                    :sourceId::integer,
                    :startedAt::timestamptz,
                    :lastConfirmedAt::timestamptz,
                    :amoundPerShare::decimal,
                    :status::public.dividend_status,
                    :amountChangedPercent::decimal,
                    :sharePrice::decimal,
                    :yield::decimal,
                    :decDate::timestamptz,
                    :exDate::timestamptz,
                    :payDate::timestamptz,
                    :frequency::public.dividend_frequency);
        ";

        var parameters = new
        {
            ticker = dividend.Ticker,
            sourceId = dividend.SourceId,
            startedAt = dividend.StartedAt,
            lastConfirmedAt = dividend.LastConfirmedAt,
            amoundPerShare = dividend.AmountPerShare,
            status = dividend.Status.ToPgsqlEnumString(),
            amountChangedPercent = dividend.AmountChangedPercent,
            sharePrice = dividend.SharePrice,
            yield = dividend.Yield,
            decDate = dividend.DecDate,
            exDate = dividend.ExDate,
            payDate = dividend.PayDate,
            frequency = dividend.Frequency?.ToPgsqlEnumString(),
        };

        var command = new CommandDefinition(query, parameters, cancellationToken: ct);

        await using var connection = _connectionProvider.GetConnection();

        await connection.ExecuteAsync(command);
    }
}