FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project files
COPY eShopLegacy.Utilities/eShopLegacy.Utilities.csproj eShopLegacy.Utilities/
COPY eShopPorted/eShopPorted.csproj eShopPorted/

# Restore dependencies
RUN dotnet restore eShopPorted/eShopPorted.csproj

# Copy source code
COPY eShopLegacy.Utilities/ eShopLegacy.Utilities/
COPY eShopPorted/ eShopPorted/

# Build and publish
RUN dotnet publish eShopPorted/eShopPorted.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "eShopPorted.dll"]
