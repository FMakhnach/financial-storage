using System.Globalization;
using CsvHelper;

namespace FinancialStorage.Migrations.Utils;

public static class Csv
{
    public static IReadOnlyCollection<T> Read<T>(string path)
    {
        using var reader = new StreamReader(path);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        var records = csv.GetRecords<T>().ToArray();

        return records;
    }
}