using SimpleMigrations;

namespace FinancialStorage.Migrations.Migrations;

[Migration(3, "Inflation rates, population, other economic values")]
public class CreateCountryValueTables : Migration
{
    protected override void Up()
    {
        Execute(@"
            create table public.inflation_rates
            (
                id                 bigint      primary key generated always as identity,
                country_key        text        not null,
                source_id          bigint      not null,
                started_at         timestamptz not null,
                last_confirmed_at  timestamptz not null,
                value              decimal     not null,

                foreign key (country_key) references public.countries (country_key) on delete cascade,
                foreign key (source_id) references public.information_sources (id) on delete cascade
            );
        ");
        
        Execute("create index if not exists inflation_rates_country_key_source_id_idx on public.inflation_rates(country_key, source_id);");
        Execute("create index if not exists inflation_rates_time_interval_idx on public.inflation_rates(started_at, last_confirmed_at);");
        Execute("create index if not exists inflation_rates_last_confirmed_at_idx on public.inflation_rates(last_confirmed_at DESC);");
        
        Execute(@"
            create table public.population
            (
                id                       bigint      primary key generated always as identity,
                country_key              text        not null,
                source_id                bigint      not null,
                started_at               timestamptz not null,
                last_confirmed_at        timestamptz not null,
                population               bigint      not null,
                migrants                 bigint      null,
                median_age               integer     null,
                urban_population_percent decimal     null,

                foreign key (country_key) references public.countries (country_key) on delete cascade,
                foreign key (source_id) references public.information_sources (id) on delete cascade
            );
        ");
        
        Execute("create index if not exists population_country_key_source_id_idx on public.population(country_key, source_id);");
        Execute("create index if not exists population_time_interval_idx on public.population(started_at, last_confirmed_at);");
        Execute("create index if not exists population_last_confirmed_at_idx on public.population(last_confirmed_at DESC);");
        
        Execute(@"
            create table public.country_economic_values
            (
                id                bigint      primary key generated always as identity,
                country_key       text        not null,
                source_id         bigint      not null,
                started_at        timestamptz not null,
                last_confirmed_at timestamptz not null,
                gdp_per_capita    bigint      null,
                public_debt       bigint      null,
                exports           bigint      null,
                imports           bigint      null,

                foreign key (country_key) references public.countries (country_key) on delete cascade,
                foreign key (source_id) references public.information_sources (id) on delete cascade
            );
        ");
        
        Execute("create index if not exists country_economic_values_country_key_source_id_idx on public.country_economic_values(country_key, source_id);");
        Execute("create index if not exists country_economic_values_time_interval_idx on public.country_economic_values(started_at, last_confirmed_at);");
        Execute("create index if not exists country_economic_values_last_confirmed_at_idx on public.country_economic_values(last_confirmed_at DESC);");
    }

    protected override void Down()
    { 
        Execute("drop index if exists country_economic_values_country_key_source_id_idx;");
        Execute("drop index if exists country_economic_values_time_interval_idx;");
        Execute("drop index if exists country_economic_values_last_confirmed_at_idx;");
        Execute("drop table public.country_economic_values;");

        Execute("drop index if exists population_country_key_source_id_idx;");
        Execute("drop index if exists population_time_interval_idx;");
        Execute("drop index if exists population_last_confirmed_at_idx;");
        Execute("drop table public.population;");

        Execute("drop index if exists inflation_rates_country_key_source_id_idx;");
        Execute("drop index if exists inflation_rates_time_interval_idx;");
        Execute("drop index if exists inflation_rates_last_confirmed_at_idx;");
        Execute("drop table public.inflation_rates;");
    }
}