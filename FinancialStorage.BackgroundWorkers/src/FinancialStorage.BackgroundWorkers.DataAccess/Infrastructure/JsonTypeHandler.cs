using System.Data;
using Dapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace FinancialStorage.BackgroundWorkers.DataAccess.Infrastructure;

public class JsonTypeHandler<T> : SqlMapper.TypeHandler<T>
{
    private static readonly JsonSerializerSettings Settings = new()
    {
        ContractResolver = new DefaultContractResolver
        {
            NamingStrategy = new SnakeCaseNamingStrategy(),
        },
    };

    public override void SetValue(IDbDataParameter parameter, T value)
    {
        parameter.Value = JsonConvert.SerializeObject(value, Settings);
    }

    public override T Parse(object value)
    {
        return JsonConvert.DeserializeObject<T>(value.ToString(), Settings);
    }
}