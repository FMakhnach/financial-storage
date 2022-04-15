using System.Globalization;
using CsvHelper;
using Dapper;
using FinancialStorage.Migrations.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Npgsql;
using SimpleMigrations;

namespace FinancialStorage.Migrations.Migrations;

[Migration(5, "Fill key rate data")]
public class FillKeyRateData : Migration
{
    private record CountryKeyRateInfo
    (
        string CountryName,
        string SourceName,
        string CountryKey,
        string PageUrl
    );

    private record KeyRateSourceParams(string CountryKey, string PageUrl);

    protected override void Up()
    {
        var keyRateInfos = Csv.Read<CountryKeyRateInfo>(GlobalConfiguration.KeyRateDataPath);

        AddCountries(keyRateInfos);
        AddSources(keyRateInfos);
    }

    protected override void Down()
    {
        var keyRateInfos = Csv.Read<CountryKeyRateInfo>(GlobalConfiguration.KeyRateDataPath);

        DeleteSources(keyRateInfos);
        DeleteCountries(keyRateInfos);
    }

    private void AddCountries(IReadOnlyCollection<CountryKeyRateInfo> items)
    {
        const string query = @"
            INSERT INTO public.countries (country_key, name) 
            VALUES (:countryKey, :countryName)
            ON CONFLICT DO NOTHING;
        ";

        foreach (var item in items)
        {
            var parameters = new
            {
                countryKey = item.CountryKey,
                countryName = item.CountryName,
            };

            var command = new CommandDefinition(query, parameters);

            Connection.Execute(command);
        }
    }
    
    private void AddSources(IReadOnlyCollection<CountryKeyRateInfo> items)
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
                @params = JsonConvert.SerializeObject(new KeyRateSourceParams(item.CountryKey, item.PageUrl), new JsonSerializerSettings
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
    
    private void DeleteCountries(IReadOnlyCollection<CountryKeyRateInfo> items)
    {
        const string query = @"
            DELETE FROM public.countries
            WHERE country_key = ANY(:countryKeys::text[]);
        ";

        var parameters = new
        {
            countryKeys = items.Select(x => x.CountryKey).ToArray(),
        };

        var command = new CommandDefinition(query, parameters);

        Connection.Execute(command);
    }
    
    private void DeleteSources(IReadOnlyCollection<CountryKeyRateInfo> items)
    {
        const string query = @"
            DELETE FROM public.information_sources
            WHERE source_name = ANY(:sourceNames::text[]);
        ";

        var parameters = new
        {
            sourceNames = items.Select(x => x.SourceName).Distinct().ToArray(),
        };

        var command = new CommandDefinition(query, parameters);

        Connection.Execute(command);
    }
}