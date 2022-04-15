FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
ARG project_name="FinancialStorage.Api"
WORKDIR /app

COPY . ./
RUN dotnet publish "src/FinancialStorage.Api" -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:6.0
COPY --from=build-env /app/${project_name}/out .

EXPOSE 80

ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_FORWARDEDHEADERS_ENABLED=true
ENTRYPOINT ["dotnet", "FinancialStorage.Api.dll"]
