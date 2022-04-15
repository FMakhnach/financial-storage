using Dapper;
using FinancialStorage.Migrations;
using Npgsql;
using SimpleMigrations;
using SimpleMigrations.DatabaseProvider;

DefaultTypeMap.MatchNamesWithUnderscores = true;
NpgsqlConnection.GlobalTypeMapper.UseJsonNet();

await using var db = new NpgsqlConnection(GlobalConfiguration.ConnectionString);
var databaseProvider = new PostgresqlDatabaseProvider(db);
var migrator = new SimpleMigrator(typeof(Program).Assembly, databaseProvider);

migrator.Load();
migrator.MigrateToLatest();