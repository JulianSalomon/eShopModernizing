# eShopLegacy Local Testing Guide

This guide explains how to run and test all eShopLegacy applications locally after the .NET 10 modernization.

## Architecture Overview

```
┌─────────────────────────────────────────────────────────────────┐
│                    Docker Compose Network                       │
│                                                                 │
│  ┌──────────────┐  ┌──────────────┐  ┌───────────────────────┐ │
│  │ eShopLegacy  │  │  eShopPorted │  │ eShopLegacyWebForms   │ │
│  │    MVC       │  │  (EF Core)   │  │ (Blazor Server + EF6) │ │
│  │ (MVC + EF6)  │  │              │  │                       │ │
│  │  :5001       │  │  :5002       │  │  :5003                │ │
│  └──────┬───────┘  └──────┬───────┘  └───────────┬───────────┘ │
│         │                 │                       │             │
│         └────────────┬────┴───────────────────────┘             │
│                      │                                          │
│              ┌───────▼────────┐                                 │
│              │   SQL Server   │                                 │
│              │   (Azure SQL   │                                 │
│              │    Edge)       │                                 │
│              │   :1433        │                                 │
│              └───────▲────────┘                                 │
│                      │                                          │
│         ┌────────────┘                                          │
│         │                                                       │
│  ┌──────┴────────┐                                              │
│  │ eShopWCF      │         ┌─────────────────────┐              │
│  │ Service.Host  │◄────────│ eShopWinForms       │              │
│  │ (CoreWCF)     │  SOAP   │ (Windows Desktop)   │              │
│  │  :5004        │         │ Windows only         │              │
│  └───────────────┘         └─────────────────────┘              │
│                              (runs outside Docker)              │
└─────────────────────────────────────────────────────────────────┘
```

All 4 web apps share the same database: `Microsoft.eShopOnContainers.Services.CatalogDb`

## Prerequisites

- **Docker Desktop** (with Docker Compose)
- **macOS**, **Linux**, or **Windows** (for web apps)
- **.NET 10 SDK** (only needed for building without Docker, or for WinForms)
- **Windows** with .NET 10 Desktop Runtime (required for eShopWinForms only)

---

## Quick Start — Docker Compose (Recommended)

### Start all services

```bash
# From the repository root:
./test-eshop.sh
```

This will:
1. Build Docker images for all 4 web applications
2. Start a SQL Server container (Azure SQL Edge)
3. Start all 4 web applications
4. Run automated integration tests against all services
5. Report test results

### Access the applications

| Application | URL | Description |
|-------------|-----|-------------|
| **eShopLegacyMVC** | http://localhost:5001 | ASP.NET Core MVC + Entity Framework 6 |
| **eShopPorted** | http://localhost:5002 | ASP.NET Core MVC + Entity Framework Core |
| **eShopLegacyWebForms** | http://localhost:5003 | Blazor Server + Entity Framework 6 |
| **eShopWCFService.Host** | http://localhost:5004/CatalogService/CatalogService.svc | CoreWCF SOAP Service |

### Stop all services

```bash
./test-eshop.sh --down
```

### Force rebuild images

```bash
./test-eshop.sh --build
```

### View logs

```bash
# All services
docker-compose -f docker-compose.legacy.yml logs -f

# Specific service
docker-compose -f docker-compose.legacy.yml logs -f eshop-legacy-mvc
docker-compose -f docker-compose.legacy.yml logs -f eshop-ported
docker-compose -f docker-compose.legacy.yml logs -f eshop-legacy-webforms
docker-compose -f docker-compose.legacy.yml logs -f eshop-wcf-host
docker-compose -f docker-compose.legacy.yml logs -f sqlserver
```

---

## Manual Docker Compose

If you prefer to run docker-compose directly:

```bash
# Build and start
docker-compose -f docker-compose.legacy.yml up -d --build

# Check status
docker-compose -f docker-compose.legacy.yml ps

# Stop
docker-compose -f docker-compose.legacy.yml down -v
```

---

## Testing the WCF Service (SOAP)

The WCF service exposes 10 operations via SOAP over HTTP. You can test it with `curl`:

### Service Info Page
```bash
curl http://localhost:5004/CatalogService/CatalogService.svc
```

### WSDL
```bash
curl http://localhost:5004/CatalogService/CatalogService.svc?wsdl
```

### GetCatalogBrands
```bash
curl -X POST http://localhost:5004/CatalogService/CatalogService.svc \
  -H "Content-Type: text/xml; charset=utf-8" \
  -H "SOAPAction: http://tempuri.org/ICatalogService/GetCatalogBrands" \
  -d '<?xml version="1.0" encoding="utf-8"?>
<soap:Envelope xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/" xmlns:tem="http://tempuri.org/">
  <soap:Body>
    <tem:GetCatalogBrands/>
  </soap:Body>
</soap:Envelope>'
```

