using SimpleMigrations;

namespace FinancialStorage.Migrations.Migrations;

[Migration(2, "Create dividend tables and types")]
public class CreateDividendTablesAndTypes : Migration
{
    protected override void Up()
    {
        Execute("create type public.dividend_frequency as enum ('unknown', 'monthly', 'quarterly', 'semi_annually', 'annually');");
        Execute("create type public.dividend_status as enum ('unknown', 'estimated', 'declared', 'paid');");

        // https://www.msci.com/our-solutions/indexes/gics
        //language=sql
        Execute(@"
            create type public.industry as enum
            (
                'energy',
                'materials',
                'industrials',
                'consumer_discretionary',
                'consumer_staples',
                'health_care',
                'financials',
                'information_technology',
                'communication_services',
                'utilities',
                'real_estate'
            );
        ");
        
        Execute(@"
            create table public.companies
            (
                id     integer primary key generated always as identity,
                name   text    not null
            );
        ");
        
        Execute(@"
            create table public.company_industry
            (
                company_id integer         primary key generated always as identity,
                industry   public.industry not null,
                
                foreign key (company_id) references public.companies (id) on delete cascade
            );
        ");
        
        Execute(@"
            create table public.instrument_info
            (
                ticker        text            primary key,
                company_id    integer         not null,
                currency      public.currency not null,
                
                foreign key (company_id) references public.companies (id) on delete cascade
            );
        ");
        
        Execute(@"
            create table public.dividends
            (
                id                     bigint                    primary key generated always as identity,
                ticker                 text                      not null,
                source_id              bigint                    not null,
                started_at             timestamptz               not null,
                last_confirmed_at      timestamptz               not null,
                amount_per_share       decimal                   not null,
                status                 public.dividend_status    not null,
                amount_changed_percent decimal                   null,
                share_price            decimal                   null,
                yield                  decimal                   null,
                dec_date               date                      null,
                ex_date                date                      null,
                pay_date               date                      null,
                frequency              public.dividend_frequency null,

                foreign key (ticker) references public.instrument_info (ticker) on delete cascade,
                foreign key (source_id) references public.information_sources (id) on delete cascade
            );
        ");
        
        Execute("create index if not exists dividends_ticker_source_id_idx on public.dividends(ticker, source_id);");
        Execute("create index if not exists dividends_time_interval_idx on public.dividends(started_at, last_confirmed_at);");
        Execute("create index if not exists dividends_last_confirmed_at_idx on public.dividends(last_confirmed_at DESC);");
    }

    protected override void Down()
    {
        Execute("drop index if exists dividends_ticker_source_id_idx");
        Execute("drop index if exists dividends_time_interval_idx");
        Execute("drop index if exists dividends_last_confirmed_at_idx");

        Execute("drop table public.dividends;");
        Execute("drop table public.instrument_info;");
        Execute("drop table public.company_industry;");
        Execute("drop table public.companies;");

        Execute("drop type public.industry;");
        Execute("drop type public.dividend_status;");
        Execute("drop type public.dividend_frequency;");
    }
}