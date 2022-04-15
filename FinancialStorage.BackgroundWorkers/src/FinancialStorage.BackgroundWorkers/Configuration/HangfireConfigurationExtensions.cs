using FinancialStorage.BackgroundWorkers.Application.Jobs;
using FinancialStorage.BackgroundWorkers.DataAccess.Configuration.Options;
using Hangfire;
using Hangfire.Common;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;

namespace FinancialStorage.BackgroundWorkers.Configuration;

public static class HangfireConfigurationExtensions
{
    public static void AddHangfire(this IServiceCollection services, IConfiguration configuration)
    {
        services.ConfigureOptions<HangfireConnectionOptions>(configuration);

        services.AddHangfire((sp, config) =>
        {
            var connectionString = sp.GetRequiredService<IOptions<HangfireConnectionOptions>>().Value.MasterConnectionString;

            config
                .UseColouredConsoleLogProvider()
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UsePostgreSqlStorage(
                    connectionString,
                    new PostgreSqlStorageOptions
                    {
                        QueuePollInterval = TimeSpan.FromMinutes(1),
                        SchemaName = "hangfire",
                    });
        });
        services.AddHangfireServer(options => { options.WorkerCount = 2; });
    }

    public static void UseHangfire(this IApplicationBuilder app)
    {
        var dashboardOptions =
            new DashboardOptions
            {
                Authorization = new[] { new DashboardNoAuthorizationFilter() },
            };
        app.UseHangfireDashboard(options: dashboardOptions);
    }

    public static void UseRecurringJobs(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();

        var recurringJobManager = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();
        var environment = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();

        var jobs = scope.ServiceProvider.GetServices<IRecurringJob>();

        foreach (var job in jobs)
        {
            var jobType = job.GetType();
            var jobMethod = jobType.GetMethod(nameof(IRecurringJob.ProcessAsync));
            var hangfireJob = new Job(jobType, jobMethod, CancellationToken.None);

            const string debugSchedule = "0 0 31 2 *";
            var cronSchedule = environment.IsDevelopment()
                ? debugSchedule
                : job.Cron;

            recurringJobManager.AddOrUpdate(
                job.JobId,
                hangfireJob,
                cronSchedule);
        }
    }
}