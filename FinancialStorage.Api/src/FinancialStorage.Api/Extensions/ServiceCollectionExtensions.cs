using System.Reflection;
using FinancialStorage.Api.DataAccess.Configuration;
using FinancialStorage.Api.DataAccess.Configuration.Options;
using FinancialStorage.Api.DataAccess.Repositories;
using FinancialStorage.Api.Services;
using NetCore.AutoRegisterDi;

// ReSharper disable UnusedMethodReturnValue.Global

namespace FinancialStorage.Api.Extensions;

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

    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services
            .RegisterAssemblyPublicNonGenericClasses(Assembly.GetAssembly(typeof(KeyRateService)))
            .Where(c => c.Name.EndsWith("Service"))
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
}