namespace FinancialStorage.BackgroundWorkers.DataAccess.Extensions;

internal static class CollectionsExtensions
{
    /// <summary>
    /// Can be used for dapper queries output.
    /// </summary>
    internal static IReadOnlyList<T> CastToReadOnlyList<T>(this IEnumerable<T> enumerable)
    {
        return (IReadOnlyList<T>)enumerable;
    }
}