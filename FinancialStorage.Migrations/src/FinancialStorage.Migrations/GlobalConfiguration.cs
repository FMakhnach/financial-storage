using Microsoft.Extensions.Configuration;

namespace FinancialStorage.Migrations;

public static class GlobalConfiguration
{
    private static IConfiguration Configuration { get; } = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json")
        .AddJsonFile("appsettings.Production.json")
        .AddEnvironmentVariables()
        .Build();

    public static string ConnectionString => Configuration.GetRequiredSection(nameof(ConnectionString)).Get<string>();

    public static string KeyRateDataPath => Configuration.GetRequiredSection(nameof(KeyRateDataPath)).Get<string>();
    
    public static string DividendDataPath => Configuration.GetRequiredSection(nameof(DividendDataPath)).Get<string>();
}