### GetCatalogItems (all products)
```bash
curl -X POST http://localhost:5004/CatalogService/CatalogService.svc \
  -H "Content-Type: text/xml; charset=utf-8" \
  -H "SOAPAction: http://tempuri.org/ICatalogService/GetCatalogItems" \
  -d '<?xml version="1.0" encoding="utf-8"?>
<soap:Envelope xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/" xmlns:tem="http://tempuri.org/">
  <soap:Body>
    <tem:GetCatalogItems>
      <tem:brandIdFilter>0</tem:brandIdFilter>
      <tem:typeIdFilter>0</tem:typeIdFilter>
    </tem:GetCatalogItems>
  </soap:Body>
</soap:Envelope>'
```

### FindCatalogItem (by ID)
```bash
curl -X POST http://localhost:5004/CatalogService/CatalogService.svc \
  -H "Content-Type: text/xml; charset=utf-8" \
  -H "SOAPAction: http://tempuri.org/ICatalogService/FindCatalogItem" \
  -d '<?xml version="1.0" encoding="utf-8"?>
<soap:Envelope xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/" xmlns:tem="http://tempuri.org/">
  <soap:Body>
    <tem:FindCatalogItem>
      <tem:id>1</tem:id>
    </tem:FindCatalogItem>
  </soap:Body>
</soap:Envelope>'
```

### GetCatalogTypes
```bash
curl -X POST http://localhost:5004/CatalogService/CatalogService.svc \
  -H "Content-Type: text/xml; charset=utf-8" \
  -H "SOAPAction: http://tempuri.org/ICatalogService/GetCatalogTypes" \
  -d '<?xml version="1.0" encoding="utf-8"?>
<soap:Envelope xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/" xmlns:tem="http://tempuri.org/">
  <soap:Body>
    <tem:GetCatalogTypes/>
  </soap:Body>
</soap:Envelope>'
```

### Available SOAP Operations

| Operation | Description | Parameters |
|-----------|-------------|------------|
| `FindCatalogItem` | Get a product by ID | `id` (int) |
| `GetCatalogBrands` | List all brands | (none) |
| `GetCatalogItems` | List products with filters | `brandIdFilter` (int), `typeIdFilter` (int) — use 0 for no filter |
| `GetCatalogTypes` | List all product types | (none) |
| `GetAvailableStock` | Get stock for a product | `date` (dateTime), `catalogItemId` (int) |
| `CreateAvailableStock` | Create stock record | `catalogItemsStock` (object) |
| `CreateCatalogItem` | Create a new product | `catalogItem` (object) |
| `UpdateCatalogItem` | Update a product | `catalogItem` (object) |
| `RemoveCatalogItem` | Delete a product | `catalogItem` (object) |
| `GetDiscount` | Get discount for a day | `day` (dateTime) |

---

## Testing eShopWinForms (Windows Desktop Client)

eShopWinForms is a Windows Forms desktop application that acts as a WCF client to the eShopWCFService.Host. It **cannot run in Docker** — it requires a Windows machine with a GUI.

### Prerequisites

