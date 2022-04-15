using Hangfire.Dashboard;

namespace FinancialStorage.BackgroundWorkers.Configuration;

public class DashboardNoAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext dashboardContext) => true;
}