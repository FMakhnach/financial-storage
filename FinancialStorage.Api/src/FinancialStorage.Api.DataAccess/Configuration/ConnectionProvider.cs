using System.Data.Common;
using FinancialStorage.Api.DataAccess.Configuration.Options;
using Microsoft.Extensions.Options;
using Npgsql;

namespace FinancialStorage.Api.DataAccess.Configuration;

public interface IConnectionProvider
{
    DbConnection GetConnection();
}

public class ConnectionProvider : IConnectionProvider
{
    private readonly string _connectionString;

    public ConnectionProvider(IOptions<PostgresConnectionOptions> options)
    {
        _connectionString = options.Value.MasterConnectionString;
    }

    public DbConnection GetConnection()
    {
        return new NpgsqlConnection(_connectionString);
    }
}