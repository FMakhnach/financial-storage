namespace FinancialStorage.BackgroundWorkers.Domain.Entities.InformationSources;

public class Source<TParams>
{
    public long Id { get; init; }

    public string SourceName { get; init; }

    public TParams Params { get; init; }
}