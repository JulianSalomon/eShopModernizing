FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project files
COPY eShopLegacy.Utilities/eShopLegacy.Utilities.csproj eShopLegacy.Utilities/
COPY src/eShopLegacyMVC/eShopLegacyMVC.csproj src/eShopLegacyMVC/

# Restore dependencies
RUN dotnet restore src/eShopLegacyMVC/eShopLegacyMVC.csproj

# Copy source code
COPY eShopLegacy.Utilities/ eShopLegacy.Utilities/
COPY src/eShopLegacyMVC/ src/eShopLegacyMVC/

# Build and publish
RUN dotnet publish src/eShopLegacyMVC/eShopLegacyMVC.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "eShopLegacyMVC.dll"]
