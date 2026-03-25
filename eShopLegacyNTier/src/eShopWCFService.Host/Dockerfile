FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project files
COPY src/eShopWCFService/eShopWCFService.csproj src/eShopWCFService/
COPY src/eShopWCFService.Host/eShopWCFService.Host.csproj src/eShopWCFService.Host/

# Restore dependencies
RUN dotnet restore src/eShopWCFService.Host/eShopWCFService.Host.csproj

# Copy source code
COPY src/eShopWCFService/ src/eShopWCFService/
COPY src/eShopWCFService.Host/ src/eShopWCFService.Host/

# Build and publish
RUN dotnet publish src/eShopWCFService.Host/eShopWCFService.Host.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "eShopWCFService.Host.dll"]
