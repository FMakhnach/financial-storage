using Dapper;
using FinancialStorage.BackgroundWorkers.DataAccess.Configuration;
using FinancialStorage.BackgroundWorkers.DataAccess.Extensions;
using FinancialStorage.BackgroundWorkers.Domain.Entities;
using FinancialStorage.BackgroundWorkers.Domain.Repositories;
using JetBrains.Annotations;

namespace FinancialStorage.BackgroundWorkers.DataAccess.Repositories;

[UsedImplicitly]
public class DividendRepository : IDividendRepository
{
    private readonly IConnectionProvider _connectionProvider;

    public DividendRepository(IConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider;
    }

    public async Task<Dividend?> GetLatestAsync(string ticker, long sourceId, CancellationToken ct)
    {
        const string query = @"
            SELECT id,
                   ticker,
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
                   frequency
            FROM public.dividends
            WHERE ticker = :ticker::text
              AND source_id = :sourceId::bigint
            ORDER BY last_confirmed_at;
        ";

        var parameters = new
        {
            ticker,
            sourceId,
        };

        var command = new CommandDefinition(query, parameters, cancellationToken: ct);

        await using var connection = _connectionProvider.GetConnection();

        var dividend = await connection.QueryFirstOrDefaultAsync<Dividend>(command);

        return dividend;
    }

    public async Task ConfirmAsync(long dividendId, CancellationToken ct)
    {
        const string query = @"
            UPDATE public.dividends
            SET last_confirmed_at = :lastConfirmedAt
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
                    :sourceId::bigint,
                    :startedAt::timestamptz,
                    :lastConfirmedAt::timestamptz,
                    :amountPerShare::decimal,
                    :status::public.dividend_status,
                    :amountChangedPercent::decimal,
                    :sharePrice::decimal,
                    :yield::decimal,
                    :decDate::date,
                    :exDate::date,
                    :payDate::date,
                    :frequency::public.dividend_frequency);
        ";

        var parameters = new
        {
            ticker = dividend.Ticker,
            sourceId = dividend.SourceId,
            startedAt = dividend.StartedAt,
            lastConfirmedAt = dividend.LastConfirmedAt,
            amountPerShare = dividend.AmountPerShare,
            status = dividend.Status.ToPgsqlEnumString(),
            amountChangedPercent = dividend.AmountChangedPercent,
            sharePrice = dividend.SharePrice,
            yield = dividend.Yield,
            decDate = dividend.DecDate,
            exDate = dividend.ExDate,
            payDate = dividend.PayDate,
            frequency = dividend.Frequency?.ToPgsqlEnumString() ?? "unknown",
        };

        var command = new CommandDefinition(query, parameters, cancellationToken: ct);

        await using var connection = _connectionProvider.GetConnection();

        await connection.ExecuteAsync(command);
    }
}