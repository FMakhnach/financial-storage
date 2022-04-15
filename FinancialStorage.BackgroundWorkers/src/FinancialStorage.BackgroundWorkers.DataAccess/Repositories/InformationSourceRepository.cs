using Dapper;
using FinancialStorage.BackgroundWorkers.DataAccess.Configuration;
using FinancialStorage.BackgroundWorkers.DataAccess.Extensions;
using FinancialStorage.BackgroundWorkers.Domain.Entities.InformationSources;
using FinancialStorage.BackgroundWorkers.Domain.Repositories;
using JetBrains.Annotations;

namespace FinancialStorage.BackgroundWorkers.DataAccess.Repositories;

[UsedImplicitly]
public class InformationSourceRepository : IInformationSourceRepository
{
    private readonly IConnectionProvider _connectionProvider;

    public InformationSourceRepository(IConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider;
    }

    public async Task<IReadOnlyList<Source<T>>> GetSourcesAsync<T>(string sourceName, CancellationToken ct)
    {
        const string query = @"
            SELECT src.id,
                   src.source_name,
                   src.params
            FROM public.information_sources src
            WHERE src.source_name = :sourceName;
        ";

        var parameters = new
        {
            sourceName,
        };

        var command = new CommandDefinition(query, parameters, cancellationToken: ct);

        await using var connection = _connectionProvider.GetConnection();

        var result = await connection.QueryAsync<Source<T>>(command);

        return result.CastToReadOnlyList();
    }
}