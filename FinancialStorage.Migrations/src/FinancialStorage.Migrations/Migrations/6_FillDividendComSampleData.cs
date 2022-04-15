using System.Globalization;
using CsvHelper;
using Dapper;
using FinancialStorage.Migrations.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Npgsql;
using SimpleMigrations;

namespace FinancialStorage.Migrations.Migrations;

[Migration(6, "Fill dividend.com data")]
public class FillDividendComSampleData : Migration
{
    private record CompanyInfo(string Name, string Ticker, string Currency, string SourceName, string PageUrl);
    private record DividendSourceParams(string Ticker, string PageUrl);

    protected override void Up()
    {
        var sampleInfo = Csv.Read<CompanyInfo>(GlobalConfiguration.DividendDataPath);

        var companyIds = AddCompanies(sampleInfo);
        AddInstruments(sampleInfo, companyIds);
        AddSources(sampleInfo);
    }

    protected override void Down()
    {
        var sampleInfo = Csv.Read<CompanyInfo>(GlobalConfiguration.DividendDataPath);

        DeleteSources(sampleInfo);
        DeleteInstruments(sampleInfo);
        DeleteCompanies(sampleInfo);
    }
    
    private IDictionary<string, long> AddCompanies(IReadOnlyCollection<CompanyInfo> items)
    {
        const string query = @"
            INSERT INTO public.companies (name) 
            VALUES (:companyName)
            ON CONFLICT DO NOTHING
            RETURNING id;
        ";

        var ids = new Dictionary<string, long>();
        
        foreach (var item in items)
        {
            var parameters = new
            {
                companyName = item.Name,
            };

            var command = new CommandDefinition(query, parameters);

            var companyId = Connection.ExecuteScalar<long>(command);

            ids[item.Name] = companyId;
        }

        return ids;
    }
    
    private void AddInstruments(IReadOnlyCollection<CompanyInfo> items, IDictionary<string, long> companyIds)
    {
        const string query = @"
            INSERT INTO public.instrument_info (ticker, company_id, currency)
            VALUES (:ticker, :companyId, :currency::public.currency)
            ON CONFLICT DO NOTHING;
        ";

        foreach (var item in items)
        {
            var parameters = new
            {
                ticker = item.Ticker,
                companyId = companyIds[item.Name],
                currency = item.Currency,
            };

            var command = new CommandDefinition(query, parameters);

            Connection.Execute(command);
        }
    }
    
    private void AddSources(IReadOnlyCollection<CompanyInfo> items)
    {
        const string query = @"
            INSERT INTO public.information_sources (source_name, params)
            VALUES (:sourceName::text, :params::jsonb)
            ON CONFLICT DO NOTHING;
        ";

        foreach (var item in items)
        {
            var parameters = new
            {
                sourceName = item.SourceName,
                @params = JsonConvert.SerializeObject(new DividendSourceParams(item.Ticker, item.PageUrl), new JsonSerializerSettings
                {
                    Formatting = Formatting.None,
                    ContractResolver = new DefaultContractResolver
                    {
                        NamingStrategy = new SnakeCaseNamingStrategy(),
                    },
                }),
            };

            var command = new CommandDefinition(query, parameters);

            Connection.Execute(command);
        }
    }
    
    private void DeleteCompanies(IReadOnlyCollection<CompanyInfo> items)
    {
        const string query = @"
            DELETE FROM public.companies
            WHERE name = ANY(:companyNames);
        ";

        var parameters = new
        {
            companyNames = items.Select(x => x.Name).Distinct().ToArray(),
        };

        var command = new CommandDefinition(query, parameters);

        Connection.Execute(command);
    }
    
    private void DeleteInstruments(IReadOnlyCollection<CompanyInfo> items)
    {
        const string query = @"
            DELETE FROM public.instrument_info
            WHERE ticker = ANY(:tickers);
        ";

        var parameters = new
        {
            tickers = items.Select(x => x.Ticker).Distinct().ToArray(),
        };

        var command = new CommandDefinition(query, parameters);

        Connection.Execute(command);
    }
    
    private void DeleteSources(IReadOnlyCollection<CompanyInfo> items)
    {
        const string query = @"
            DELETE FROM public.information_sources
            WHERE source_name = ANY(:sourceNames);
        ";

        var parameters = new
        {
            sourceNames = items.Select(x => x.SourceName).Distinct().ToArray(),
        };

        var command = new CommandDefinition(query, parameters);

        Connection.Execute(command);
    }
}