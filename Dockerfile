FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY CurrencyConverter.Web/CurrencyConverter.Web.csproj CurrencyConverter.Web/
RUN dotnet restore CurrencyConverter.Web/CurrencyConverter.Web.csproj

COPY CurrencyConverter.Web/ CurrencyConverter.Web/
WORKDIR /src/CurrencyConverter.Web
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENTRYPOINT ["dotnet", "CurrencyConverter.Web.dll"]
