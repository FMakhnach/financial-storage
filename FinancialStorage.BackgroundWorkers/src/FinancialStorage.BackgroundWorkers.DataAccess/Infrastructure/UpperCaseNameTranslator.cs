using Npgsql;
using Npgsql.NameTranslation;

namespace FinancialStorage.BackgroundWorkers.DataAccess.Infrastructure;

public class UpperCaseNameTranslator : INpgsqlNameTranslator
{
    public string TranslateTypeName(string clrName)
    {
        return NpgsqlSnakeCaseNameTranslator.ConvertToSnakeCase(clrName);
    }

    public string TranslateMemberName(string clrName)
    {
        // returning value as is
        return clrName;
    }
}