- **Windows 10/11** (64-bit)
- **.NET 10 SDK** installed (download from https://dotnet.microsoft.com/download/dotnet/10.0)
- The eShopWCFService.Host must be running and accessible from the Windows machine

### Step 1: Start the backend services

On any machine (can be the same Windows machine or a remote server), start the Docker Compose stack:

```bash
./test-eshop.sh
```

Or if running on the same Windows machine:
```bash
docker-compose -f docker-compose.legacy.yml up -d
```

### Step 2: Configure the WCF endpoint

The WinForms app connects to the WCF service via the endpoint configured in `App.config`. The default endpoint is:

```
http://localhost:62314/CatalogService.svc
```

You need to update it to point to the Docker-hosted WCF service. Edit the file:

```
eShopLegacyNTier/src/eShopWinForms/App.config
```

Change the endpoint address to match your WCF host:

```xml
<client>
    <endpoint address="http://localhost:5004/CatalogService/CatalogService.svc"
        binding="basicHttpBinding"
        bindingConfiguration="BasicHttpBinding_ICatalogService"
        contract="eShopServiceReference.ICatalogService"
        name="BasicHttpBinding_ICatalogService" />
</client>
```

If the WCF service is running on a different machine, replace `localhost` with that machine's IP address or hostname.

### Step 3: Build the WinForms application

Open a terminal (PowerShell or Command Prompt) on the Windows machine:

```powershell
cd eShopLegacyNTier\src\eShopWinForms
dotnet build
```

### Step 4: Run the WinForms application

```powershell
dotnet run
```

The WinForms application will open a GUI window showing the catalog. It communicates with the WCF service to:
- Browse catalog items (products, brands, types)
- Create, update, and delete products
- View stock information

### Step 5: Verify connectivity

When the app starts, it should display the same catalog products that are shown in the web applications:
- .NET Bot Black Hoodie ($19.50)
- .NET Black & White Mug ($8.50)
- Prism White T-Shirt ($12.00)
- etc.

If you see an error about connection refused, verify:
1. The Docker Compose stack is running (`docker-compose -f docker-compose.legacy.yml ps`)
2. The WCF service is accessible: `curl http://localhost:5004/CatalogService/CatalogService.svc`
3. The `App.config` endpoint address matches the WCF host URL

### Troubleshooting WinForms

| Issue | Solution |
|-------|----------|
| "Could not connect to endpoint" | Verify WCF service is running and `App.config` has correct address |
| Build error on macOS/Linux | WinForms only builds with `dotnet build -p:EnableWindowsTargeting=true` on non-Windows, but can only **run** on Windows |
| "System.Windows.Forms not found" | Ensure you're using .NET 10 SDK with Windows Desktop workload |
| Products not showing | Ensure SQL Server is running and the database has been seeded by first accessing one of the web apps |

---

## Database

All applications share the same SQL Server database:

- **Database name**: `Microsoft.eShopOnContainers.Services.CatalogDb`
- **Connection**: `Server=localhost,1433` (from host) or `Server=sqlserver,1433` (from Docker network)
- **Credentials**: `sa` / `eShop@Pass123`

### Seed Data

The database is automatically seeded when eShopLegacyMVC starts for the first time, creating:
- **5 brands**: Azure, .NET, Visual Studio, SQL Server, Other
- **4 types**: Mug, T-Shirt, Sheet, USB Memory Stick
- **12 catalog items**: Various .NET-themed merchandise

### Connect with a SQL client

```bash
# Using sqlcmd (if available)
sqlcmd -S localhost,1433 -U sa -P "eShop@Pass123" -d "Microsoft.eShopOnContainers.Services.CatalogDb" -Q "SELECT Id, Name, Price FROM Catalog"

# Or from within the SQL Server container
docker exec -it eshop-sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "eShop@Pass123" -d "Microsoft.eShopOnContainers.Services.CatalogDb" -Q "SELECT Id, Name, Price FROM Catalog"
```

---

## Project Structure

```
eshop/
├── docker-compose.legacy.yml              # Docker Compose for all eShopLegacy services
├── test-eshop.sh                          # Automated test script
├── TESTING.md                             # This file
│
├── eShopLegacyMVCSolution/
│   ├── src/eShopLegacyMVC/               # ASP.NET Core MVC + EF6 (port 5001)
│   │   └── Dockerfile
│   ├── eShopPorted/                      # ASP.NET Core MVC + EF Core (port 5002)
│   │   └── Dockerfile
│   └── eShopLegacy.Utilities/            # Shared utilities library
│
├── eShopLegacyWebFormsSolution/
│   └── src/eShopLegacyWebForms/          # Blazor Server + EF6 (port 5003)
│       └── Dockerfile
│
└── eShopLegacyNTier/
    └── src/
        ├── eShopWCFService/              # CoreWCF service library
        ├── eShopWCFService.Host/         # CoreWCF host (port 5004)
        │   └── Dockerfile
        └── eShopWinForms/                # Windows Forms client (Windows only)
```

---

## Known Issues (Pre-existing)

The following are **pre-existing issues from the original .NET Framework to ASP.NET Core modernization** and are NOT related to the current transformation:

### Static Asset/Styling Differences Across Apps

| App | Port | Styling | Notes |
|-----|------|---------|-------|
| **eShopPorted** | 5002 | ✅ Best | All CSS, images, and layout working correctly |
| **eShopLegacyMVC** | 5001 | ⚠️ Partial | Product images missing (`Pics/` folder not in `wwwroot/`); some CSS broken due to case-sensitivity (`site.css` vs `Site.css`) on Linux |
| **eShopLegacyWebForms** | 5003 | ⚠️ Partial | Minor CSS issues from case-sensitivity mismatches on Linux containers |

**Root cause**: In ASP.NET Core, static files must be served from `wwwroot/`. The original modernization placed some asset folders (`Pics/`, `Content/`) at the project root rather than inside `wwwroot/`. Additionally, Linux file systems are case-sensitive, so `site.css` ≠ `Site.css`.

**Impact**: Functional behavior is correct (data loads, CRUD operations work) — only visual presentation is affected.

---

## Environment Variables

All connection strings can be overridden via environment variables:

| App | Variable | Default |
|-----|----------|---------|
| eShopLegacyMVC | `ConnectionStrings__CatalogDBContext` | See appsettings.json |
| eShopPorted | `ConnectionStrings__DefaultConnection` | See appsettings.json |
| eShopLegacyWebForms | `ConnectionStrings__CatalogDBContext` | See appsettings.json |
| eShopWCFService.Host | `ConnectionStrings__EntityModel` | See appsettings.json |

Mock data can be enabled by setting:
- `UseMockData=true` (eShopLegacyMVC, eShopPorted)
- `AppSettings__UseMockData=true` (eShopLegacyWebForms)
