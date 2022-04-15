using FinancialStorage.BackgroundWorkers.Configuration;
using Microsoft.AspNetCore.Builder;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
var configuration = builder.Configuration;

// Add services to container
{
    services.AddHangfire(configuration);

    services.ConfigureDatabase(configuration);

    services.AddScrapers();
    services.AddRepositories();
    services.AddJobs();
}

var app = builder.Build();

// Configure pipeline
{
    app.UseHangfire();
    app.UseRecurringJobs();
}

app.Run();