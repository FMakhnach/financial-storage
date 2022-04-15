namespace FinancialStorage.BackgroundWorkers.Domain.Entities.InformationSources;

public class DividendSourceParams
{
    public string Ticker { get; init; }

    public string PageUrl { get; init; }
}