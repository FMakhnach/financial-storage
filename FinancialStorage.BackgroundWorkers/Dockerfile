FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
ARG project_name="FinancialStorage.BackgroundWorkers"
WORKDIR /app

COPY . ./
RUN dotnet publish "src/FinancialStorage.BackgroundWorkers" -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build-env /app/${project_name}/out .

EXPOSE 80

ENTRYPOINT ["dotnet", "FinancialStorage.BackgroundWorkers.dll"]

