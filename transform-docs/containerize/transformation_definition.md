# Fix eShopLegacy Build Errors and Containerize for ECS

## Objective

Resolve all remaining build errors in the three eShopLegacy solutions (eShopLegacyMVCSolution, eShopLegacyNTier, eShopLegacyWebFormsSolution) after their modernization from .NET Framework 4.x to .NET 10, create a new ASP.NET Core host project for the CoreWCF service in eShopLegacyNTier so it can run as a containerizable web service, and ensure all web applications plus the new CoreWCF host are compatible with Linux containers for deployment on Amazon ECS. The eShopModernized solutions must be completely ignored.

## Summary

The eShopLegacy applications were modernized from .NET Framework 4.x to .NET 10 but have build errors to fix across all three solutions. eShopLegacyWebForms (a Blazor Server app) is missing its Program.cs entry point and needs one that bootstraps Blazor Server with Autofac DI and Entity Framework 6. eShopPorted has EF Core package version misalignment (mix of 9.0.x and 10.0.0 packages) causing NU1605. eShopWinForms has an incompatible System.Windows.Forms package reference pinned at version [4.0.0, 4.0.0] and needs its TFM changed to net10.0-windows with UseWindowsForms instead. Beyond fixing build errors, the eShopLegacyNTier solution requires a brand-new ASP.NET Core host project for eShopWCFService (which is a CoreWCF class library with no entry point) so the WCF service can be hosted as a Linux-containerizable web service. The WinForms client remains a Windows desktop app and will not be containerized.

## Entry Criteria

1. The repository contains the three eShopLegacy solutions: eShopLegacyMVCSolution, eShopLegacyNTier, and eShopLegacyWebFormsSolution
2. All projects within the eShopLegacy solutions already target net10.0 (or will be changed to net10.0-windows for WinForms) and use SDK-style .csproj files
3. The eShopLegacyWebFormsSolution/src/eShopLegacyWebForms project is missing a Program.cs file and fails with error CS5001 (no static Main method entry point). It is a Blazor Server app with Components/App.razor as the root component, Blazor component structure under Components/Pages/ and Components/Layout/, _Imports.razor with proper namespaces, Autofac DI via ApplicationModule.cs (registering CatalogService, CatalogServiceMock, CatalogDBContext, CatalogDBInitializer, CatalogItemHiLoGenerator), and Entity Framework 6 (not EF Core) for data access
4. The eShopLegacyNTier/src/eShopWinForms project has a PackageReference to System.Windows.Forms pinned at version [4.0.0, 4.0.0] which is incompatible with the net10.0 TFM, and has wildcard version PackageReferences that need to be pinned
5. The eShopLegacyMVCSolution/eShopPorted project has EF Core package version misalignment: Microsoft.EntityFrameworkCore at 9.0.10, Microsoft.EntityFrameworkCore.Design at 9.0.10, Microsoft.EntityFrameworkCore.SqlServer at 9.0.9, while Microsoft.EntityFrameworkCore.Relational is at 10.0.0, producing error NU1605
6. The eShopLegacyNTier/src/eShopWCFService project is a CoreWCF service library (OutputType Library) using CoreWCF.Primitives, CoreWCF.Http, and CoreWCF.NetTcp, but has no host project and no entry point - it cannot run on its own
7. The eShopLegacyNTier solution contains only eShopWCFService and eShopWinForms as active projects. Any leftover build artifacts from a previously removed eShopWCFService.Host directory must be ignored
8. The eShopModernized solutions (eShopModernizedMVCSolution, eShopModernizedNTier, eShopModernizedWebFormsSolution) exist in the repository but must not be modified
9. The eShopLegacyMVCSolution contains eShopLegacyMVC (ASP.NET Core MVC web app), eShopPorted (ASP.NET Core web app), and eShopLegacy.Utilities (class library). Only eShopPorted has build errors

## Implementation Steps

### Solution 1: eShopLegacyWebFormsSolution - Fix CS5001 Missing Entry Point

1. Locate the eShopLegacyWebFormsSolution/src/eShopLegacyWebForms project directory
2. Verify that no Program.cs file exists in the project
3. Create a new Program.cs file in the eShopLegacyWebFormsSolution/src/eShopLegacyWebForms project directory with a Blazor Server application entry point using the minimal hosting model. The file should:
   - Call WebApplication.CreateBuilder(args) to create the application builder
   - Register Autofac as the service provider factory using builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory()) and builder.Host.ConfigureContainer<ContainerBuilder>() to register the ApplicationModule which handles DI for CatalogService, CatalogServiceMock, CatalogDBContext, CatalogDBInitializer, and CatalogItemHiLoGenerator
   - Register Blazor Server services with builder.Services.AddRazorComponents() and AddInteractiveServerComponents()
   - Register Entity Framework 6 DbContext (CatalogDBContext) and any connection string configuration from appsettings.json. Note this uses EntityFramework 6, not EF Core, so registration should be appropriate for EF6 patterns
   - Configure the middleware pipeline with app.UseStaticFiles(), app.UseRouting(), app.UseAntiforgery(), and app.MapRazorComponents<App>() with AddInteractiveServerRenderMode()
   - Use the existing appsettings.json configuration for connection strings and app settings
   - Reference the Components/App.razor as the root component
4. Verify the Program.cs compiles with the existing project structure and references the correct namespaces from the eShopLegacyWebForms project

### Solution 2: eShopLegacyNTier - Fix eShopWinForms Build Errors

