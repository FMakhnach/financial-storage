using SimpleMigrations;

namespace FinancialStorage.Migrations.Migrations;

[Migration(1, "Create key rate tables")]
public class CreateKeyRateTables : Migration
{
    protected override void Up()
    {
        Execute(@"
            create table public.countries
            (
                country_key text primary key,
                name        text not null
            );
        ");

        Execute("create type public.currency as enum ('USD', 'EUR', 'RUB', 'CNY');");
        Execute(@"
            create table public.country_currency
            (
                country_key text            not null,
                currency    public.currency not null
            );
        ");
        
        Execute(@"
            create table public.information_sources
            (
                id          bigint  primary key generated always as identity,
                source_name text    not null,
                params      jsonb   not null default '{}'::jsonb
            );
        ");

        Execute(@"
            create table public.key_rates
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
        
        Execute("create index if not exists key_rates_country_key_source_id_idx on public.key_rates(country_key, source_id);");
        Execute("create index if not exists key_rates_time_interval_idx on public.key_rates(started_at, last_confirmed_at);");
        Execute("create index if not exists key_rates_last_confirmed_at_idx on public.key_rates(last_confirmed_at DESC);");
    }

    protected override void Down()
    {
        Execute("drop index if exists key_rates_country_key_source_id_idx;");
        Execute("drop index if exists key_rates_time_interval_idx;");
        Execute("drop index if exists key_rates_last_confirmed_at_idx;");

        Execute("drop table public.key_rates;");
        Execute("drop table public.information_sources;");
        Execute("drop table public.country_currency;");
        Execute("drop type public.currency;");
        Execute("drop table public.countries;");
    }
}