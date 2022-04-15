namespace FinancialStorage.BackgroundWorkers.Application.Jobs;

public interface IRecurringJob
{
    string JobId { get; }

    string Cron { get; }

    Task ProcessAsync(CancellationToken ct);
}