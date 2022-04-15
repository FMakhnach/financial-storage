FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
ARG project_name="FinancialStorage.Migrations"
WORKDIR /app

COPY . ./
RUN dotnet publish "src/FinancialStorage.Migrations" -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build-env /app/${project_name}/out .
COPY "src/FinancialStorage.Migrations/Data" /app/Data

ENTRYPOINT ["dotnet", "FinancialStorage.Migrations.dll"]