5. Open the eShopLegacyNTier/src/eShopWinForms/eShopWinForms.csproj file
6. Change the TargetFramework from net10.0 to net10.0-windows to enable the Windows Desktop SDK workload, since this is a Windows Forms desktop application that requires the Windows Desktop runtime APIs
7. Add or verify that the UseWindowsForms property is set to true in the csproj PropertyGroup
8. Remove the explicit PackageReference to System.Windows.Forms that is pinned at version [4.0.0, 4.0.0], as Windows Forms support is provided implicitly by the SDK when targeting net10.0-windows with UseWindowsForms enabled
9. Pin any wildcard version PackageReference entries (Version="*") to explicit stable versions compatible with .NET 10 to prevent non-deterministic builds. Specifically, pin System.ServiceModel.Http, System.ServiceModel.NetTcp, and System.ServiceModel.Primitives to their latest stable versions for .NET 10
10. Do not modify the CatalogServiceClient (System.ServiceModel.ClientBase) connectivity code - the WinForms app will continue to connect to the WCF service as a client

### Solution 2: eShopLegacyNTier - Create ASP.NET Core Host for CoreWCF Service

11. Create a new ASP.NET Core host project directory at eShopLegacyNTier/src/eShopWCFService.Host (or similar appropriate location within the eShopLegacyNTier solution structure)
12. Create an SDK-style .csproj file for the host project targeting net10.0 with OutputType Exe. Add PackageReferences for CoreWCF.Http and CoreWCF.NetTcp (matching the versions used by eShopWCFService) and a ProjectReference to the eShopWCFService class library
13. Create a Program.cs in the host project that:
    - Uses WebApplication.CreateBuilder(args) for the minimal hosting model
    - Adds CoreWCF services with builder.Services.AddServiceModelServices() and builder.Services.AddServiceModelMetadata()
    - Configures CoreWCF endpoints for the WCF service contracts defined in eShopWCFService (binding the appropriate HTTP and/or NetTcp endpoints)
    - Exposes the service on HTTP endpoints suitable for Linux container deployment
    - Registers any dependencies the WCF service implementation requires
14. Add the new host project to the eShopLegacyNTier.sln solution file
15. Ignore any leftover build artifacts in any previously existing eShopWCFService.Host directory - if stale files exist, they should be replaced by the newly created project

### Solution 3: eShopLegacyMVCSolution - Fix NU1605 EF Core Version Downgrade

16. Open the eShopLegacyMVCSolution/eShopPorted/eShopPorted.csproj file
17. Update the PackageReference for Microsoft.EntityFrameworkCore from version 9.0.10 to 10.0.0 to align with the version required by Microsoft.EntityFrameworkCore.Relational 10.0.0
18. Update the PackageReference for Microsoft.EntityFrameworkCore.Design from version 9.0.10 to 10.0.0 to maintain version alignment across all EF Core packages
19. Update the PackageReference for Microsoft.EntityFrameworkCore.SqlServer from version 9.0.9 to 10.0.0 to maintain version alignment across all EF Core packages
20. Confirm that no other NuGet package version conflicts exist in the eShopPorted project after the version alignment

### Scope Exclusion Verification

21. Confirm that no files within the eShopModernizedMVCSolution, eShopModernizedNTier, or eShopModernizedWebFormsSolution directories have been modified
22. Confirm that no files within eShopLegacyMVCSolution/src/eShopLegacyMVC or eShopLegacyMVCSolution/eShopLegacy.Utilities have been modified, as those projects have zero build errors
23. Confirm that the eShopWCFService class library itself has not been modified (only a new host project was created that references it)

## Validation / Exit Criteria

1. Running dotnet build on the eShopLegacyWebFormsSolution solution produces no CS5001 errors and the eShopLegacyWebForms project compiles successfully with a valid Blazor Server entry point that uses Autofac DI and Entity Framework 6
2. Running dotnet build on the eShopLegacyNTier solution produces no build errors for eShopWinForms under the net10.0-windows TFM with UseWindowsForms enabled and no explicit System.Windows.Forms PackageReference
3. The new eShopWCFService.Host project compiles successfully, references the eShopWCFService class library, and hosts the CoreWCF service with HTTP endpoints suitable for Linux container deployment
4. Running dotnet build on the eShopLegacyMVCSolution solution produces no NU1605 errors and the eShopPorted project restores with all EF Core packages (Microsoft.EntityFrameworkCore, Microsoft.EntityFrameworkCore.Design, Microsoft.EntityFrameworkCore.SqlServer, Microsoft.EntityFrameworkCore.Relational) aligned at version 10.0.0
5. All three eShopLegacy solutions build without any errors (zero build errors total across all projects, including the new host project)
6. No files in the eShopModernizedMVCSolution, eShopModernizedNTier, or eShopModernizedWebFormsSolution directories have been modified
7. No files in the eShopLegacy.Utilities, eShopLegacyMVC, or eShopWCFService (the class library, not the host) projects have been modified
8. The eShopLegacyWebForms Program.cs correctly bootstraps a Blazor Server application using the Components/App.razor root component, registers Autofac DI via ApplicationModule, and configures Entity Framework 6 data access
9. The eShopWinForms.csproj targets net10.0-windows, has UseWindowsForms set to true, does not contain an explicit PackageReference to System.Windows.Forms, and has all wildcard package versions pinned
10. The eShopPorted.csproj has all Microsoft.EntityFrameworkCore packages aligned at version 10.0.0
11. The three web applications (eShopLegacyMVC, eShopPorted, eShopLegacyWebForms) and the new eShopWCFService.Host are all Linux container compatible (no Windows-only dependencies, no System.Windows.Forms, no Windows-specific APIs)
12. The eShopWinForms project compiles successfully as a Windows desktop application (net10.0-windows) but is not expected to be Linux container compatible
