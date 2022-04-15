using System.Data.Common;
using FinancialStorage.BackgroundWorkers.DataAccess.Configuration.Options;
using Microsoft.Extensions.Options;
using Npgsql;

namespace FinancialStorage.BackgroundWorkers.DataAccess.Configuration;

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