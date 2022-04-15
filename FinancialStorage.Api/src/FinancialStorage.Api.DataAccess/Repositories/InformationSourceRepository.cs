using Dapper;
using FinancialStorage.Api.DataAccess.Configuration;
using FinancialStorage.Api.Domain.Entities;
using FinancialStorage.Api.Domain.Repositories;
using JetBrains.Annotations;

namespace FinancialStorage.Api.DataAccess.Repositories;

[UsedImplicitly]
public class InformationSourceRepository : IInformationSourceRepository
{
    private readonly IConnectionProvider _connectionProvider;

    public InformationSourceRepository(IConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider;
    }

    public async Task<InformationSource?> GetByNameAsync(string sourceName, CancellationToken ct)
    {
        const string query = @"
            SELECT inf.id,
                   inf.source_name
            FROM public.information_sources AS inf
            WHERE inf.source_name = :sourceName
        ";

        var parameters = new
        {
            sourceName,
        };

        var command = new CommandDefinition(query, parameters, cancellationToken: ct);

        await using var connection = _connectionProvider.GetConnection();

        var informationSource = await connection.QueryFirstOrDefaultAsync<InformationSource>(command);

        return informationSource;
    }

    public async Task<InformationSource> CreateAsync(string sourceName, CancellationToken ct)
    {
        const string query = @"
            INSERT INTO public.information_sources (source_name)
            VALUES (:sourceName)
            RETURNING id, source_name;
        ";

        var parameters = new
        {
            sourceName,
        };

        var command = new CommandDefinition(query, parameters, cancellationToken: ct);

        await using var connection = _connectionProvider.GetConnection();

        var createdSource = await connection.QueryFirstAsync<InformationSource>(command);

        return createdSource;
    }
}