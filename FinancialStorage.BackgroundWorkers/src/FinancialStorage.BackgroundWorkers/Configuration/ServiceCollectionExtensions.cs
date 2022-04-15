using System.Reflection;
using FinancialStorage.BackgroundWorkers.Application.Jobs.KeyRates;
using FinancialStorage.BackgroundWorkers.Application.WebScrapers;
using FinancialStorage.BackgroundWorkers.DataAccess.Configuration;
using FinancialStorage.BackgroundWorkers.DataAccess.Configuration.Options;
using FinancialStorage.BackgroundWorkers.DataAccess.Repositories;
using NetCore.AutoRegisterDi;

namespace FinancialStorage.BackgroundWorkers.Configuration;

public static class ServiceCollectionExtensions
{
    public static void ConfigureOptions<TOptions>(this IServiceCollection services, IConfiguration configuration)
        where TOptions : class
    {
        var optionsSection = configuration.GetSection(typeof(TOptions).Name);
        services.Configure<TOptions>(optionsSection);
    }

    public static void ConfigureDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.ConfigureOptions<PostgresConnectionOptions>(configuration);
        services.AddSingleton<IConnectionProvider, ConnectionProvider>();

        PostgresConfiguration.ConfigureDatabaseMappings();
    }

    public static IServiceCollection AddScrapers(this IServiceCollection services)
    {
        services
            .RegisterAssemblyPublicNonGenericClasses(Assembly.GetAssembly(typeof(RuInvestingComScraper)))
            .Where(c => c.Name.EndsWith("Scraper"))
            .AsPublicImplementedInterfaces(ServiceLifetime.Scoped);

        return services;
    }

    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services
            .RegisterAssemblyPublicNonGenericClasses(Assembly.GetAssembly(typeof(KeyRateRepository)))
            .Where(c => c.Name.EndsWith("Repository"))
            .AsPublicImplementedInterfaces(ServiceLifetime.Scoped);

        return services;
    }

    public static IServiceCollection AddJobs(this IServiceCollection services)
    {
        services
            .RegisterAssemblyPublicNonGenericClasses(Assembly.GetAssembly(typeof(RuInvestingComKeyRateScrapJob)))
            .Where(c => c.Name.EndsWith("Job"))
            .AsPublicImplementedInterfaces(ServiceLifetime.Scoped);

        return services;
    }
}