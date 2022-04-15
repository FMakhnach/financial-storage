using SimpleMigrations;

namespace FinancialStorage.Migrations.Migrations;

[Migration(4, "Inflation rates, population, other economic values")]
public class CreateStocksAndCompanyStats : Migration
{
    protected override void Up()
    {
        Execute(@"
            create table public.stocks
            (
                id             bigint           generated always as identity,
                ticker         text             not null,
                source_id      bigint           not null,
                start          timestamptz      not null,
                bucket_seconds bigint           not null,
                price_open     double precision null,
                price_close    double precision null,
                price_low      double precision null,
                price_high     double precision null,
                volume         integer          null,

                primary key (id, start), -- Adding 'start' to primary key to create a hypertable 
                foreign key (ticker) references public.instrument_info (ticker) on delete cascade,
                foreign key (source_id) references public.information_sources (id) on delete cascade
            );
        ");

        //language=sql
        Execute("create extension if not exists timescaledb CASCADE;");
        Execute("SELECT create_hypertable('public.stocks','start');");
        Execute("create index if not exists stocks_ticker_source_id_idx on public.stocks(ticker, source_id, start DESC);");

        Execute(@"
            create table public.company_financial_values
            (
                id                bigint           primary key generated always as identity,
                company_id        integer          not null,
                source_id         bigint           not null,
                started_at        timestamptz      not null,
                last_confirmed_at timestamptz      not null,
                market_cap        bigint           null,
                ebitda            bigint           null,
                price_to_sales    double precision null,
                diluted_eps       double precision null,
                roe               double precision null,
                roa               double precision null,
                roi               double precision null,
                net_profit_margin double precision null,

                foreign key (company_id) references public.companies (id) on delete cascade,
                foreign key (source_id) references public.information_sources (id) on delete cascade
            );
        ");
        
        Execute("create index if not exists company_financial_values_company_id_source_id_idx on public.company_financial_values(company_id, source_id);");
        Execute("create index if not exists company_financial_values_time_interval_idx on public.company_financial_values(started_at, last_confirmed_at);");
        Execute("create index if not exists company_financial_values_last_confirmed_at_idx on public.company_financial_values(last_confirmed_at);");
    }

    protected override void Down()
    {
        Execute("drop index if exists company_financial_values_company_id_source_id_idx");
        Execute("drop index if exists company_financial_values_time_interval_idx");
        Execute("drop index if exists company_financial_values_last_confirmed_at_idx");
        Execute("drop table public.company_financial_values;");
        
        Execute("drop index if exists stocks_ticker_source_id_idx;");
        Execute("drop table public.stocks;");
    }
}