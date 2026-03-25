

eShopModernizing Repository
Repository Overview

Solutions

eShopModernizedMVC.sln
Solution Transformation Summary
Transformation Overview
Linux Readiness

Projects

eShopModernizedMVC

eShopLegacyNTier.sln
Solution Transformation Summary
Transformation Overview
Linux Readiness

Projects

eShopModernizedNTier.sln
Solution Transformation Summary
Transformation Overview
Linux Readiness

Projects

eShopLegacyWebForms.sln
Solution Transformation Summary
Transformation Overview
Linux Readiness

Projects

eShopModernizedWebForms.sln
Solution Transformation Summary
Transformation Overview
Linux Readiness

Projects

eShopLegacyMVC.sln
Solution Transformation Summary
Transformation Overview
Linux Readiness

Projects
Repository Transformation Report
Expand All
Collapse All
Comprehensive report of repository transformation results

Repository Name
eShopModernizing
Job ID
7caab669-3e44-494b-9785-85635936b22a
Transformation Overview
Detailed transformation results overview
Projects
Total
10
Completely Transformed
6
Partially Transformed
4
Skipped
0
NuGet Packages
Added
99
Removed
177
Updated
144
Files
Added
32
Removed
103
Updated
74
Moved
303
NuGet Changes Overview
Summary of NuGet package changes across all solutions
Packages Added
99
Packages Removed
177
Packages Updated
144
File Changes Overview
Summary of file changes across all solutions
Files Added
32
Files Removed
103
Files Updated
74
Files Moved
303
Test Results & Other Metrics
Aggregated test results and other metrics across all solutions
Tests Passed
0
Tests Failed
0
Tests Skipped
0
Build Errors
4
Linux Recommendations
144
eShopModernizedMVC.sln
Download NuGet Package Changes
Transformation details for eShopModernizedMVC.sln

Solution Transformation Summary
Details about the transformation settings and configuration
eShopModernizedMVC – Modernization Summary
Project Infrastructure
The project migrates from .NET Framework 4.7.2 MSBuild format to SDK-style Microsoft.NET.Sdk.Web targeting .NET 10, with OutputType changing from Library to Exe. All <Reference> elements with <HintPath> DLL pointers and the packages.config manifest (149 entries) are replaced with <PackageReference> elements. Legacy MSBuild targets including Microsoft.WebApplication.targets, MvcBuildViews, AspNetCompiler, ApplicationInsights, and EntityFramework .targets imports are removed.

Entry Point and Startup
Global.asax/Global.asax.cs and the HttpApplication lifecycle are replaced by Program.cs using the minimal hosting model (WebApplication.CreateBuilder). All App_Start/ files are removed, including BundleConfig.cs, RouteConfig.cs, FilterConfig.cs, CatalogConfiguration.cs, Startup.Auth.cs, SqlAccessTokenProvider.cs, MyTelemetryInitializer.cs, and OptionalKeyVaultConfigurationBuilder.cs. Route registration moves to MapControllerRoute in Program.cs. Properties/AssemblyInfo.cs is replaced by SDK-style project metadata.

Authentication and Middleware
The OWIN pipeline (Microsoft.Owin.*, IAppBuilder, OwinMiddleware) is replaced with ASP.NET Core middleware (RequestDelegate, IApplicationBuilder, app.UseMiddleware<T>()). AccountController replaces HttpContext.GetOwinContext().Authentication with HttpContext.ChallengeAsync/HttpContext.SignOutAsync. Microsoft.IdentityModel.* packages are updated from 5.6.0 to 8.15.x–8.16.x, the legacy Microsoft.IdentityModel.Protocol.Extensions is dropped, and Microsoft.AspNetCore.Authentication.OpenIdConnect 10.0.3 is added.

Configuration
ConfigurationManager/Web.config and Microsoft.Configuration.ConfigurationBuilders.* (Azure, Base, Environment, UserSecrets) are removed. Configuration now uses the Microsoft.Extensions.Configuration.* pipeline natively via IConfiguration.

Azure and Storage
WindowsAzure.Storage (CloudStorageAccount/CloudBlobClient/CloudBlockBlob) and WindowsAzure.ServiceBus are removed. ImageAzureStorage.cs replaces the Azure Blob Storage SDK with direct REST API calls using HttpClient with SharedKey HMAC-SHA256 signing. Account name and key are currently hardcoded as placeholders ("youraccount", "yourkey") and must be replaced with secure configuration values before deployment. Microsoft.Azure.KeyVault is updated from 3.0.4 to 3.0.5 and Microsoft.Azure.Services.AppAuthentication from 1.3.1 to 1.6.2.

Data Access
Microsoft.Data.SqlClient 6.1.3 is added. The CatalogDBContext constructor parameter changes from ISqlConnectionFactory to DbConnection. ManagedIdentitySqlConnectionFactory, AzureServiceTokenProvider, and ISqlConnectionFactory are removed from ApplicationModule.cs and SqlAccessTokenProvider.cs.

Controllers and Models
CatalogController: HttpStatusCodeResult → StatusCodeResult; HttpNotFound() → NotFound(); [Bind(Include=...)] → [Bind(...)]
PicController: HttpPostedFile → IFormFile; System.Drawing.Image format validation replaced with MIME type string check; IHttpContextAccessor injected for form file access
IImageService: HttpPostedFile → IFormFile
ImageMockStorage: HttpContext.Current.Server.MapPath → IWebHostEnvironment.WebRootPath
Views
System.Web.Optimization bundle helpers (@Styles.Render/@Scripts.Render) are replaced with explicit <link> and <script> tags in _Layout.cshtml, Create.cshtml, and Edit.cshtml. HttpContext.Current.Session is replaced with Context.Session.GetString. Request.IsAuthenticated is replaced with User.Identity.IsAuthenticated in _LoginPartial.cshtml. _ViewImports.cshtml is added with @addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers.

Static Assets
All static files are relocated from root-level directories (/Content, /Scripts, /Images, /Pics) to wwwroot/, required by ASP.NET Core's StaticFiles middleware (app.UseStaticFiles()). No file contents are modified. All hardcoded paths referencing the original directories must be updated to wwwroot/-relative paths.

Dependency Updates
| Package | From | To | |---|---|---| | Autofac | 4.9.4 | 8.4.0 | | bootstrap | 4.3.1 | 5.3.1 | | jQuery | 3.5.0 | 3.7.0 | | jQuery.Validation | 1.19.4 | 1.19.5 | | log4net | 2.0.12 | 3.2.0 | | Newtonsoft.Json | 12.0.3 | 13.0.4 | | System.Text.Json | 4.6.0 | 9.0.9 | | Microsoft.Rest.ClientRuntime | 2.3.20 | 3.0.3 | | Microsoft.Rest.ClientRuntime.Azure | 3.3.19 | 4.0.3 |

Autofac.Mvc5 is removed as there is no MVC5 target. Microsoft.AspNetCore.SystemWebAdapters 2.3.0 and Microsoft.AspNetCore.Owin 10.0.3 are added.

Linux Readiness
Details about the Linux readiness assessment and recommendations
eShopModernized — Linux Readiness
Linux Compatibility Assessment
Executive Summary
The eShopModernized solution has been assessed across 1 project: eShopModernizedMVC. The project targets ASP.NET Core (10.0.0) and establishes a cross-platform architectural foundation. The combined assessment identifies 15 total findings: 13 high-severity and 2 low-severity items. The high-severity flags are predominantly rooted in analyzer limitations recognizing core ASP.NET Core SDK components — WebApplication, IApplicationBuilder, ControllerBase, and MVC routing — which are natively cross-platform. Two targeted areas require verification before production deployment.

Items Requiring Attention
Two packages are flagged as cross-platform incompatible:

Microsoft.AspNetCore.Diagnostics-10.0.0-NUGET — flagged as incompatible
System.Configuration.ConfigurationManager-10.0.0-NUGET — flagged as incompatible
One configuration-level concern:

IIS-based authentication/authorization (IISConfigFeature) — High severity; IIS-specific auth configuration does not apply to Linux environments
Recommended Actions:

Validate whether System.Configuration.ConfigurationManager usage can be replaced with the ASP.NET Core configuration system already present in the project
Identify and migrate any IIS-specific authentication bindings to a Linux-compatible middleware or identity provider
Additional Verification Items
File permission APIs (FileAccessControlFeature) detected — behavior differences exist between Windows ACLs and Linux permission models
External service connectivity (HttpLibraryFeature) — HTTP client usage requires endpoint accessibility validation in the Linux target environment
Recommended Validation:

Verify that file permission logic produces expected results under Linux's POSIX permission model
Confirm all external service endpoints are reachable from the Linux deployment environment
Validate IIS authentication replacement functions correctly end-to-end on Linux
Dependencies Review
| Package | Status | |---|---| | Microsoft.AspNetCore.Diagnostics-10.0.0-NUGET | Flagged cross-platform incompatible | | System.Configuration.ConfigurationManager-10.0.0-NUGET | Flagged cross-platform incompatible | | Microsoft.AspNetCore.Mvc.ViewFeatures-10.0.0-SDK | Core SDK — natively cross-platform | | Microsoft.AspNetCore.Mvc.Core-10.0.0-SDK | Core SDK — natively cross-platform | | Microsoft.AspNetCore-10.0.0-SDK | Core SDK — natively cross-platform | | Microsoft.AspNetCore.Http.Abstractions-10.0.0-SDK | Core SDK — natively cross-platform |

Recommended Updates:

Investigate the specific usage of Microsoft.AspNetCore.Diagnostics to determine whether the flagged incompatibility applies to the active code paths
Scope System.Configuration.ConfigurationManager references to confirm whether they can be retired in favor of the built-in ASP.NET Core configuration pipeline already in use
Transformation Overview
COMPLETELY TRANSFORMED
Detailed transformation results overview

Projects
Total
1
Completely Transformed
1
Partially Transformed
0
Skipped
0
NuGet Packages
Added
18
Removed
67
Updated
49
Files
Added
2
Removed
12
Updated
23
Moved
57
Projects (1)
List of projects in the transformation


1


Project Name
	
Status
	
Files Updated
	
Files Removed
	
Files Added
	
NuGet Added
	
NuGet Removed
	
NuGet Updated
	
Build Errors

Project Name
	
Status
	
Files Updated
	
Files Removed
	
Files Added
	
NuGet Added
	
NuGet Removed
	
NuGet Updated
	
Build Errors

eShopModernizedMVC
COMPLETELY TRANSFORMED
23
12
2
18
67
49
0
eShopModernizedMVC
COMPLETELY TRANSFORMED
Transformation details for eShopModernizedMVCSolution/src/eShopModernizedMVC/wwwroot/favicon.ico

Project Summary
eShopModernizedMVC – Modernization Summary
Project Infrastructure
The project transitions from legacy .NET Framework 4.7.2 MSBuild format to SDK-style Microsoft.NET.Sdk.Web targeting .NET 10. The OutputType changes from Library to Exe. All <Reference> elements with <HintPath> DLL pointers and the packages.config manifest (149 entries) are replaced with <PackageReference> elements. Legacy MSBuild targets (Microsoft.WebApplication.targets, MvcBuildViews, AspNetCompiler, ApplicationInsights and EntityFramework .targets imports) are removed.

Entry Point and Startup
Global.asax/Global.asax.cs and the HttpApplication lifecycle are replaced by Program.cs using the minimal hosting model (WebApplication.CreateBuilder). All App_Start/ files (BundleConfig.cs, RouteConfig.cs, FilterConfig.cs, CatalogConfiguration.cs, Startup.Auth.cs, SqlAccessTokenProvider.cs, MyTelemetryInitializer.cs, OptionalKeyVaultConfigurationBuilder.cs) are removed. Route registration moves to MapControllerRoute in Program.cs. Properties/AssemblyInfo.cs is replaced by SDK-style project metadata.

Authentication and Middleware
The OWIN pipeline (Microsoft.Owin.*, IAppBuilder, OwinMiddleware) is replaced with ASP.NET Core middleware (RequestDelegate, IApplicationBuilder, app.UseMiddleware<T>()). AccountController replaces HttpContext.GetOwinContext().Authentication with HttpContext.ChallengeAsync/HttpContext.SignOutAsync. Microsoft.IdentityModel.* packages are updated from 5.6.0 to 8.15.x–8.16.x. Microsoft.IdentityModel.Protocol.Extensions (legacy) is dropped. Microsoft.AspNetCore.Authentication.OpenIdConnect 10.0.3 is added.

Configuration
ConfigurationManager/Web.config and Microsoft.Configuration.ConfigurationBuilders.* (Azure, Base, Environment, UserSecrets) are removed. Configuration now uses the Microsoft.Extensions.Configuration.* pipeline natively via IConfiguration.

Azure and Storage
WindowsAzure.Storage (CloudStorageAccount/CloudBlobClient/CloudBlockBlob) and WindowsAzure.ServiceBus are removed. ImageAzureStorage.cs replaces the Azure Blob Storage SDK with direct REST API calls using HttpClient with SharedKey HMAC-SHA256 signing. Account name and key are currently hardcoded as placeholders ("youraccount", "yourkey") — these must be replaced with secure configuration values before deployment. Microsoft.Azure.KeyVault is updated from 3.0.4 to 3.0.5 and Microsoft.Azure.Services.AppAuthentication from 1.3.1 to 1.6.2.

Data Access
Microsoft.Data.SqlClient 6.1.3 is added. CatalogDBContext constructor parameter changes from ISqlConnectionFactory to DbConnection. ManagedIdentitySqlConnectionFactory, AzureServiceTokenProvider, and ISqlConnectionFactory are removed from ApplicationModule.cs and SqlAccessTokenProvider.cs.

Controllers and Models
CatalogController: HttpStatusCodeResult → StatusCodeResult; HttpNotFound() → NotFound(); [Bind(Include=...)] → [Bind(...)]
PicController: HttpPostedFile → IFormFile; System.Drawing.Image format validation replaced with MIME type string check; IHttpContextAccessor injected for form file access
IImageService: HttpPostedFile → IFormFile
ImageMockStorage: HttpContext.Current.Server.MapPath → IWebHostEnvironment.WebRootPath
Views
System.Web.Optimization bundle helpers (@Styles.Render/@Scripts.Render) are replaced with explicit <link> and <script> tags in _Layout.cshtml, Create.cshtml, and Edit.cshtml. HttpContext.Current.Session is replaced with Context.Session.GetString. Request.IsAuthenticated is replaced with User.Identity.IsAuthenticated in _LoginPartial.cshtml. _ViewImports.cshtml is added with @addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers.

Static Assets
All static files are relocated from root-level directories (/Content, /Scripts, /Images, /Pics) to wwwroot/, required by ASP.NET Core's StaticFiles middleware (app.UseStaticFiles()). No file contents are modified. All hardcoded paths referencing the original directories must be updated to wwwroot/-relative paths.

Dependency Updates
| Package | From | To | |---|---|---| | Autofac | 4.9.4 | 8.4.0 | | bootstrap | 4.3.1 | 5.3.1 | | jQuery | 3.5.0 | 3.7.0 | | jQuery.Validation | 1.19.4 | 1.19.5 | | log4net | 2.0.12 | 3.2.0 | | Newtonsoft.Json | 12.0.3 | 13.0.4 | | System.Text.Json | 4.6.0 | 9.0.9 | | Microsoft.Rest.ClientRuntime | 2.3.20 | 3.0.3 | | Microsoft.Rest.ClientRuntime.Azure | 3.3.19 | 4.0.3 |

Autofac.Mvc5 is removed (no MVC5 target). Microsoft.AspNetCore.SystemWebAdapters 2.3.0 and Microsoft.AspNetCore.Owin 10.0.3 are added.

NuGet Package Changes (134)
List of nuget package changes in the transformation


1
2
3
4
5
6
7


Package Name
	
Change Type
	
Old Version
	
New Version

Package Name
	
Change Type
	
Old Version
	
New Version

Microsoft.AspNetCore.SystemWebAdapters
added
-
2.3.0
System.Configuration.ConfigurationManager
added
-
*
Microsoft.Data.SqlClient
added
-
6.1.3
Microsoft.AspNetCore.Owin
added
-
10.0.3
Microsoft.AspNetCore.Authentication.OpenIdConnect
added
-
10.0.3
Microsoft.AspNetCore.Diagnostics
added
-
*
bootstrap
added
-
5.3.1
jQuery
added
-
3.7.0
jQuery.Validation
added
-
1.19.5
Microsoft.ApplicationInsights.DependencyCollector
added
-
2.23.0
Microsoft.ApplicationInsights.Log4NetAppender
added
-
2.23.0
Microsoft.ApplicationInsights.PerfCounterCollector
added
-
2.23.0
Microsoft.ApplicationInsights.Web
added
-
2.21.0
Microsoft.ApplicationInsights.WindowsServer
added
-
2.23.0
Microsoft.ApplicationInsights.WindowsServer.TelemetryChannel
added
-
2.23.0
Microsoft.jQuery.Unobtrusive.Validation
added
-
4.0.0
Microsoft.NETCore.Platforms
added
-
7.0.4
popper.js
added
-
1.16.1
Antlr3.Runtime
removed
3.5.0.2
-
Autofac.Integration.Mvc
removed
4.0.0.0
-
File Changes (43)
List of file changes in the transformation


1
2
3


File(s)
	
Change Type
	
Summary
	
Code Diff

File(s)
	
Change Type
	
Summary
	
Code Diff

eShopModernizedMVCSolution/src/eShopModernizedMVC/Program.cs
added
Deleted because ASP.NET Core has no OWIN pipeline. Authentication configuration previously handled by this partial Startup class is now registered directly in Program.cs using ASP.NET Core's DI and middleware pipeline, making a separate OWIN bootstrap file obsolete.
View file
eShopModernizedMVCSolution/src/eShopModernizedMVC/Views/_ViewImports.cshtml
added
Created as a new file to replace the namespace imports previously registered globally in ASP.NET MVC's Web.config. It consolidates all Razor view namespaces and enables Tag Helpers via `@addTagHelper`, which is the ASP.NET Core mechanism replacing HTML Helpers registered through configuration.
View file
Files moved to wwwroot directory related changes
moved
Moving `favicon.ico` into the `wwwroot` directory aligns with ASP.NET Core's static file serving convention, where only files within `wwwroot` are publicly accessible by default. This replaces the legacy ASP.NET approach of serving static files from the project root, ensuring the Static Files Middleware correctly serves the favicon without custom routing configuration.
View 2 files
Files moved to Content directory related changes
moved
These CSS files were relocated from a legacy ASP.NET MVC `Content` folder convention into the ASP.NET Core `wwwroot/Content` directory, aligning static asset storage with ASP.NET Core's required web root structure. This move enables the static file middleware to serve these assets directly over HTTP, which is mandatory for proper functioning in the modernized ASP.NET Core MVC application.
View 9 files
Files moved to Images directory related changes
moved
These four UI image assets — brand logo, dark brand variant, main banner, and footer text — were relocated into a dedicated `Images` directory under `wwwroot`. This consolidates all static image resources into a single, organized folder, establishing a clear separation from other static asset types (CSS, JavaScript) and aligning with ASP.NET Core's conventional `wwwroot` static file organization structure.
View 4 files
Files moved to Scripts directory related changes
moved
These JavaScript library files — jQuery, Bootstrap, Popper, Modernizr, Respond, and validation plugins — were consolidated from their previous scattered locations into a single `wwwroot/Scripts` directory. This reorganization establishes a standardized static asset structure aligned with ASP.NET Core conventions, centralizing all client-side script dependencies under `wwwroot` to enable proper static file serving and improve maintainability.
View 28 files
Files moved to esm directory related changes
moved
Four Popper.js files (both full and minified versions of `popper.js` and `popper-utils.js`) were relocated into a dedicated `esm` subdirectory under `Scripts`. This reorganization separates ES Module formatted JavaScript from other script formats, enabling the application to serve modern module-based scripts distinctly from legacy UMD/CommonJS bundles, improving script organization and supporting browser-native ES module consumption.
View 4 files
Files moved to umd directory related changes
moved
These four Popper.js files (full and minified versions of both popper.js and popper-utils.js) were relocated into a dedicated `umd` subdirectory to explicitly organize scripts by their module format. UMD (Universal Module Definition) builds support multiple environments, and isolating them ensures clear separation from other script formats, improving maintainability and dependency resolution within the modernized MVC application.
View 4 files
eShopModernizedMVCSolution/src/eShopModernizedMVC/App_Start/BundleConfig.cs
removed
Deleted because `System.Web.Optimization` bundling is exclusive to ASP.NET Framework and has no equivalent in ASP.NET Core. The `_Layout.cshtml` changes show the replacement strategy: explicit individual `<link>` and `<script>` tags, with ASP.NET Core's built-in static file middleware serving assets directly.
View file
eShopModernizedMVCSolution/src/eShopModernizedMVC/App_Start/CatalogConfiguration.cs
removed
Deleted because it depended entirely on `System.Configuration.ConfigurationManager`, which is an ASP.NET Framework API. ASP.NET Core replaces this with `IConfiguration` injected via dependency injection, requiring all configuration access points that referenced this static class to be refactored accordingly.
View file
eShopModernizedMVCSolution/src/eShopModernizedMVC/App_Start/FilterConfig.cs
removed
Deleted because ASP.NET Core registers global filters through `MvcOptions` in `Startup.cs`, not through a static `FilterConfig` class. The App_Start pattern is obsolete in ASP.NET Core.
View file
eShopModernizedMVCSolution/src/eShopModernizedMVC/App_Start/MyTelemetryInitializer.cs
removed
Deleted because Application Insights role name configuration in ASP.NET Core is handled differently, through `TelemetryConfiguration` in `Startup.cs`, making this legacy initializer class redundant.
View file
eShopModernizedMVCSolution/src/eShopModernizedMVC/App_Start/OptionalKeyVaultConfigurationBuilder.cs
removed
Deleted because ASP.NET Core's `IConfiguration` system handles Azure Key Vault integration natively via `AddAzureKeyVault`, making this workaround for the legacy `Microsoft.Configuration.ConfigurationBuilders` package unnecessary.
View file
eShopModernizedMVCSolution/src/eShopModernizedMVC/App_Start/RouteConfig.cs
removed
Deleted because ASP.NET Core configures routing via middleware in `Startup.cs` using `UseRouting()` and `UseEndpoints()`, replacing the static `RouteConfig.RegisterRoutes` App_Start pattern entirely.
View file
eShopModernizedMVCSolution/src/eShopModernizedMVC/App_Start/SqlAccessTokenProvider.cs
removed
Deleted because `System.Configuration.ConfigurationManager` and `Microsoft.Azure.Services.AppAuthentication` are ASP.NET Framework dependencies. ASP.NET Core uses `IConfiguration` for connection strings and `Azure.Identity` for managed identity, making this class obsolete and requiring a replacement implementation aligned with Core conventions.
View file
eShopModernizedMVCSolution/src/eShopModernizedMVC/App_Start/Startup.Auth.cs
removed
Deleted because ASP.NET Core has no OWIN pipeline. Authentication configuration previously handled by this partial Startup class is now registered directly in Program.cs using ASP.NET Core's DI and middleware pipeline, making a separate OWIN bootstrap file obsolete.
View file
eShopModernizedMVCSolution/src/eShopModernizedMVC/Global.asax
removed
Deleted entirely because ASP.NET Core eliminated the HttpApplication pipeline model. Application startup logic migrates to Program.cs and Startup.cs, making Global.asax architecturally obsolete.
View file
eShopModernizedMVCSolution/src/eShopModernizedMVC/Global.asax.cs
removed
Deleted entirely because the migration from ASP.NET MVC (System.Web) to ASP.NET Core eliminates the HttpApplication lifecycle model. ASP.NET Core replaces Global.asax with Program.cs/Startup.cs, where dependency injection, middleware pipelines, and application initialization are handled natively, making this file architecturally obsolete.
View file
eShopModernizedMVCSolution/src/eShopModernizedMVC/Properties/AssemblyInfo.cs
removed
Deleted because .NET Core/.NET 5+ projects use SDK-style project files where assembly metadata is declared in the `.csproj` file via MSBuild properties. The `log4net.Config.XmlConfigurator` attribute was also removed, indicating a logging framework migration away from log4net.
View file
eShopModernizedMVCSolution/src/eShopModernizedMVC/packages.config
removed
This file was deleted as part of migrating from .NET Framework 4.7.2 to SDK-style project format (.NET Core/.NET 5+). The legacy `packages.config` NuGet dependency management system is replaced by `PackageReference` entries directly inside the `.csproj` file, eliminating the need for this separate XML manifest that tracked 149 packages including ASP.NET MVC5, OWIN, and classic Azure SDK dependencies.
View file
eShopLegacyNTier.sln
Download NuGet Package Changes
Transformation details for eShopLegacyNTier.sln

Solution Transformation Summary
Details about the transformation settings and configuration
eShopWinForms and eShopWCFService Modernization Summary
Both the eShopWinForms and eShopWCFService projects are upgraded from .NET Framework to .NET 10, with project files converted from legacy MSBuild XML format to SDK-style .csproj format. Package management moves from packages.config to PackageReference in both projects, and AssemblyInfo.cs files are removed in favor of SDK-style assembly metadata defaults.

eShopWinForms
Target framework updated from net47 to net10.0
Package versions updated: EntityFramework (6.1.3 → 6.5.1), Microsoft.AspNet.WebApi.Client (5.2.3 → 6.0.0), Newtonsoft.Json (6.0.4 → 13.0.4)
New PackageReference entries added for System.ServiceModel.Primitives, System.ServiceModel.Http, System.ServiceModel.NetTcp, System.Resources.Extensions, and System.Windows.Forms
Explicit <Reference>, <Compile>, and <Content> items removed; several Helpers/ source files explicitly excluded via <Compile Remove="..."> entries
ClickOnce publish settings removed
CatalogServiceClient constructor overloads updated: named endpoint configuration from app.config/web.config is no longer used; endpointConfigurationName parameters replaced with null, and endpoints must now be configured programmatically
eShopWCFService
Target framework updated from v4.6.1 to net10.0, with output type remaining Library
System.ServiceModel and System.ServiceModel.Web assembly references replaced with CoreWCF.Primitives, CoreWCF.Http, and CoreWCF.NetTcp NuGet packages
Microsoft.AspNetCore and Microsoft.AspNetCore.SystemWebAdapters 2.3.0 added; EntityFramework upgraded from 6.1.3 to 6.5.1
using System.ServiceModel and using System.ServiceModel.Web replaced with using CoreWCF in CatalogService.svc.cs and ICatalogService.cs
IIS Express, ProjectTypeGuids, UseIIS, and web application configuration removed
CatalogServiceClient.cs excluded from compilation; client-side proxy code referencing System.ServiceModel.ClientBase<T> is not yet ported
.svc file hosting model no longer applies; CoreWCF requires ASP.NET Core middleware hosting
Linux Readiness
Details about the Linux readiness assessment and recommendations
eShop Solution
Linux Compatibility Assessment
Executive Summary
The eShop solution presents a Linux readiness profile spanning two projects — eShopWinForms and eShopWCFService — with a combined 37 findings: 32 High severity and 5 Low severity. The primary compatibility blockers are concentrated in eShopWinForms, where the Windows Forms UI foundation drives incompatibility across all assessed files. eShopWCFService presents a narrower set of blockers, limited to an incompatible AspNetCore package and IIS-coupled authentication. Both projects share common cross-cutting concerns around file permission handling and external HTTP connectivity that require validation under Linux runtime conditions.

Items Requiring Attention
Cross-platform compatibility items identified across both projects:

eShopWinForms:

System.Windows.Forms-[4.0.0, 4.0.0] — drives the majority of High severity findings across all UI files
System.Drawing.Common-6.0.0 — Image.FromFile() and GetThumbnailImage() are cross-platform incompatible
System.Resources.Extensions-10.0.0 — cross-platform incompatible
System.ServiceModel.NetTcp-10.0.0 — cross-platform incompatible
System.ServiceModel.Primitives-10.0.0 — cross-platform incompatible
System.Configuration.ConfigurationManager-6.0.0 — SettingsBase.Synchronized() is cross-platform incompatible
CatalogView.cs — hardcoded Windows-style path \..\..\Assets\Images\Catalog\ requires platform-neutral path construction
eShopWCFService:

Microsoft.AspNetCore-10.0.0-NUGET — flagged as cross-platform incompatible
IISConfigFeature — authentication and authorization configuration tied to IIS, incompatible with Linux hosting models
Recommended Actions:

Transition the eShopWinForms UI layer away from System.Windows.Forms to a cross-platform UI framework
Replace System.Drawing.Common image processing APIs with a cross-platform imaging alternative
Replace System.ServiceModel.NetTcp and System.ServiceModel.Primitives with cross-platform communication alternatives
Address the hardcoded file path in CatalogView.cs using Path.Combine() or equivalent platform-neutral construction
Evaluate System.Resources.Extensions for a cross-platform replacement
Resolve the Microsoft.AspNetCore-10.0.0-NUGET incompatibility in eShopWCFService
Migrate IIS-coupled authentication and authorization configuration to a Linux-compatible hosting model
Additional Verification Items
Items to validate for optimal Linux deployment across both projects:

External service connectivity — HttpLibraryFeature detected in both projects; Linux network stack behavior and endpoint resolution should be confirmed in each runtime context
File permission handling — FileAccessControlFeature detected in both projects; Windows ACL-based permission APIs do not translate directly to Linux file permission models
Recommended Validation:

Verify all HTTP-based external service connections function correctly under Linux networking conditions for both projects
Validate file permission operations against Linux permission semantics to ensure expected access control behavior across both projects
Dependencies Review
| Package | Project | Compatibility Status | |---|---|---| | System.Windows.Forms-[4.0.0, 4.0.0] | eShopWinForms | Cross-platform incompatible | | System.Drawing.Common-6.0.0 | eShopWinForms | Cross-platform incompatible (specific APIs) | | System.Resources.Extensions-10.0.0 | eShopWinForms | Cross-platform incompatible | | System.ServiceModel.NetTcp-10.0.0 | eShopWinForms | Cross-platform incompatible | | System.ServiceModel.Primitives-10.0.0 | eShopWinForms | Cross-platform incompatible | | System.Configuration.ConfigurationManager-6.0.0 | eShopWinForms | Cross-platform incompatible (specific APIs) | | Microsoft.AspNetCore-10.0.0-NUGET | eShopWCFService | Cross-platform incompatible |

Recommended Updates:

System.Windows.Forms is the highest-priority dependency in eShopWinForms — its pervasive use across all UI files blocks Linux readiness until replaced
Microsoft.AspNetCore-10.0.0-NUGET in eShopWCFService should be replaced or updated to a verified cross-platform-compatible version
System.ServiceModel.NetTcp and System.ServiceModel.Primitives should be evaluated for replacement with cross-platform communication libraries supported under .NET on Linux
System.Drawing.Common usage should be scoped to confirm whether only the identified incompatible APIs are in use, or whether broader replacement is warranted
System.Configuration.ConfigurationManager usage should be reviewed to determine whether the incompatible SettingsBase.Synchronized() call can be replaced with a supported configuration pattern
Transformation Overview
COMPLETELY TRANSFORMED
Detailed transformation results overview

Projects
Total
2
Completely Transformed
2
Partially Transformed
0
Skipped
0
NuGet Packages
Added
11
Removed
3
Updated
3
Files
Added
0
Removed
4
Updated
5
Moved
0
Projects (2)
List of projects in the transformation


1


Project Name
	
Status
	
Files Updated
	
Files Removed
	
Files Added
	
NuGet Added
	
NuGet Removed
	
NuGet Updated
	
Build Errors

Project Name
	
Status
	
Files Updated
	
Files Removed
	
Files Added
	
NuGet Added
	
NuGet Removed
	
NuGet Updated
	
Build Errors

eShopWCFService
COMPLETELY TRANSFORMED
3
2
0
5
1
1
0
eShopWinForms
COMPLETELY TRANSFORMED
2
2
0
6
2
2
0
eShopWinForms
COMPLETELY TRANSFORMED
Transformation details for eShopLegacyNTier/src/eShopWinForms/Connected Services/eShopServiceReference/Reference.cs

Project Summary
eShopWinForms Modernization Summary
The eShopWinForms project is modernized from .NET Framework 4.7 to .NET 10.0, converting the project file from the legacy MSBuild XML format to the SDK-style csproj format. Package management moves from packages.config to PackageReference, and AssemblyInfo.cs is removed in favor of SDK-style assembly metadata.

eShopWinForms.csproj

Target framework changed from net47 (TargetFrameworkVersion v4.7) to net10.0
Project file converted from legacy MSBuild XML schema to Microsoft.NET.Sdk SDK-style format
packages.config replaced with PackageReference entries directly in .csproj
Package versions updated:
EntityFramework: 6.1.3 → 6.5.1
Microsoft.AspNet.WebApi.Client: 5.2.3 → 6.0.0
Newtonsoft.Json: 6.0.4 → 13.0.4
New PackageReference entries added: System.ServiceModel.Primitives, System.ServiceModel.Http, System.ServiceModel.NetTcp, System.Resources.Extensions, and System.Windows.Forms
Removed explicit <Reference> entries for framework assemblies (auto-resolved by SDK)
Removed ClickOnce publish settings (PublishUrl, Install, UpdateEnabled, etc.)
Removed explicit <Compile> and <Content> items (SDK-style uses glob inclusion by default)
Several Helpers/ source files explicitly excluded via <Compile Remove="..."> entries
Properties/AssemblyInfo.cs — deleted; assembly metadata now handled by SDK defaults

packages.config — deleted; replaced by PackageReference in .csproj

Connected Services/eShopServiceReference/Reference.cs

Whitespace-only changes (trailing spaces removed from blank lines)
CatalogServiceClient constructor overloads that previously passed endpointConfigurationName to the base class now pass null for the binding/configuration name parameter:
base(endpointConfigurationName) → base(null, null)
base(endpointConfigurationName, remoteAddress) → base(null, new System.ServiceModel.EndpointAddress(remoteAddress))
base(endpointConfigurationName, remoteAddress) → base(null, remoteAddress) (for EndpointAddress overload)
This is a breaking change: named endpoint configuration from app.config/web.config is no longer used; endpoints must be configured programmatically
NuGet Package Changes (10)
List of nuget package changes in the transformation


1


Package Name
	
Change Type
	
Old Version
	
New Version

Package Name
	
Change Type
	
Old Version
	
New Version

System.Windows.Forms
added
-
[4.0.0, 4.0.0]
System.ServiceModel.Primitives
added
-
*
System.ServiceModel.Http
added
-
*
System.ServiceModel.NetTcp
added
-
*
Microsoft.AspNet.WebApi.Client
added
-
6.0.0
System.Resources.Extensions
added
-
*
EntityFramework.SqlServer
removed
6.0.0.0
-
System.Net.Http.Formatting
removed
5.2.3.0
-
EntityFramework
updated
6.0.0.0
6.5.1
Newtonsoft.Json
updated
6.0.0.0
13.0.4
File Changes (4)
List of file changes in the transformation


1


File(s)
	
Change Type
	
Summary
	
Code Diff

File(s)
	
Change Type
	
Summary
	
Code Diff

eShopLegacyNTier/src/eShopWinForms/Properties/AssemblyInfo.cs
removed
Deleted because SDK-style projects auto-generate assembly metadata from `.csproj` properties like `<AssemblyTitle>`, `<Version>`, and `<Company>`, making the manual `AssemblyInfo.cs` file redundant and potentially causing duplicate attribute compilation errors.
View file
eShopLegacyNTier/src/eShopWinForms/packages.config
removed
Deleted as part of migrating from .NET Framework 4.6.1 to the SDK-style project format (.NET Core/.NET 5+), where NuGet dependencies are declared directly in the `.csproj` file using `<PackageReference>` elements, eliminating the need for a separate packages.config file.
View file
eShopLegacyNTier/src/eShopWinForms/Connected Services/eShopServiceReference/Reference.cs
updated
The changes address .NET Core/5+ compatibility where WCF `ClientBase` constructors that accept `endpointConfigurationName` strings (relying on `app.config`/`system.serviceModel` configuration) are not supported. The three affected constructors are redirected to pass `null` binding and an explicit `EndpointAddress` instead, bypassing config-file-based endpoint resolution. Trailing whitespace on blank lines is also normalized throughout.
View file
eShopLegacyNTier/src/eShopWinForms/eShopWinForms.csproj
updated
The project was migrated from the legacy .NET Framework 4.7 MSBuild XML format to the modern SDK-style project format targeting net10.0. This eliminated verbose manual file inclusions, ClickOnce deployment metadata, and hard-coded assembly hint paths. NuGet `PackageReference` elements replaced `packages.config` references, and UWP-specific asset files and incompatible helper classes were excluded.
View file
eShopWCFService
COMPLETELY TRANSFORMED
Transformation details for eShopLegacyNTier/src/eShopWCFService/CatalogService.svc.cs

Project Summary
WCF Service Modernization: .NET Framework 4.6.1 → .NET 10
The eShopWCFService project is modernized from a .NET Framework 4.6.1 WCF web application to a .NET 10 class library using CoreWCF. The project file format moves from the legacy MSBuild XML format to SDK-style .csproj, and System.ServiceModel references are replaced with CoreWCF equivalents.

eShopWCFService.csproj
TargetFrameworkVersion v4.6.1 → TargetFramework net10.0
SDK-style project format (Microsoft.NET.Sdk) replaces legacy MSBuild schema
packages.config dependency management replaced with <PackageReference> in .csproj
System.ServiceModel / System.ServiceModel.Web assembly references removed
Added NuGet packages:
CoreWCF.Primitives, CoreWCF.Http, CoreWCF.NetTcp
Microsoft.AspNetCore and Microsoft.AspNetCore.SystemWebAdapters 2.3.0
EntityFramework 6.5.1 (upgraded from 6.1.3)
IIS Express, ProjectTypeGuids, UseIIS, and web application flavor configuration removed
CatalogServiceClient.cs excluded from compilation
OutputType remains Library
CatalogService.svc.cs and ICatalogService.cs
using System.ServiceModel and using System.ServiceModel.Web replaced with using CoreWCF
Properties/AssemblyInfo.cs
File deleted; assembly metadata now handled by SDK-style project defaults
packages.config
File deleted; package management consolidated into <PackageReference> entries in .csproj
Breaking Changes
System.ServiceModel attributes ([ServiceContract], [OperationContract]) must be sourced from CoreWCF namespace — binary and namespace compatibility is not guaranteed for all WCF bindings
.svc file hosting model is no longer applicable; CoreWCF requires ASP.NET Core middleware hosting
CatalogServiceClient.cs is excluded, meaning any client-side proxy code referencing System.ServiceModel.ClientBase<T> is not yet ported
NuGet Package Changes (7)
List of nuget package changes in the transformation


1


Package Name
	
Change Type
	
Old Version
	
New Version

Package Name
	
Change Type
	
Old Version
	
New Version

Microsoft.AspNetCore.SystemWebAdapters
added
-
2.3.0
CoreWCF.Primitives
added
-
*
CoreWCF.Http
added
-
*
CoreWCF.NetTcp
added
-
*
Microsoft.AspNetCore
added
-
*
EntityFramework.SqlServer
removed
6.0.0.0
-
EntityFramework
updated
6.0.0.0
6.5.1
File Changes (5)
List of file changes in the transformation


1


File(s)
	
Change Type
	
Summary
	
Code Diff

File(s)
	
Change Type
	
Summary
	
Code Diff

eShopLegacyNTier/src/eShopWCFService/Properties/AssemblyInfo.cs
removed
Deleted because the SDK-style project format auto-generates assembly metadata, making the manual `Properties/AssemblyInfo.cs` file redundant and incompatible with the new project structure.
View file
eShopLegacyNTier/src/eShopWCFService/packages.config
removed
Deleted because the project migrated from the legacy `packages.config` NuGet package management system to SDK-style `<PackageReference>` elements directly in the `.csproj` file, eliminating the need for this separate dependency manifest.
View file
eShopLegacyNTier/src/eShopWCFService/CatalogService.svc.cs
updated
`System.ServiceModel` and `System.ServiceModel.Web` were replaced with `CoreWCF` to align the service implementation with the CoreWCF migration, removing .NET Framework-exclusive WCF dependencies.
View file
eShopLegacyNTier/src/eShopWCFService/ICatalogService.cs
updated
`System.ServiceModel` was replaced with `CoreWCF` to migrate WCF service contracts from the Windows-only .NET Framework WCF implementation to the cross-platform CoreWCF library.
View file
eShopLegacyNTier/src/eShopWCFService/eShopWCFService.csproj
updated
Migrated from the legacy `ToolsVersion` MSBuild web application project format targeting `net461` to the modern SDK-style format targeting `net10.0`, replacing all manually declared assembly references with `<PackageReference>` entries for CoreWCF packages and removing IIS Express configuration, enabling cross-platform hosting via ASP.NET Core.
View file
eShopModernizedNTier.sln
Download NuGet Package Changes
Transformation details for eShopModernizedNTier.sln

Solution Transformation Summary
Details about the transformation settings and configuration
Combined Modernization Summary
Overview
Both eShopWCFService and eShopWinForms projects are migrated to .NET 10.0. eShopWCFService moves from the legacy WCF hosting model to CoreWCF, while eShopWinForms updates its dependencies and SDK references.

Target Framework
eShopWCFService: TargetFrameworkVersion=v4.6.1 → net10.0
eShopWinForms: net6.0-windows → net10.0 (Windows Forms support retained via UseWindowsForms=true; -windows TFM suffix removed)
Project File Format
eShopWCFService converted from legacy MSBuild XML format to SDK-style .csproj; explicit <Compile> and <Reference> item groups removed; CatalogServiceClient.cs explicitly excluded via <Compile Remove="..." />
IIS Express configuration properties (UseIISExpress, IISExpressSSLPort, ProjectTypeGuids) removed from eShopWCFService
Dependency Management
packages.config removed from eShopWCFService; replaced by <PackageReference> in the .csproj
AssemblyInfo.cs deleted from both projects; SDK auto-generates assembly metadata from .csproj properties. Note: eShopWinForms sets GenerateAssemblyInfo=false, preserving manual control without the file.
Package Changes
eShopWCFService — New references: | Package | Version | |---|---| | CoreWCF.Primitives | * | | CoreWCF.Http | * | | CoreWCF.NetTcp | * | | Microsoft.AspNetCore | * | | Microsoft.AspNetCore.SystemWebAdapters | 2.3.0 | | EntityFramework | 6.5.1 (upgraded from 6.1.3) |

eShopWinForms — Updated references: | Package | Change | |---|---| | EntityFramework | 6.4.4 → 6.5.1 | | Microsoft.AspNet.WebApi.Client | 5.2.7 → 6.0.0 | | System.ServiceModel.Duplex/Security | 4.8.0 → 6.0.0 | | System.ServiceModel.Http/NetTcp/Primitives | Added/updated, floating version (*) | | Microsoft.WindowsDesktop.App.Ref | Added at 10.0.0 |

WCF Migration
eShopWCFService replaces System.ServiceModel with CoreWCF; using System.ServiceModel and using System.ServiceModel.Web are replaced with using CoreWCF in CatalogService.svc.cs and ICatalogService.cs
eShopWinForms adds System.ServiceModel.Primitives as a new dependency
Breaking Changes
System.ServiceModel.Web (REST/WebHttp bindings) is removed from eShopWCFService with no replacement; any code using WebGetAttribute, WebInvokeAttribute, or WebServiceHost will fail to compile
Floating versions ("*") used for CoreWCF, ASP.NET Core, and several System.ServiceModel.* packages across both projects introduce non-deterministic build behavior; explicit versions should be pinned for production builds
Linux Readiness
Details about the Linux readiness assessment and recommendations
eShopOnDotNet — Linux Readiness
Linux Compatibility Assessment
Executive Summary
The eShopOnDotNet solution contains 2 assessed projects with a combined 39 total findings — 35 High severity and 4 Low severity. Linux readiness varies significantly across the solution. eShopWinForms carries the highest incompatibility load, driven by a Windows-native WinForms architecture, WCF transport dependencies, and GDI+ APIs. eShopWCFService presents a lighter finding profile but includes a flagged Microsoft.AspNetCore package incompatibility and IIS-specific configuration dependencies that block Linux deployment.

Items Requiring Attention
The following packages and APIs are cross-platform incompatible across the solution:

Packages:

| Package | Project | Severity | |---|---|---| | Microsoft.WindowsDesktop.App.Ref 10.0.0 | eShopWinForms | High | | System.ServiceModel.NetTcp 10.0.0 | eShopWinForms | High | | System.ServiceModel.Primitives 10.0.0 | eShopWinForms | High | | Microsoft.AspNetCore 10.0.0 | eShopWCFService | High |

APIs and Configuration (by project):

eShopWinForms — WinForms UI controls (ListView, DataGridView, Form, MessageBox, ComboBox, Control), System.Drawing.Image, layout management APIs, Application.EnableVisualStyles(), Application.SetCompatibleTextRenderingDefault(), Form.ShowDialog(), SettingsBase.Synchronized(), WCF communication APIs (ICommunicationObject.BeginOpen(), ICommunicationObject.BeginClose()), and a hardcoded Windows-style path in CatalogView.cs
eShopWCFService — IIS-specific authentication and authorization configuration
Recommended Actions:

Replace Microsoft.WindowsDesktop.App.Ref by migrating the UI layer from WinForms to a cross-platform UI framework
Replace System.ServiceModel.NetTcp and System.ServiceModel.Primitives with a Linux-compatible communication stack; changes are scoped to Reference.cs
Resolve the Microsoft.AspNetCore 10.0.0 cross-platform incompatibility in eShopWCFService before Linux deployment
Migrate IIS-specific authentication and authorization configuration to a Linux-compatible alternative
Refactor the hardcoded Windows-style path in CatalogView.cs using Path.Combine() with Linux case-sensitivity applied — this is addressable independently of the UI framework migration
Additional Verification Items
The following features were detected across both projects and require environment-level validation:

| Feature | Project | Validation Required | |---|---|---| | HttpLibraryFeature | eShopWinForms, eShopWCFService | External HTTP endpoints reachable and correctly configured from Linux host | | FileAccessControlFeature | eShopWinForms, eShopWCFService | File permission operations validated under Linux ACL and permission models |

Recommended Validation:

Verify all external HTTP service endpoints resolve and authenticate correctly from a Linux environment
Validate that file permission operations produce expected behavior under Linux filesystem semantics across both projects
Dependencies Review
| Package | Project | Compatibility Status | |---|---|---| | Microsoft.WindowsDesktop.App.Ref 10.0.0 | eShopWinForms | Cross-platform incompatible | | System.ServiceModel.NetTcp 10.0.0 | eShopWinForms | Cross-platform incompatible | | System.ServiceModel.Primitives 10.0.0 | eShopWinForms | Cross-platform incompatible | | Microsoft.AspNetCore 10.0.0 | eShopWCFService | Cross-platform incompatible |

Recommended Updates:

eShopWinForms — Microsoft.WindowsDesktop.App.Ref is the highest-priority dependency; Linux viability for this project is blocked until the UI layer is replaced with a cross-platform framework. System.ServiceModel.NetTcp and System.ServiceModel.Primitives replacements are scoped to Reference.cs
eShopWCFService — Microsoft.AspNetCore 10.0.0 is the highest-priority dependency; the nature of the incompatibility requires investigation before a replacement path can be confirmed
Transformation Overview
PARTIALLY TRANSFORMED
Detailed transformation results overview

Projects
Total
2
Completely Transformed
1
Partially Transformed
1
Skipped
0
NuGet Packages
Added
7
Removed
1
Updated
7
Files
Added
0
Removed
3
Updated
4
Moved
0
Projects (2)
List of projects in the transformation


1


Project Name
	
Status
	
Files Updated
	
Files Removed
	
Files Added
	
NuGet Added
	
NuGet Removed
	
NuGet Updated
	
Build Errors

Project Name
	
Status
	
Files Updated
	
Files Removed
	
Files Added
	
NuGet Added
	
NuGet Removed
	
NuGet Updated
	
Build Errors

eShopWCFService
COMPLETELY TRANSFORMED
3
2
0
5
1
1
0
eShopWinForms
PARTIALLY TRANSFORMED
1
1
0
2
0
6
1
eShopWCFService
COMPLETELY TRANSFORMED
Transformation details for eShopModernizedNTier/src/eShopWCFService/CatalogService.svc.cs

Project Summary
eShopWCFService Modernization Summary
The eShopWCFService project is modernized from .NET Framework 4.6.1 to .NET 10.0, replacing the legacy System.ServiceModel WCF hosting model with CoreWCF — an open-source, community-maintained port of WCF to .NET Core. The project file is converted from the legacy MSBuild XML format to the SDK-style .csproj format, and packages.config is replaced with <PackageReference> elements.

eShopWCFService.csproj
TargetFrameworkVersion=v4.6.1 → TargetFramework=net10.0
Converted from legacy MSBuild project format (ToolsVersion="15.0") to SDK-style (Sdk="Microsoft.NET.Sdk")
Removed packages.config dependency management; replaced with <PackageReference> in the .csproj
Removed IIS Express configuration properties (UseIISExpress, IISExpressSSLPort, ProjectTypeGuids, etc.)
Removed explicit <Compile> and <Reference> item groups — SDK-style projects include files by default
CatalogServiceClient.cs explicitly excluded via <Compile Remove="..." />
New package references:

| Package | Version | |---|---| | CoreWCF.Primitives | * | | CoreWCF.Http | * | | CoreWCF.NetTcp | * | | Microsoft.AspNetCore | * | | Microsoft.AspNetCore.SystemWebAdapters | 2.3.0 | | EntityFramework | 6.5.1 (upgraded from 6.1.3) |

⚠️ Wildcard versions ("*") for CoreWCF and ASP.NET Core packages will resolve to the latest available at restore time, which may introduce unintended breaking changes in CI/CD pipelines. Pin to explicit versions for production.

CatalogService.svc.cs and ICatalogService.cs
Removed using System.ServiceModel; and using System.ServiceModel.Web;
Added using CoreWCF; — service contracts and behaviors now resolve from CoreWCF instead of System.ServiceModel
Properties/AssemblyInfo.cs
Deleted entirely — SDK-style projects auto-generate assembly metadata from .csproj properties (<AssemblyName>, <Version>, etc.)
packages.config
Deleted — replaced by <PackageReference> in the .csproj
Breaking change: System.ServiceModel.Web (REST/WebHttp bindings) is removed with no replacement reference. Any code relying on WebGetAttribute, WebInvokeAttribute, or WebServiceHost will fail to compile.

NuGet Package Changes (7)
List of nuget package changes in the transformation


1


Package Name
	
Change Type
	
Old Version
	
New Version

Package Name
	
Change Type
	
Old Version
	
New Version

Microsoft.AspNetCore.SystemWebAdapters
added
-
2.3.0
CoreWCF.Primitives
added
-
*
CoreWCF.Http
added
-
*
CoreWCF.NetTcp
added
-
*
Microsoft.AspNetCore
added
-
*
EntityFramework.SqlServer
removed
6.0.0.0
-
EntityFramework
updated
6.0.0.0
6.5.1
File Changes (5)
List of file changes in the transformation


1


File(s)
	
Change Type
	
Summary
	
Code Diff

File(s)
	
Change Type
	
Summary
	
Code Diff

eShopModernizedNTier/src/eShopWCFService/Properties/AssemblyInfo.cs
removed
Deleted because SDK-style projects auto-generate assembly metadata, making the manual Properties/AssemblyInfo.cs file redundant and potentially causing duplicate attribute compilation errors.
View file
eShopModernizedNTier/src/eShopWCFService/packages.config
removed
Deleted because the project migrated from the legacy packages.config NuGet management system to SDK-style PackageReference format, which handles dependencies directly in the .csproj file, eliminating the need for this separate file.
View file
eShopModernizedNTier/src/eShopWCFService/CatalogService.svc.cs
updated
`System.ServiceModel` replaced with `CoreWCF` namespace because the project is migrating from .NET Framework's built-in WCF to CoreWCF, the open-source WCF port that runs on .NET Core/.NET 5+, enabling cross-platform compatibility.
View file
eShopModernizedNTier/src/eShopWCFService/ICatalogService.cs
updated
`System.ServiceModel` replaced with `CoreWCF` namespace because the project is migrating from .NET Framework's built-in WCF to CoreWCF, the open-source WCF port that runs on .NET Core/.NET 5+, enabling cross-platform compatibility.
View file
eShopModernizedNTier/src/eShopWCFService/eShopWCFService.csproj
updated
Converted from the legacy MSBuild web application project format targeting .NET 4.6.1 to the modern SDK-style format targeting net10.0. IIS Express hosting configuration was removed in favor of CoreWCF with ASP.NET Core hosting. EntityFramework was upgraded from 6.1.3 to 6.5.1, `System.ServiceModel` references were replaced with CoreWCF NuGet packages, and `CatalogServiceClient.cs` was explicitly excluded since client-side WCF proxy generation is handled differently in CoreWCF.
View file
eShopWinForms
PARTIALLY TRANSFORMED
Transformation details for eShopModernizedNTier/src/eShopWinForms/Properties/AssemblyInfo.cs

Project Summary
eShopWinForms Modernization Summary
The eShopWinForms project targets .NET 10.0 (upgraded from net6.0-windows) and removes the legacy AssemblyInfo.cs file, relying on SDK-generated assembly metadata. Package references are updated to current versions, and new WCF/WinDesktop references are added.

eShopWinForms/Properties/AssemblyInfo.cs — Deleted

Removed legacy AssemblyInfo.cs; assembly metadata is now handled by the SDK via MSBuild properties
GenerateAssemblyInfo is explicitly set to false in the .csproj, preserving manual control without the file
eShopWinForms/eShopWinForms.csproj — Modified

TargetFramework: net6.0-windows → net10.0 — drops the -windows TFM suffix; Windows Forms support is maintained via UseWindowsForms=true
Breaking change: Floating version (Version="*") used for System.ServiceModel.Http, System.ServiceModel.NetTcp, and System.ServiceModel.Primitives — introduces non-deterministic build behavior
Added Microsoft.WindowsDesktop.App.Ref (10.0.0) — explicit WinDesktop SDK reference
Added System.ServiceModel.Primitives — new WCF dependency not present in prior version
EntityFramework: 6.4.4 → 6.5.1
Microsoft.AspNet.WebApi.Client: 5.2.7 → 6.0.0
System.ServiceModel.Duplex/Security: 4.8.0 → 6.0.0
Whitespace and XML formatting normalized throughout the project file
Build Errors Summary
Build Errors Summary
NuGet Package Compatibility Error
Project: eShopWinForms.csproj

| Property | Detail | |---|---| | Error Code | NU1213 | | Package | Microsoft.WindowsDesktop.App.Ref 10.0.0 | | Package Type | DotnetPlatform |

Root Cause:

The package type DotnetPlatform in Microsoft.WindowsDesktop.App.Ref 10.0.0 is incompatible with the current project configuration. This package is a Windows Desktop SDK reference package and requires the project to target a compatible Windows Desktop SDK workload (e.g., net10.0-windows with UseWindowsForms or UseWPF enabled in the .csproj).

Resolution Checklist:

Verify the target framework moniker (TFM) in eShopWinForms.csproj is set to net10.0-windows
Confirm <UseWindowsForms>true</UseWindowsForms> is declared in the project file
Ensure the .NET 10 Windows Desktop SDK workload is installed via:
dotnet workload install microsoft-windows-desktop
Remove any explicit PackageReference to Microsoft.WindowsDesktop.App.Ref, as it is included implicitly by the Windows Desktop SDK
* Builds are performed on a Linux machine and may not match the errors if you build on a Windows environment.
NuGet Package Changes (8)
List of nuget package changes in the transformation


1


Package Name
	
Change Type
	
Old Version
	
New Version

Package Name
	
Change Type
	
Old Version
	
New Version

Microsoft.WindowsDesktop.App.Ref
added
-
10.0.0
System.ServiceModel.Primitives
added
-
*
EntityFramework
updated
6.4.4
6.5.1
Microsoft.AspNet.WebApi.Client
updated
5.2.7
6.0.0
System.ServiceModel.Duplex
updated
4.8.0
6.0.0
System.ServiceModel.Http
updated
4.8.0
*
System.ServiceModel.NetTcp
updated
4.8.0
*
System.ServiceModel.Security
updated
4.8.0
6.0.0
File Changes (2)
List of file changes in the transformation


1


File(s)
	
Change Type
	
Summary
	
Code Diff

File(s)
	
Change Type
	
Summary
	
Code Diff

eShopModernizedNTier/src/eShopWinForms/Properties/AssemblyInfo.cs
removed
Deleted because `GenerateAssemblyInfo>false</GenerateAssemblyInfo>` was already present in the project file, making the manual file redundant. The SDK-style project system handles assembly metadata automatically, and retaining both caused conflicts. Removing it eliminates duplicate attribute declarations that would cause build errors.
View file
eShopModernizedNTier/src/eShopWinForms/eShopWinForms.csproj
updated
Upgraded from `net6.0-windows` to `net10.0` to target the latest .NET release, requiring corresponding package version bumps (EntityFramework 6.5.1, WebApi.Client 6.0.0, ServiceModel packages to 6.0.0). The wildcard versions (`*`) for Http, NetTcp, and Primitives allow automatic resolution to the latest compatible builds. `Microsoft.WindowsDesktop.App.Ref` was added explicitly to ensure Windows Desktop runtime APIs are available under the non-suffixed `net10.0` TFM, since the `-windows` suffix was dropped.
View file
Build Errors (1)
List of build errors in the transformation. Builds are performed on a Linux machine and may not match the errors if you build on a Windows environment.


1


Error Code
	
Description
	
Location

Error Code
	
Description
	
Location

NU1213
The package Microsoft.WindowsDesktop.App.Ref 10.0.0 has a package type DotnetPlatform that is incompatible with this project.
eShopWinForms.csproj
eShopLegacyWebForms.sln
Download NuGet Package Changes
Transformation details for eShopLegacyWebForms.sln

Solution Transformation Summary
Details about the transformation settings and configuration
eShopLegacyWebForms Modernization Summary
The eShopLegacyWebForms application has been migrated from ASP.NET Web Forms on .NET Framework 4.7.2 to Blazor Server on .NET 10.

Project Format and Target Framework
The .csproj is converted from legacy MSBuild format (ToolsVersion="12.0") to Microsoft.NET.Sdk.Web SDK-style format targeting net10.0. Legacy <Import> targets, <ProjectExtensions>, VisualStudioVersion properties, and all .aspx/.ascx compile entries are removed.

Package Management
packages.config is deleted and replaced with <PackageReference> entries in the .csproj. Key updates include Autofac (4.9.1 → 8.4.0), bootstrap (4.3.1 → 5.3.1), EntityFramework (6.2.0 → 6.5.1), log4net (2.0.10 → 3.2.0), and Newtonsoft.Json (12.0.1 → 13.0.4). Microsoft.Data.SqlClient 6.1.3 is added. Removed packages include Antlr, WebGrease, Modernizr, Respond, all Microsoft.AspNet.* packages, and legacy compiler packages.

Configuration
Web.config, Web.Debug.config, Web.Release.config, and ApplicationInsights.config are removed. Configuration moves to appsettings.json, covering connection strings, app settings, and logging. XML-based Entity Framework config, httpModules, and assembly binding redirects are eliminated. Application Insights must be reconfigured programmatically via AddApplicationInsightsTelemetry().

Removed Files
All Web Forms pages and code-behind files are removed, including Default.aspx, About.aspx, Contact.aspx, and all Catalog/ CRUD pages with their .designer.cs files. Master pages (Site.Master, Site.Mobile.Master) and user controls (ViewSwitcher.ascx) are removed. Application lifecycle files (Global.asax, BundleConfig.cs, RouteConfig.cs) and all DI configuration files (AutofacConfig.cs, UnityConfig.cs, NinjectWebCommon.cs) are removed. AssemblyInfo.cs is replaced by SDK auto-generation.

Added Files
A Blazor component structure is introduced under Components/, including:

App.razor — root document wiring HeadOutlet, Routes, and blazor.web.js
Routes.razor — Blazor <Router> with <RouteView> and <FocusOnNavigate>
_Imports.razor — global @using directives
Layout/MainLayout.razor and Layout/Site.MobileLayout.razor — replacing master pages
Pages/ — About.razor, Contact.razor, Default.razor, and Catalog/ CRUD components (Create.razor, Delete.razor, Details.razor, Edit.razor)
ViewSwitcher.razor — replaces ViewSwitcher.ascx, using IHttpContextAccessor for user-agent detection
Routing
RouteCollection.MapPageRoute is replaced by Blazor's @page directive with typed route parameters. Route values previously read via Page.RouteData.Values are now declared as [Parameter] properties on components.

Page Lifecycle
Web Forms lifecycle events (Page_Load, IsPostBack) are replaced by Blazor component lifecycle methods (OnParametersSet, OnInitialized). Response.Redirect is replaced by NavigationManager.NavigateTo. Auto-generated .designer.cs files are eliminated entirely.

Dependency Injection
Autofac's container builder pattern from Global.asax is replaced by @inject directives in Razor components, with service registration moved to ASP.NET Core's IServiceCollection in Program.cs.

Logging
log4net (ILog, LogManager.GetLogger, XmlConfigurator) is replaced by Microsoft.Extensions.Logging.ILogger<T>, injected via constructor or @inject.

Form Validation
Server-side Web Forms validators are replaced by EditForm with DataAnnotationsValidator and ValidationSummary, using @rendermode InteractiveServer for CRUD pages.

Data Access
System.Data.SqlClient is replaced with Microsoft.Data.SqlClient in CatalogService.cs. HostingEnvironment.ApplicationPhysicalPath is replaced with Directory.GetCurrentDirectory() in CatalogDBInitializer.cs.

Static Assets
Files under Content/, Scripts/, Pics/, fonts/, and images/ are relocated to wwwroot/. CSS bundling via BundleConfig is replaced by direct <link> tags in App.razor referencing Bootstrap 5.3 CDN alongside local content files.

Namespace Removals
System.Web.* (including System.Web.UI, System.Web.Optimization, System.Web.Routing, System.Web.Hosting), log4net, and the Autofac container builder pattern are fully removed from the codebase.

Linux Readiness
Details about the Linux readiness assessment and recommendations
eShopLegacyWebForms — Linux Compatibility Assessment
Linux Readiness Overview
The combined assessment across eShopLegacyWebForms covers findings identified during Linux compatibility analysis. The project presents a low-risk profile for Linux deployment, with all findings classified at low severity and oriented toward runtime validation rather than code changes.

Findings Summary
| Category | Finding | Severity | Action Required | |---|---|---|---| | File System | FileAccessControlFeature | Low | Verify POSIX permission semantics | | Connectivity | HttpLibraryFeature | Low | Validate external endpoint reachability |

Areas Requiring Attention
File System Compatibility
FilePermission — FileAccessControlFeature detected; file permission APIs behave differently under Linux POSIX semantics compared to Windows ACL-based permissions
Validate permission-dependent workflows against the target Linux environment during deployment
External Connectivity
ExternalServiceConnectivity — HttpLibraryFeature detected; HTTP-based external service connections require environment-level validation
Confirm endpoint reachability, certificate trust chains, and proxy configurations on the Linux host
Dependencies
No package-level dependency findings were identified. All findings are runtime and environment verification items.

Overall Linux Readiness
Severity Breakdown:

🔴 Critical: 0
🟠 High: 0
🟡 Medium: 0
🟢 Low: 2
All findings are deployment validation steps executable without codebase modifications, placing eShopLegacyWebForms in a strong position for Linux deployment.

Transformation Overview
PARTIALLY TRANSFORMED
Detailed transformation results overview

Projects
Total
1
Completely Transformed
0
Partially Transformed
1
Skipped
0
NuGet Packages
Added
9
Removed
24
Updated
14
Files
Added
14
Removed
38
Updated
3
Moved
94
Projects (1)
List of projects in the transformation


1


Project Name
	
Status
	
Files Updated
	
Files Removed
	
Files Added
	
NuGet Added
	
NuGet Removed
	
NuGet Updated
	
Build Errors

Project Name
	
Status
	
Files Updated
	
Files Removed
	
Files Added
	
NuGet Added
	
NuGet Removed
	
NuGet Updated
	
Build Errors

eShopLegacyWebForms
PARTIALLY TRANSFORMED
3
38
14
9
24
14
1
eShopLegacyWebForms
PARTIALLY TRANSFORMED
Transformation details for eShopLegacyWebFormsSolution/src/eShopLegacyWebForms/wwwroot/images/main_footer_text.png

Project Summary
eShopLegacyWebForms — Consolidated Modernization Summary
The eShopLegacyWebForms application has been modernized from ASP.NET Web Forms on .NET Framework 4.7.2 to Blazor Server on .NET 10, replacing the Web Forms page lifecycle, XML-based configuration, legacy DI containers, and MSBuild project format with their ASP.NET Core equivalents.

Project Format and Target Framework
The .csproj is converted from legacy verbose MSBuild format (ToolsVersion="12.0") to Microsoft.NET.Sdk.Web SDK-style format:

<!-- Before -->
<Project ToolsVersion="12.0" DefaultTargets="Build"
         xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
  <PropertyGroup>
    <TargetFrameworkVersion>v4.7.2</TargetFrameworkVersion>
  </PropertyGroup>
</Project>

<!-- After -->
<Project Sdk="Microsoft.NET.Sdk.Web">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <GenerateAssemblyInfo>false</GenerateAssemblyInfo>
  </PropertyGroup>
</Project>
Legacy <Import> targets, <ProjectExtensions>, VisualStudioVersion properties, EnsureNuGetPackageBuildImports, and all .aspx/.ascx <Compile SubType="ASPXCodeBehind"> entries are removed.

Package Management
packages.config (47 packages targeting net472) is deleted. Packages are now declared as <PackageReference> entries in the .csproj. Key version changes:

| Package | Before | After | |---|---|---| | Autofac | 4.9.1 | 8.4.0 | | bootstrap | 4.3.1 | 5.3.1 | | EntityFramework | 6.2.0 | 6.5.1 | | jQuery | 3.5.0 | 3.7.0 | | log4net | 2.0.10 | 3.2.0 | | Microsoft.ApplicationInsights suite | 2.9.1 | 2.21.0–2.23.0 | | Newtonsoft.Json | 12.0.1 | 13.0.4 | | Microsoft.Data.SqlClient | — | 6.1.3 (added) |

Removed entirely: Antlr, WebGrease, Modernizr, Respond, Microsoft.AspNet.* packages, Microsoft.CodeDom.Providers.DotNetCompilerPlatform, Microsoft.Net.Compilers.

Licensing note: Autofac 8.x remains MIT. bootstrap 5.x remains MIT. Microsoft.Data.SqlClient and the Microsoft.ApplicationInsights suite are MIT-licensed but carry separate attribution requirements. Update your internal license inventory accordingly.

Configuration
Web.config, Web.Debug.config, Web.Release.config, and ApplicationInsights.config are removed. Configuration moves to appsettings.json:

{
  "ConnectionStrings": {
    "CatalogDBContext": "Server=.;Database=eShopLegacy;Trusted_Connection=True;"
  },
  "AppSettings": {
    "UseMockData": "true",
    "UseCustomizationData": "false"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  }
}
XML-based entity framework config, httpModules, and assembly binding redirects are eliminated. Application Insights must be reconfigured programmatically via AddApplicationInsightsTelemetry() if required.

Removed Files
Web Forms Pages and Code-Behind
Default.aspx / .cs / .designer.cs — catalog list with asp:ListView, asp:HyperLink
About.aspx / .cs / .designer.cs
Contact.aspx / .cs / .designer.cs
Catalog/Create.aspx, Delete.aspx, Details.aspx, Edit.aspx — all with .cs and .designer.cs
Master Pages and Controls
Site.Master / .cs / .designer.cs — MasterPage with ScriptManager, ContentPlaceHolder
Site.Mobile.Master / .cs / .designer.cs
ViewSwitcher.ascx / .cs / .designer.cs — used WebFormsFriendlyUrlResolver, RouteTable
Bootstrapping and App Lifecycle
Global.asax / .cs — Autofac IContainerProviderAccessor, RouteConfig, BundleConfig, HttpApplication lifecycle
Properties/AssemblyInfo.cs — replaced by SDK auto-generation
App_Start/BundleConfig.cs — System.Web.Optimization bundle registration
App_Start/RouteConfig.cs — System.Web.Routing page route registration
AuthConfig.cs, FilterConfig.cs, WebApiConfig.cs, UnityConfig.cs, NinjectWebCommon.cs, AutofacConfig.cs — all excluded from compilation
Added Files
Blazor Component Structure
Components/
├── App.razor                        # Root document; HeadOutlet, Routes, blazor.web.js
├── Routes.razor                     # Blazor <Router> with <RouteView>, <FocusOnNavigate>
├── _Imports.razor                   # Global @using directives
├── Layout/
│   ├── MainLayout.razor             # LayoutComponentBase with Blazor error UI
│   └── Site.MobileLayout.razor      # Mobile layout using NavigationManager
├── Pages/
│   ├── About.razor
│   ├── Contact.razor
│   ├── Default.razor                # @page "/", @page "/Default"
│   └── Catalog/
│       ├── Create.razor             # EditForm, DataAnnotationsValidator, @rendermode InteractiveServer
│       ├── Delete.razor             # [Parameter] int Id
│       ├── Details.razor            # OnParametersSet lifecycle
│       └── Edit.razor               # EditForm, InputText, InputNumber
└── ViewSwitcher.razor               # Replaces ViewSwitcher.ascx; IHttpContextAccessor for UA detection
App.razor wires the root document:

<!DOCTYPE html>
<html lang="en">
<head>
    <HeadOutlet />
</head>
<body>
    <Routes />
    <script src="_framework/blazor.web.js"></script>
</body>
</html>
Routing
Web Forms RouteCollection.MapPageRoute is replaced by Blazor's declarative @page directive with typed route parameters:

// Before — RouteConfig.cs
routes.MapPageRoute("CatalogEdit", "Catalog/Edit/{id}", "~/Catalog/Edit.aspx");

// Before — Edit.aspx.cs
int id = Convert.ToInt32(Page.RouteData.Values["id"]);
@* After — Edit.razor *@
@page "/Catalog/Edit/{id:int}"

@code {
    [Parameter] public int Id { get; set; }
}
Page Lifecycle
Web Forms lifecycle events are replaced by Blazor component lifecycle methods:

// Before — Web Forms code-behind
protected void Page_Load(object sender, EventArgs e)
{
    if (!IsPostBack)
    {
        var item = CatalogService.FindCatalogItem(id);
        DataBind();
    }
}
@* After — Blazor component *@
@code {
    private CatalogItem? item;

    protected override void OnParametersSet()
    {
        item = CatalogService.FindCatalogItem(Id);
    }
}
Response.Redirect is replaced by NavigationManager.NavigateTo. Auto-generated .designer.cs files are eliminated; Blazor's component model handles control references natively.

Dependency Injection
Autofac's property injection pattern from Global.asax is replaced by @inject directives in Razor components. DI registration moves to ASP.NET Core's IServiceCollection in Program.cs:

// Before — Global.asax.cs (Autofac)
var builder = new ContainerBuilder();
builder.RegisterType<CatalogService>().As<ICatalogService>();
@* After — Blazor component *@
@inject ICatalogService CatalogService
@inject ILogger<Edit> Logger
@inject NavigationManager Navigation
Logging
log4net (ILog, LogManager.GetLogger, XmlConfigurator) is replaced by Microsoft.Extensions.Logging.ILogger<T> injected via DI:

// Before
private static readonly ILog _log = LogManager.GetLogger(typeof(CatalogService));
_log.Info("Items loaded");

// After
private readonly ILogger<CatalogService> _logger;
public CatalogService(ILogger<CatalogService> logger) => _logger = logger;
_logger.LogInformation("Items loaded");
Form Validation
Server-side Web Forms validators are replaced by EditForm with DataAnnotationsValidator:

@* After — Create.razor *@
<EditForm Model="newItem" OnValidSubmit="HandleSubmit"
          @rendermode="InteractiveServer">
    <DataAnnotationsValidator />
    <ValidationSummary />
    <InputText @bind-Value="newItem.Name" />
    <InputNumber @bind-Value="newItem.Price" />
    <button type="submit">Create</button>
</EditForm>
Data Access
System.Data.SqlClient is replaced with Microsoft.Data.SqlClient in CatalogService.cs. HostingEnvironment.ApplicationPhysicalPath (System.Web.Hosting) in CatalogDBInitializer.cs is replaced with Directory.GetCurrentDirectory().

Static Assets
All files under Content/, Scripts/, Pics/, fonts/, and images/ are relocated under wwwroot/, aligning with ASP.NET Core static file serving conventions. CSS bundling via BundleConfig is replaced by direct <link> tags in App.razor referencing Bootstrap 5.3 CDN alongside local content files. No file contents are changed during the move.

Namespace Removals
The following namespaces are fully removed from the codebase:

System.Web.* — System.Web.UI.Page, System.Web.UI.WebControls, System.Web.Optimization, System.Web.Routing, System.Web.Hosting
log4net
Autofac (container builder pattern; package retained for potential service registration use)
Build Errors Summary
Build Errors Summary
Missing Application Entry Point

The project eShopLegacyWebForms.csproj lacks a valid static Main method, which is required as the application entry point in .NET Core.

Root Cause: ASP.NET Web Forms applications relied on the IIS pipeline for startup, using Global.asax and HttpApplication instead of an explicit Main entry point. .NET Core requires an explicit entry point, typically defined in a Program.cs file.

Resolution: Add a Program.cs file with a Main method using the .NET Core hosting model:

public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
}
| Property | Detail | |---|---| | Error Code | CS5001 | | Project | eShopLegacyWebForms.csproj | | Affected File | Program.cs (missing) |

* Builds are performed on a Linux machine and may not match the errors if you build on a Windows environment.
NuGet Package Changes (47)
List of nuget package changes in the transformation


1
2
3


Package Name
	
Change Type
	
Old Version
	
New Version

Package Name
	
Change Type
	
Old Version
	
New Version

Microsoft.Data.SqlClient
added
-
6.1.3
bootstrap
added
-
5.3.1
jQuery
added
-
3.7.0
Microsoft.ApplicationInsights.DependencyCollector
added
-
2.23.0
Microsoft.ApplicationInsights.PerfCounterCollector
added
-
2.23.0
Microsoft.ApplicationInsights.Web
added
-
2.21.0
Microsoft.ApplicationInsights.WindowsServer
added
-
2.23.0
Microsoft.ApplicationInsights.WindowsServer.TelemetryChannel
added
-
2.23.0
popper.js
added
-
1.16.1
Antlr3.Runtime
removed
3.5.0.2
-
AspNet.ScriptManager.bootstrap
removed
4.3.1.0
-
AspNet.ScriptManager.jQuery
removed
3.3.1.0
-
Autofac.Integration.Web
removed
4.0.0.0
-
EntityFramework.SqlServer
removed
6.0.0.0
-
Microsoft.AI.Agent.Intercept
removed
2.4.0.0
-
Microsoft.AI.DependencyCollector
removed
2.9.1.0
-
Microsoft.AI.PerfCounterCollector
removed
2.9.1.0
-
Microsoft.AI.ServerTelemetryChannel
removed
2.9.1.0
-
Microsoft.AI.Web
removed
2.9.1.0
-
Microsoft.AI.WindowsServer
removed
2.9.1.0
-
File Changes (64)
List of file changes in the transformation


1
2
3
4


File(s)
	
Change Type
	
Summary
	
Code Diff

File(s)
	
Change Type
	
Summary
	
Code Diff

eShopLegacyWebFormsSolution/src/eShopLegacyWebForms/Components/App.razor
added
Introduced as the Blazor application shell, replacing the WebForms Global.asax bootstrapping model. It defines the HTML document structure, links CSS assets migrated from the legacy `Content/` folder, and mounts the `<Routes />` component and `blazor.web.js` runtime.
View file
eShopLegacyWebFormsSolution/src/eShopLegacyWebForms/Components/Layout/MainLayout.razor
added
Created as the Blazor root layout component, replacing the WebForms Site.Master master page. It provides the base UI shell and includes the Blazor-native error UI pattern for unhandled exception feedback.
View file
eShopLegacyWebFormsSolution/src/eShopLegacyWebForms/Components/Layout/Site.MobileLayout.razor
added
Created to replace the WebForms mobile master page, implementing Blazor's LayoutComponentBase pattern with @Body rendering and a ViewSwitcher component reference, preserving mobile layout functionality in the new framework.
View file
eShopLegacyWebFormsSolution/src/eShopLegacyWebForms/Components/Pages/About.razor
added
Created as the direct Blazor replacement for About.aspx and About.aspx.cs. The page lifecycle method `OnInitialized` replaces `Page_Load`, and `ILogger<T>` via dependency injection replaces log4net's static `LogManager`, aligning with .NET's built-in logging abstractions.
View file
eShopLegacyWebFormsSolution/src/eShopLegacyWebForms/Components/Pages/Catalog/Create.razor
added
This file migrates the legacy ASP.NET WebForms `Create.aspx` page to a Blazor Server interactive component, replacing WebForms code-behind and server controls with Blazor's `EditForm`, `InputText`, and event binding. Manual string-based validation for numeric fields (price, stock) compensates for WebForms' built-in validators that lack direct Blazor equivalents, while `NavigationManager` replaces `Response.Redirect`.
View file
eShopLegacyWebFormsSolution/src/eShopLegacyWebForms/Components/Pages/Catalog/Delete.razor
added
A new Blazor Server component replacing the legacy WebForms Delete.aspx page. The migration adopts `@page` routing with typed route parameters, constructor injection via `@inject` replacing Autofac/WebForms dependency resolution, and `OnInitialized`/event handler patterns replacing WebForms lifecycle methods, enabling the page to run on .NET modern runtime.
View file
eShopLegacyWebFormsSolution/src/eShopLegacyWebForms/Components/Pages/Catalog/Details.razor
added
Created as the Blazor replacement for Details.aspx and its code-behind, adopting component-based routing via `@page`, dependency injection via `@inject`, and `OnParametersSet` lifecycle instead of Page_Load, while replacing ASP.NET server controls with standard HTML and Razor syntax.
View file
eShopLegacyWebFormsSolution/src/eShopLegacyWebForms/Components/Pages/Catalog/Edit.razor
added
This file migrates the legacy ASP.NET WebForms `Edit.aspx` page to a Blazor Server component, replacing server-side postback patterns with `EditForm`, `@bind-Value`, and `OnValidSubmit`. The `@rendermode InteractiveServer` directive enables real-time interactivity, while `NavigationManager` replaces `Response.Redirect`, and manual validation guards compensate for dropdown binding limitations outside `DataAnnotationsValidator`.
View file
eShopLegacyWebFormsSolution/src/eShopLegacyWebForms/Components/Pages/Contact.razor
added
Created to replace Contact.aspx, converting the static contact page into a Blazor component with dependency-injected ILogger, @page routing, and OnInitialized lifecycle method replacing the WebForms Page_Load pattern.
View file
eShopLegacyWebFormsSolution/src/eShopLegacyWebForms/Components/Pages/Default.razor
added
Created to replace the WebForms Default.aspx page with a Blazor Server component. The `@rendermode InteractiveServer` directive enables stateful server-side rendering, `@inject` replaces Autofac property injection, `[SupplyParameterFromQuery]` replaces QueryString parsing, and `NavLink` replaces WebForms HyperLink controls, while `OnParametersSet` handles URL-driven pagination state changes.
View file
eShopLegacyWebFormsSolution/src/eShopLegacyWebForms/Components/Routes.razor
added
Introduced as the Blazor routing entry point to replace WebForms' imperative RouteConfig.cs. It uses the declarative `<Router>` component to handle navigation, with commented multi-layout logic preserved to guide future extensibility without requiring immediate implementation.
View file
eShopLegacyWebFormsSolution/src/eShopLegacyWebForms/Components/ViewSwitcher.razor
added
Created as the Blazor replacement for `ViewSwitcher.ascx.cs`, reimplementing mobile detection using raw User-Agent header parsing instead of `WebFormsFriendlyUrlResolver`, and using `IHttpContextAccessor` and `NavigationManager` as ASP.NET Core-compatible substitutes.
View file
eShopLegacyWebFormsSolution/src/eShopLegacyWebForms/Components/_Imports.razor
added
Created to centralize global `@using` directives for all Blazor components in the project, replacing the need for per-file namespace imports. It explicitly includes both legacy namespaces (Entity Framework, Newtonsoft.Json) and modern ASP.NET Core namespaces, bridging the migration gap.
View file
eShopLegacyWebFormsSolution/src/eShopLegacyWebForms/appsettings.json
added
Introduced to replace Web.config-based configuration, adopting ASP.NET Core's JSON configuration system with connection strings, mock data flags, globalization settings, and structured logging — all incompatible with the legacy System.Web configuration model.
View file
Files moved to images directory related changes
moved
These image files were consolidated into the `wwwroot/images/` directory to align with ASP.NET's static file serving conventions, where `wwwroot` is the designated web root for publicly accessible content. Grouping all UI image assets — branding, banners, and footer graphics — under a single structured path establishes consistent static asset organization and ensures proper HTTP serving behavior in the modernized application.
View 5 files
Files moved to Content directory related changes
moved
These CSS files were relocated from their previous location into the `wwwroot/Content` directory to align with ASP.NET Core's static file serving conventions, where `wwwroot` is the designated web root. Consolidating all stylesheets — Bootstrap variants, custom, and site-specific CSS — under `Content` establishes a consistent, predictable structure for static asset organization within the modernized project layout.
View 9 files
Files moved to Pics directory related changes
moved
These 13 product images, including a placeholder dummy.png, were consolidated into a dedicated `wwwroot/Pics/` directory, aligning static assets with ASP.NET Core's conventions for serving static files. Grouping all catalog images under one path simplifies URL routing, enables consistent static file middleware configuration, and establishes a clean separation between application code and product image assets.
View 13 files
Files moved to WebForms directory related changes
moved
These ten ASP.NET WebForms framework JavaScript files were relocated into a dedicated `wwwroot/Scripts/WebForms/` subdirectory, consolidating all WebForms-specific client-side scripts under a single, clearly namespaced folder. This establishes clean separation between WebForms infrastructure scripts and application-level JavaScript, improving static asset organization within the `wwwroot` structure required by ASP.NET Core's static file serving conventions.
View 10 files
Files moved to MSAjax directory related changes
moved
These 11 Microsoft Ajax JavaScript library files were consolidated into a dedicated `MSAjax` subdirectory under `Scripts/WebForms/`, separating them from other WebForms scripts. This organizational move groups the entire MSAjax client-side library by vendor/framework origin, improving maintainability and making the legacy ASP.NET WebForms Ajax dependencies explicitly identifiable as a discrete, bounded set of files within the static asset structure.
View 11 files
Files moved to Scripts directory related changes
moved
All client-side JavaScript libraries — jQuery, Bootstrap, Popper, Modernizr, and Respond — were relocated into a dedicated `wwwroot/Scripts` directory. This consolidates all third-party scripts under a single, conventionally named folder within the web root, establishing a clear separation between script assets and other static content types like stylesheets and images, improving asset organization and maintainability.
View 18 files
Build Errors (1)
List of build errors in the transformation. Builds are performed on a Linux machine and may not match the errors if you build on a Windows environment.


1


Error Code
	
Description
	
Location

Error Code
	
Description
	
Location

CS5001
Program does not contain a static 'Main' method suitable for an entry point
eShopLegacyWebForms.csproj
eShopModernizedWebForms.sln
Download NuGet Package Changes
Transformation details for eShopModernizedWebForms.sln

Solution Transformation Summary
Details about the transformation settings and configuration
eShopModernizedWebForms: ASP.NET WebForms to .NET Core Modernization Summary
Overview
The eShopModernizedWebForms application has been converted from ASP.NET WebForms targeting .NET Framework 4.7.2 to a Blazor Server application targeting net10.0. The legacy MSBuild project format with packages.config is replaced by an SDK-style .csproj with <PackageReference> items. All WebForms constructs — .aspx, .master, .asax, code-behind files, Web.config, and App_Start — are removed and replaced with ASP.NET Core equivalents.

Project System and Configuration
Target framework: TargetFrameworkVersion v4.7.2 → net10.0
Project format: Legacy MSBuild → SDK-style
Dependency model: packages.config → <PackageReference> in .csproj
Configuration: Web.config → appsettings.json with ASP.NET Core configuration system
Removed: NuGet restore validation imports, Roslyn compiler copy targets, <VisualStudio> flavor properties, TypeScriptCompile item groups
UI Framework
| Removed (WebForms) | Added (Blazor) | |---|---| | Default.aspx, Site.Master, Site.Mobile.Master | Default.razor, MainLayout.razor, Site.MobileLayout.razor | | Create.aspx, Edit.aspx, Delete.aspx, Details.aspx | Create.razor, Edit.razor, Delete.razor, Details.razor | | ViewSwitcher.ascx / .ascx.cs / .designer.cs | ViewSwitcher.razor (mobile/desktop switching removed) | | .aspx.cs and .aspx.designer.cs code-behind files | Logic merged into .razor components | | Global.asax / HttpApplication event handlers | ASP.NET Core Startup.cs / Program.cs middleware pipeline |

Catalog CRUD pages use EditForm, DataAnnotationsValidator, InputText, and InputNumber components
@page directives replace RouteConfig.cs
NavigationManager replaces Response.Redirect()
@rendermode InteractiveServer applied to stateful pages
@attribute [Authorize] replaces Web Forms role-based page access
LayoutComponentBase replaces .master page inheritance
Application Lifecycle and Middleware
Application_Start, Session_Start, and Application_BeginRequest event handlers removed
IAppBuilder (OWIN) → IApplicationBuilder (ASP.NET Core)
app.Use<AuthenticationMiddleware>() → app.UseMiddleware<AuthenticationMiddleware>()
Authentication
Startup.Auth.cs (Microsoft.Owin.Security.OpenIdConnect, CookieAuthentication) removed
OwinMiddleware → ASP.NET Core RequestDelegate middleware
IOwinContext.Authentication → context.SignInAsync(ClaimsPrincipal) via Microsoft.AspNetCore.Authentication
@attribute [Authorize] on Razor components replaces page-level access control
Full Microsoft.Owin.* stack removed
Dependency Injection
Autofac IContainerProviderAccessor pattern → ASP.NET Core built-in DI via constructor injection and @inject directives
PicUploader: Autofac container removed; replaced with constructor injection of IHttpContextAccessor and IImageService
AppSettingsSqlConnectionFactory registration replaced with string-based connection string registration
Autofac: 4.9.4 → 8.4.0
HTTP Abstractions
| Removed | Replacement | |---|---| | System.Web.HttpContext / HttpContext.Current | IHttpContextAccessor | | HttpPostedFile | IFormFile | | Response.End() | HTTP status code assignment | | System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath | Directory.GetCurrentDirectory() | | HttpContext.Current.Server.MapPath | IWebHostEnvironment.WebRootPath |

Infrastructure Changes
Serialization

JavaScriptSerializer → System.Text.Json.JsonSerializer
Logging

log4net (ILog, LogManager, XmlConfigurator, log4net.xml) → Microsoft.Extensions.Logging.ILogger<T> via DI
log4net: 2.0.10 → 3.2.0
Telemetry

ApplicationInsights.config (XML) and MyTelemetryInitializer.cs removed
Configuration moves to appsettings.json and Program.cs
ApplicationInsights: 2.11.x → 2.23.x
Bundling

BundleConfig.cs (System.Web.Optimization) removed
Static assets referenced via <link> tags and CDN references in App.razor
Bootstrap 4.x → 5.3.1, jQuery 3.4.1 → 3.7.0
Azure Storage SDK

Microsoft.WindowsAzure.Storage (CloudStorageAccount, CloudBlobClient, CloudBlockBlob) → Azure.Storage.Blobs (BlobServiceClient, BlobContainerClient, BlobClient) 12.25.1
Key Vault

OptionalKeyVaultConfigurationBuilder.cs removed; replaced with AddAzureKeyVault() in Program.cs
Static Assets

All files under Content/, Scripts/, Pics/, fonts/, images/, and favicon.ico relocated to wwwroot/
Entity Framework

CatalogDBContext constructor changed to parameterless; no longer accepts ISqlConnectionFactory
EntityFramework: 6.3.0 → 6.5.1
Selected Package Version Changes
| Package | Old | New | |---|---|---| | Microsoft.Extensions.* | 3.0.0 | 9.0.x | | Microsoft.IdentityModel.* | 5.6.0 | 8.14.0–8.16.0 | | System.IdentityModel.Tokens.Jwt | 5.6.0 | 8.14.0 | | StackExchange.Redis | 2.0.601 | 2.11.0 | | System.Text.Json | 4.6.0 | 9.0.9 | | Newtonsoft.Json | 13.0.2 | 13.0.4 | | System.Threading.Channels | 4.6.0 | 10.0.3 | | Microsoft.Rest.ClientRuntime | 2.3.24 | 3.0.3 |

Packages Removed With No Replacement
Antlr, WebGrease, WindowsAzure.Storage, WindowsAzure.ServiceBus
Full Microsoft.Owin.* stack
Microsoft.AspNet.* WebForms-specific packages (FriendlyUrls, ScriptManager, Web.Optimization)
Microsoft.CodeDom.Providers.DotNetCompilerPlatform, Microsoft.Net.Compilers
Microsoft.Configuration.ConfigurationBuilders.* (beta)
NETStandard.Library
Modernizr, Respond (front-end polyfills)
AssemblyInfo.cs (metadata now managed via SDK-style .csproj properties)
Breaking Changes
| Area | Change | |---|---| | PicUploader | No longer a WebService (System.Web.Services.WebService); ASMX endpoint behavior removed — must be re-exposed via an ASP.NET Core controller or minimal API endpoint | | CatalogDBContext | Constructor changed to parameterless; callers using connection injection are affected | | Microsoft.IdentityModel.Clients.ActiveDirectory | Retained at 5.3.0 — this library is deprecated; Microsoft.Identity.Client (MSAL) is its replacement | | Mobile view switching | ViewSwitcher.ascx and Microsoft.AspNet.FriendlyUrls removed with no replacement | | Static asset paths | Files relocated to wwwroot/; all virtual path references in removed .aspx/.master files required updates |

Linux Readiness
Details about the Linux readiness assessment and recommendations
eShopModernized — Linux Readiness
Linux Compatibility Assessment
Executive Summary
The assessment of eShopModernized covers the eShopModernizedWebForms project. The overall Linux readiness posture is strong, with only 2 low-severity findings identified across the project. All findings are verification-oriented rather than blocking compatibility issues, indicating a solid cross-platform foundation. No critical or high-severity findings were identified.

Items Requiring Attention
Cross-platform compatibility items to address:

FileAccessControlFeature (eShopModernizedWebForms) — File permission APIs detected that exhibit platform-specific behavior differences between Windows and Linux
Recommended Actions:

Verify that file permission logic aligns with Linux permission models (user/group/other)
Prioritize file permission validation early in deployment testing to catch environment-specific behavior
Additional Verification Items
Items to validate for optimal Linux deployment:

HttpLibraryFeature (eShopModernizedWebForms) — External service connectivity detected, requiring validation in a Linux runtime environment
FilePermission (eShopModernizedWebForms) — File access control behavior requires confirmation under Linux execution context
Recommended Validation:

Validate external service endpoints are reachable from the Linux host environment
Confirm file permission operations behave as expected under the Linux security model
Dependencies Review
Dependencies with enhancement opportunities:

HttpLibraryFeature (eShopModernizedWebForms) — External connectivity dependency flagged at low severity; no blocking compatibility issue identified
Recommended Updates:

Confirm the HTTP library in use targets a cross-platform compatible implementation
No dependency updates are indicated as required based on current assessment data
Transformation Overview
PARTIALLY TRANSFORMED
Detailed transformation results overview

Projects
Total
1
Completely Transformed
0
Partially Transformed
1
Skipped
0
NuGet Packages
Added
11
Removed
67
Updated
49
Files
Added
12
Removed
37
Updated
10
Moved
96
Projects (1)
List of projects in the transformation


1


Project Name
	
Status
	
Files Updated
	
Files Removed
	
Files Added
	
NuGet Added
	
NuGet Removed
	
NuGet Updated
	
Build Errors

Project Name
	
Status
	
Files Updated
	
Files Removed
	
Files Added
	
NuGet Added
	
NuGet Removed
	
NuGet Updated
	
Build Errors

eShopModernizedWebForms
PARTIALLY TRANSFORMED
10
37
12
11
67
49
1
eShopModernizedWebForms
PARTIALLY TRANSFORMED
Transformation details for eShopModernizedWebFormsSolution/src/eShopModernizedWebForms/wwwroot/images/main_footer_text.png

Project Summary
eShopModernizedWebForms: ASP.NET WebForms to .NET Core Modernization Summary
Overview
The eShopModernizedWebForms application has been fully converted from ASP.NET WebForms targeting .NET Framework 4.7.2 to a Blazor Server application targeting net10.0. The legacy MSBuild project format with packages.config is replaced by an SDK-style .csproj with <PackageReference> items. All WebForms constructs — .aspx, .master, .asax, code-behind files, Web.config, and App_Start — are removed and replaced with their ASP.NET Core equivalents.

Project System and Configuration
Target framework: TargetFrameworkVersion v4.7.2 → net10.0
Project format: Legacy MSBuild (ToolsVersion="12.0") → SDK-style (Sdk="Microsoft.NET.Sdk.Web")
Dependency model: packages.config (150 packages, net472) → <PackageReference> in .csproj
Configuration: Web.config (assembly binding redirects, <system.web>, <system.webServer>, <entityFramework>, <configBuilders>) → appsettings.json with ASP.NET Core configuration system
Removed legacy MSBuild artifacts: NuGet restore validation imports, Roslyn compiler copy targets, <VisualStudio> flavor properties (IIS port bindings), TypeScriptCompile item groups
UI Framework
| Removed (WebForms) | Added (Blazor) | |---|---| | Default.aspx, Site.Master, Site.Mobile.Master | Default.razor, MainLayout.razor, Site.MobileLayout.razor | | Create.aspx, Edit.aspx, Delete.aspx, Details.aspx | Create.razor, Edit.razor, Delete.razor, Details.razor | | ViewSwitcher.ascx / .ascx.cs / .designer.cs | ViewSwitcher.razor (mobile/desktop switching removed) | | .aspx.cs and .aspx.designer.cs code-behind files | Eliminated; logic merged into .razor components | | Global.asax / HttpApplication event handlers | ASP.NET Core Startup.cs / Program.cs middleware pipeline |

Catalog CRUD pages use EditForm, DataAnnotationsValidator, InputText, and InputNumber components
@page directives (e.g., @page "/Catalog/Create") replace RouteConfig.cs
NavigationManager replaces Response.Redirect()
@rendermode InteractiveServer applied to stateful pages
@attribute [Authorize] replaces Web Forms role-based page access
LayoutComponentBase replaces .master page inheritance
Application Lifecycle and Middleware
Application_Start, Session_Start, and Application_BeginRequest event handlers removed
IAppBuilder (OWIN) → IApplicationBuilder (ASP.NET Core)
app.Use<AuthenticationMiddleware>() → app.UseMiddleware<AuthenticationMiddleware>()
Authentication
Startup.Auth.cs (Microsoft.Owin.Security.OpenIdConnect, CookieAuthentication) removed
OwinMiddleware → ASP.NET Core RequestDelegate middleware
IOwinContext.Authentication → context.SignInAsync(ClaimsPrincipal) via Microsoft.AspNetCore.Authentication
@attribute [Authorize] on Razor components replaces page-level access control
Full Microsoft.Owin.* stack removed
Dependency Injection
Autofac IContainerProviderAccessor pattern → ASP.NET Core built-in DI via constructor injection and @inject directives
PicUploader: Autofac container removed; replaced with constructor injection of IHttpContextAccessor and IImageService
AppSettingsSqlConnectionFactory registration replaced with string-based connection string registration
Package version: Autofac 4.9.4 → 8.4.0
HTTP Abstractions
| Removed | Replacement | |---|---| | System.Web.HttpContext / HttpContext.Current | IHttpContextAccessor | | HttpPostedFile | IFormFile | | Response.End() | HTTP status code assignment | | System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath | Directory.GetCurrentDirectory() | | HttpContext.Current.Server.MapPath | IWebHostEnvironment.WebRootPath |

Infrastructure Changes
Serialization

JavaScriptSerializer (System.Web.Script.Serialization) → System.Text.Json.JsonSerializer
Logging

log4net (ILog, LogManager, XmlConfigurator, log4net.xml) → Microsoft.Extensions.Logging.ILogger<T> via DI
Package version: log4net 2.0.10 → 3.2.0
Telemetry

ApplicationInsights.config (XML) and MyTelemetryInitializer.cs removed
Configuration moves to appsettings.json and Program.cs
Package version: ApplicationInsights 2.11.x → 2.23.x
Bundling

BundleConfig.cs (System.Web.Optimization) removed
Static assets referenced via <link> tags and CDN references in App.razor
Bootstrap 4.x → 5.3.1, jQuery 3.4.1 → 3.7.0
Azure Storage SDK

Microsoft.WindowsAzure.Storage (CloudStorageAccount, CloudBlobClient, CloudBlockBlob) → Azure.Storage.Blobs (BlobServiceClient, BlobContainerClient, BlobClient) 12.25.1
Key Vault

OptionalKeyVaultConfigurationBuilder.cs (extending AzureKeyVaultConfigBuilder) removed
Replaced with AddAzureKeyVault() in Program.cs
Static Assets

All files under Content/, Scripts/, Pics/, fonts/, images/, and favicon.ico relocated to wwwroot/
Entity Framework

CatalogDBContext constructor no longer accepts ISqlConnectionFactory; changed to parameterless constructor
Package version: EntityFramework 6.3.0 → 6.5.1
Selected Package Version Changes
| Package | Old | New | |---|---|---| | Microsoft.Extensions.* | 3.0.0 | 9.0.x | | Microsoft.IdentityModel.* | 5.6.0 | 8.14.0–8.16.0 | | System.IdentityModel.Tokens.Jwt | 5.6.0 | 8.14.0 | | StackExchange.Redis | 2.0.601 | 2.11.0 | | System.Text.Json | 4.6.0 | 9.0.9 | | Newtonsoft.Json | 13.0.2 | 13.0.4 | | System.Threading.Channels | 4.6.0 | 10.0.3 | | Microsoft.Rest.ClientRuntime | 2.3.24 | 3.0.3 |

Packages Removed With No Replacement
Antlr, WebGrease, WindowsAzure.Storage, WindowsAzure.ServiceBus
Microsoft.Owin.* stack
Microsoft.AspNet.* WebForms-specific packages (FriendlyUrls, ScriptManager, Web.Optimization)
Microsoft.CodeDom.Providers.DotNetCompilerPlatform, Microsoft.Net.Compilers
Microsoft.Configuration.ConfigurationBuilders.* (beta)
NETStandard.Library
Modernizr, Respond (front-end polyfills)
AssemblyInfo.cs (metadata now managed via SDK-style .csproj properties)
Breaking Changes
| Area | Change | |---|---| | PicUploader | No longer a WebService (System.Web.Services.WebService); ASMX endpoint behavior removed — must be re-exposed via an ASP.NET Core controller or minimal API endpoint | | CatalogDBContext | Constructor changed to parameterless; callers using connection injection are affected | | Microsoft.IdentityModel.Clients.ActiveDirectory | Retained at 5.3.0 — this library is deprecated; Microsoft.Identity.Client (MSAL) is its replacement | | Mobile view switching | ViewSwitcher.ascx and Microsoft.AspNet.FriendlyUrls removed with no replacement | | Static asset paths | ~60+ files relocated to wwwroot/; all virtual path references in removed .aspx/.master files required updates |

Build Errors Summary
Build Error Summary
Missing Entry Point
The project eShopModernizedWebForms.csproj has no valid static Main method defined as an application entry point, which is required for .NET Core console or web applications.

Error: CS5001 — No suitable static Main method found in the project.

Affected Project: eShopModernizedWebFormsSolution/src/eShopModernizedWebForms/eShopModernizedWebForms.csproj

Resolution: Add a Program.cs file with a static Main entry point or a top-level statement:

// Option 1: Explicit Main method
public class Program
{
    public static void Main(string[] args)
    {
        // Application startup logic
    }
}

// Option 2: Top-level statements (.NET 6+)
// No class or method wrapper needed
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
app.Run();
* Builds are performed on a Linux machine and may not match the errors if you build on a Windows environment.
NuGet Package Changes (127)
List of nuget package changes in the transformation


1
2
3
4
5
6
7


Package Name
	
Change Type
	
Old Version
	
New Version

Package Name
	
Change Type
	
Old Version
	
New Version

System.Drawing.Common
added
-
9.0.10
Azure.Storage.Blobs
added
-
12.25.1
bootstrap
added
-
5.3.1
jQuery
added
-
3.7.0
Microsoft.ApplicationInsights.DependencyCollector
added
-
2.23.0
Microsoft.ApplicationInsights.PerfCounterCollector
added
-
2.23.0
Microsoft.ApplicationInsights.Web
added
-
2.21.0
Microsoft.ApplicationInsights.WindowsServer
added
-
2.23.0
Microsoft.ApplicationInsights.WindowsServer.TelemetryChannel
added
-
2.23.0
Microsoft.NETCore.Platforms
added
-
7.0.4
popper.js
added
-
1.16.1
Antlr3.Runtime
removed
3.5.0.2
-
AspNet.ScriptManager.bootstrap
removed
4.3.1.0
-
AspNet.ScriptManager.jQuery
removed
3.4.1.0
-
Autofac.Integration.Web
removed
4.0.0.0
-
EntityFramework.SqlServer
removed
6.0.0.0
-
log4net.Appender.Azure
removed
1.4.0.22240
-
Microsoft.AI.Agent.Intercept
removed
2.4.0.0
-
Microsoft.AI.DependencyCollector
removed
2.11.2.0
-
Microsoft.AI.PerfCounterCollector
removed
2.11.2.0
-
File Changes (68)
List of file changes in the transformation


1
2
3
4


File(s)
	
Change Type
	
Summary
	
Code Diff

File(s)
	
Change Type
	
Summary
	
Code Diff

eShopModernizedWebFormsSolution/src/eShopModernizedWebForms/Components/App.razor
added
Created as the Blazor application root document, replacing WebForms' `Site.Master`. It defines the full HTML shell including `<HeadOutlet>` for dynamic head content, Bootstrap 5 CDN reference alongside migrated legacy CSS files, and the Blazor WebAssembly/Server bootstrap script reference.
View file
eShopModernizedWebFormsSolution/src/eShopModernizedWebForms/Components/Layout/MainLayout.razor
added
Created as the Blazor root layout component, replacing the WebForms master page pattern with Blazor's LayoutComponentBase inheritance model, including built-in error UI handling.
View file
eShopModernizedWebFormsSolution/src/eShopModernizedWebForms/Components/Layout/Site.MobileLayout.razor
added
Retained as a static asset compatible with both legacy WebForms and the new Blazor application, referenced explicitly in Site.MobileLayout.razor.
View file
eShopModernizedWebFormsSolution/src/eShopModernizedWebForms/Components/Pages/Catalog/Create.razor
added
This file was created to migrate the catalog item creation functionality from ASP.NET Web Forms (.aspx) to Blazor Server, as evidenced by the preserved log message referencing "Create.aspx". The `@rendermode InteractiveServer`, `EditForm`, injected services, and manual validation flags replace Web Forms postback/code-behind patterns while retaining identical business logic and UI structure.
View file
eShopModernizedWebFormsSolution/src/eShopModernizedWebForms/Components/Pages/Catalog/Delete.razor
added
Deleted as part of migrating from ASP.NET Web Forms to Blazor. The server-side markup using ASP.NET controls (`asp:Image`, `asp:Label`, `asp:Button`) and code-behind event model is replaced entirely by the new Delete.razor component.
View file
eShopModernizedWebFormsSolution/src/eShopModernizedWebForms/Components/Pages/Catalog/Details.razor
added
Replaces `Details.aspx` with a Blazor component, adopting route parameters (`{id:int}`), DI-injected services, and null-conditional operators instead of WebForms data-binding expressions and code-behind lifecycle events.
View file
eShopModernizedWebFormsSolution/src/eShopModernizedWebForms/Components/Pages/Catalog/Edit.razor
added
This file was created to migrate a WebForms-based catalog edit page to a Blazor Server component, replacing the WebForms code-behind pattern with Blazor's `@code` block and component lifecycle (`OnInitializedAsync`). `EditForm` with `DataAnnotationsValidator` replaces WebForms server controls, `InteractiveServer` render mode enables stateful interactivity, and `NavigationManager` replaces `Response.Redirect` for post-save navigation.
View file
eShopModernizedWebFormsSolution/src/eShopModernizedWebForms/Components/Pages/Default.razor
added
Created as the Blazor replacement for Default.aspx, adopting component-based architecture with `@page` routing, dependency injection via `@inject`, `@rendermode InteractiveServer` for interactivity, and standard C# `@foreach` loops replacing `asp:ListView`. Route parameters (`{index:int}/{size:int}`) replace WebForms postback-driven pagination, and null-conditional operators (`?.`) add defensive data access absent in the original.
View file
eShopModernizedWebFormsSolution/src/eShopModernizedWebForms/Components/Routes.razor
added
Added as the Blazor routing entry point, replacing WebForms' URL routing (`RouteTable`/`RegisterRoutes`). This file defines the `Router` component that maps URLs to Razor components, which is the foundational navigation mechanism in Blazor, with commented-out multi-layout logic preserved as migration guidance for developers.
View file
eShopModernizedWebFormsSolution/src/eShopModernizedWebForms/Components/ViewSwitcher.razor
added
Created as the ASP.NET Core Blazor replacement for ViewSwitcher.ascx.cs. Uses IHttpContextAccessor for request inspection, NavigationManager for URL construction, and ILogger for observability — all ASP.NET Core native patterns replacing WebForms equivalents.
View file
eShopModernizedWebFormsSolution/src/eShopModernizedWebForms/Components/_Imports.razor
added
Created to centralize `@using` directives across all Razor components, equivalent to global `using` statements. It imports both the migrated application namespaces and ASP.NET Core-specific namespaces (`Microsoft.AspNetCore.Components.*`), ensuring all components have access to Blazor, authorization, routing, and other framework services without per-file declarations.
View file
eShopModernizedWebFormsSolution/src/eShopModernizedWebForms/appsettings.json
added
The `ISqlConnectionFactory` dependency injection was removed from the constructor, reverting to EF's default parameterless constructor. This eliminates the custom connection factory pattern because ASP.NET Core's built-in dependency injection and `DbContext` configuration via `appsettings.json` connection strings replaces the manual connection management approach used in the legacy WebForms application.
View file
Files moved to images directory related changes
moved
These image files — brand.png, brand_dark.png, main_banner.png, and main_footer_text.png — were consolidated into the wwwroot/images directory as part of migrating static assets to the ASP.NET Core-compatible wwwroot structure. This aligns with ASP.NET Core's convention of serving static files exclusively from wwwroot, replacing the legacy WebForms approach of storing assets throughout the project directory.
View 5 files
Files moved to Content directory related changes
moved
These CSS files were relocated into the `wwwroot/Content` directory to align with ASP.NET Core's static file serving conventions, where `wwwroot` is the designated web root. This consolidation ensures all stylesheets—Bootstrap framework files, custom styles, and site-specific CSS—are served correctly by the static file middleware, replacing the legacy ASP.NET Web Forms `Content` folder structure.
View 9 files
Files moved to Pics directory related changes
moved
These 14 product images were relocated into the `wwwroot/Pics/` directory as part of aligning the eShop WebForms application with ASP.NET's `wwwroot` static file serving convention. By placing all catalog images under `wwwroot`, the application can serve them directly via the static files middleware without custom routing, which is a required structural change when modernizing a legacy WebForms app toward ASP.NET Core conventions.
View 14 files
Files moved to WebForms directory related changes
moved
These ten ASP.NET WebForms framework JavaScript files were relocated together into the `wwwroot/Scripts/WebForms/` directory to comply with ASP.NET Core's static file serving conventions. In ASP.NET Core, only files under `wwwroot` are served as static assets, so this collective move ensures all WebForms client-side scripts remain accessible to the browser during the modernization migration.
View 10 files
Files moved to MSAjax directory related changes
moved
These 11 Microsoft Ajax JavaScript library files were relocated into a dedicated `MSAjax` subdirectory under `Scripts/WebForms/`, consolidating all ASP.NET Ajax client-side scripts into a single, logically grouped folder. This reorganization establishes a clear separation between WebForms infrastructure scripts and other JavaScript assets, improving maintainability and aligning static file organization with the `wwwroot` conventions of the modernized ASP.NET application.
View 11 files
Files moved to Scripts directory related changes
moved
All client-side JavaScript assets — jQuery 3.4.1, Bootstrap, Popper.js, Modernizr, Respond.js, and site.js — were consolidated into a single `wwwroot/Scripts` directory. This aligns with ASP.NET Core's static file serving conventions under `wwwroot`, replacing the legacy Web Forms `Scripts` folder location at the project root, ensuring the static file middleware serves these assets correctly in the modernized application.
View 19 files
Files moved to esm directory related changes
moved
Four Popper.js files (the main library and its utilities, in both full and minified forms) were moved into a dedicated `esm` subdirectory within the Scripts folder. This reorganization separates the ES Module format of Popper.js from other script formats (UMD, CommonJS), enabling the application to explicitly reference the correct module format for modern browser consumption.
View 4 files
Files moved to umd directory related changes
moved
These four Popper.js files — the core library and its utilities, in both full and minified forms — were relocated into a dedicated `umd` subdirectory to explicitly organize them by their Universal Module Definition format. This separation distinguishes UMD-formatted scripts from other module formats within the Scripts directory, enabling cleaner dependency management and more intentional script loading in the modernized WebForms application.
View 4 files
Build Errors (1)
List of build errors in the transformation. Builds are performed on a Linux machine and may not match the errors if you build on a Windows environment.


1


Error Code
	
Description
	
Location

Error Code
	
Description
	
Location

CS5001
Program does not contain a static 'Main' method suitable for an entry point
eShopModernizedWebForms.csproj
eShopLegacyMVC.sln
Download NuGet Package Changes
Transformation details for eShopLegacyMVC.sln

Solution Transformation Summary
Details about the transformation settings and configuration
Combined Modernization Summary
The changes across eShopLegacy.Utilities, eShopLegacyMVC, and eShopPorted migrate the full solution from .NET Framework (4.6.1 / 4.7.2 / 4.9.x) to .NET 10.0, replacing legacy ASP.NET MVC 5 and System.Web dependencies with ASP.NET Core equivalents.

Project Files
All .csproj files converted from legacy MSBuild format to SDK-style (Microsoft.NET.Sdk / Microsoft.NET.Sdk.Web)
Target frameworks updated to net10.0; eShopLegacyMVC sets OutputType to Exe
packages.config removed across all projects; dependencies consolidated into <PackageReference> entries
Explicit <Compile>, <Content>, and <Reference> item declarations removed in favor of SDK-style auto-discovery
AssemblyInfo.cs files removed; assembly metadata now managed via SDK-generated assembly info
Migrations folder excluded from all build actions in eShopPorted
Selected package version changes:

| Package | Old Version | New Version | |---|---|---| | Autofac | 4.9.1 | 8.4.0 | | Autofac.Extensions.DependencyInjection | 4.4.0 | 10.0.0 | | EntityFramework | — | 6.5.1 | | EntityFrameworkCore | 2.2.6 | 9.0.10 | | EFCore.Relational | 2.2.6 | 10.0.0 | | EFCore.SqlServer | 2.2.6 | 9.0.9 | | log4net | 2.0.10 | 3.2.0 | | Newtonsoft.Json | 13.0.2 | 13.0.4 | | Microsoft.AspNetCore.SystemWebAdapters | — | 2.3.0 (new) |

Application Entry Point and Configuration
Global.asax / Global.asax.cs removed; replaced by Program.cs using the WebApplication builder pattern
Program.cs configures middleware pipeline, DI, session, localization, routing, and CatalogDBContext initialization; includes a static ConfigurationManager shim bridging IConfiguration to legacy call sites
appsettings.json replaces Web.config, Web.Debug.config, and Web.Release.config for connection strings and app settings
App_Start/ files (BundleConfig.cs, FilterConfig.cs, RouteConfig.cs, WebApiConfig.cs) removed; their responsibilities transferred to Program.cs middleware and routing configuration
Controllers
System.Web.Mvc and System.Web.Http namespaces replaced with Microsoft.AspNetCore.Mvc and Microsoft.AspNetCore.Http
ApiController replaced with ControllerBase decorated with [ApiController] and [Route("[controller]")]
IHttpActionResult return type replaced with IActionResult
API response method replacements:

| Legacy | ASP.NET Core | |---|---| | HttpStatusCodeResult(HttpStatusCode.BadRequest) | StatusCode(StatusCodes.Status400BadRequest) | | HttpNotFound() | NotFound() | | ResponseMessage(new HttpResponseMessage(...)) | NotFound() / Ok() | | [Bind(Include = "...")] | [Bind("...")] |

PicController: IWebHostEnvironment injected; Server.MapPath("~/Pics") replaced with _webHostEnvironment.WebRootPath
AddUriPlaceHolder: Url.RouteUrl(...) call body removed — pending reimplementation
Views
Views/_ViewImports.cshtml added, centralizing @using directives and registering ASP.NET Core Tag Helpers globally via @addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers
@Styles.Render() / @Scripts.Render() bundle helper calls replaced with explicit <link> and <script> tags
_Layout.cshtml: HttpContext.Current.Session["key"] replaced with Context.Session.GetString("key")
UTF-8 BOM removed from all .cshtml files (CatalogTable.cshtml, Create.cshtml, Delete.cshtml, Details.cshtml, Edit.cshtml, Index.cshtml, Error.cshtml, _Layout.cshtml, _ViewStart.cshtml)
Static Assets
All static assets relocated to wwwroot/ to align with ASP.NET Core's IStaticFileMiddleware convention:

| Source | Destination | |---|---| | Content/ | wwwroot/Content/ | | Scripts/ | wwwroot/Scripts/ | | Images/ | wwwroot/Images/ | | favicon.ico | wwwroot/favicon.ico |

Hardcoded paths referencing ~/Scripts/ require updating. Files outside wwwroot are not served without an explicit PhysicalFileProvider configuration.

Serialization (eShopLegacy.Utilities)
BinaryFormatter replaced with System.Text.Json.JsonSerializer in Serializing.cs
SerializeBinary: serializes object to JSON string, writes UTF-8 bytes to MemoryStream
DeserializeBinary: reads UTF-8 bytes from stream, deserializes via JsonSerializer.Deserialize(json, typeof(object))
BinaryFormatter is obsolete and disabled by default in .NET 5+ due to deserialization vulnerabilities (remote code execution risk)
JsonSerializer.Deserialize(..., typeof(object)) returns JsonElement, not the original type; callers relying on strongly-typed deserialization need to update to the generic JsonSerializer.Deserialize<T>() overload
Outstanding Issues
EF Core package version mismatch: EFCore.Relational at 10.0.0 vs EFCore.SqlServer at 9.0.9 — runtime compatibility requires verification
Migrations excluded from build; re-scaffolding may be required against the updated EF Core version
Autofac.Mvc5 is MVC5-specific and incompatible with ASP.NET Core's DI pipeline without Autofac.Extensions.DependencyInjection
Any remaining <Content> or <None> MSBuild items referencing pre-wwwroot paths should be updated to avoid build warnings or missing file errors
Linux Readiness
Details about the Linux readiness assessment and recommendations
eShopLegacy — Overall Linux Compatibility Assessment
Executive Summary
The Linux readiness assessment covers three projects: eShopLegacy.Utilities, eShopLegacyMVC, and eShopPorted. Across the portfolio, 47 total findings were identified — 43 high-severity and 4 low-severity — distributed unevenly across the three projects. eShopLegacy.Utilities presents no blockers. eShopLegacyMVC and eShopPorted share a common set of incompatible legacy packages and carry the majority of the remediation work required before Linux deployment.

Portfolio Readiness Score: Medium — 43 High-Severity and 4 Low-Severity Items Requiring Attention

| Project | High Severity | Low Severity | Total Findings | Readiness | |---|---|---|---|---| | eShopLegacy.Utilities | 0 | 2 | 2 | High | | eShopLegacyMVC | 28 | 2 | 30 | Low | | eShopPorted | 15 | 2 | 17 | Medium | | Portfolio Total | 43 | 6 | 49 | Medium |

Items Requiring Attention
Cross-platform compatibility items to address across the portfolio:

Legacy Package Blockers (High Severity — eShopLegacyMVC, eShopPorted)
Both eShopLegacyMVC and eShopPorted reference the same core set of Windows-bound or legacy-framework-specific packages:

Antlr-3.5.0.2 — cross-platform incompatible; present in both projects
Autofac.Mvc5-4.0.2 — MVC5-specific DI integration; cross-platform incompatible; present in both projects
autofac.webapi2-6.0.1 — WebAPI2-specific DI integration; cross-platform incompatible; present in eShopLegacyMVC
WebGrease-1.6.0 — cross-platform incompatible; present in both projects
Microsoft.AspNet.Mvc-5.2.7, Microsoft.AspNet.Razor-3.2.7, Microsoft.AspNet.WebPages-3.2.7, Microsoft.AspNet.WebApi.Core-5.2.7, Microsoft.AspNet.WebApi.WebHost-5.2.7 — full legacy ASP.NET stack; Windows-only; present in eShopLegacyMVC
Microsoft.AspNet.Web.Optimization-1.1.3, Microsoft.AspNet.SessionState.SessionStateModule-1.1.0, Microsoft.AspNet.TelemetryCorrelation-1.0.5 — legacy ASP.NET support packages; present in eShopLegacyMVC
Microsoft.ApplicationInsights.Agent.Intercept-2.4.0, Microsoft.CodeDom.Providers.DotNetCompilerPlatform-2.0.1, Microsoft.Web.Infrastructure-1.0.0 — Windows-bound infrastructure packages; present in eShopLegacyMVC
System.Configuration.ConfigurationManager-10.0.0, System.Diagnostics.DiagnosticSource-4.5.1 — flagged as cross-platform incompatible; present in eShopLegacyMVC
Authentication and Authorization (High Severity — eShopLegacyMVC)
IISConfigFeature detected — IIS-based authentication configuration is not portable to Linux
ASP.NET Core API Findings (High Severity — eShopLegacyMVC, eShopPorted)
Standard ASP.NET Core MVC methods — Ok(), Json(), View(), RedirectToAction(), Dispose(), CreateBuilder(), Build(), MapControllerRoute(), Run() — flagged as cross-platform incompatible across controller and program entry-point files in both projects; these APIs originate from Microsoft.AspNetCore.Mvc.Core and Microsoft.AspNetCore.Mvc.ViewFeatures 10.0.0 SDK packages and require runtime verification to confirm or dismiss the reported incompatibilities
File Permission APIs (Low Severity — All Projects)
FileAccessControlFeature detected across all three projects — ACL-based file permission APIs present; behavior differs between Windows and Linux POSIX permission models
Recommended Actions:

Replace the Microsoft.AspNet.* package set in eShopLegacyMVC with ASP.NET Core equivalents already partially present in that project
Replace Autofac.Mvc5 and autofac.webapi2 in both affected projects with Autofac.Extensions.DependencyInjection
Replace Antlr-3.5.0.2 and WebGrease-1.6.0 with actively maintained, cross-platform compatible alternatives in both eShopLegacyMVC and eShopPorted
Migrate IIS-based authentication in eShopLegacyMVC to ASP.NET Core middleware-based authentication
Transition System.Configuration.ConfigurationManager usage in eShopLegacyMVC to Microsoft.Extensions.Configuration
Validate all flagged ASP.NET Core MVC API findings against actual ASP.NET Core 10.0 cross-platform support documentation before investing remediation effort
Review file permission APIs across all three projects against Linux POSIX semantics
Additional Verification Items
Items to validate for optimal Linux deployment across the portfolio:

ASP.NET Core API runtime behavior — flagged controller and program APIs in eShopLegacyMVC and eShopPorted require Linux runtime validation to confirm cross-platform compatibility
External service connectivity — HttpLibraryFeature detected in all three projects; HTTP client behavior and endpoint reachability must be confirmed in the target Linux environment
File permission behavior — FileAccessControlFeature present in all three projects; file access operations must be tested on a Linux host
Recommended Validation:

Deploy each project to a Linux environment and execute targeted runtime tests covering the flagged ASP.NET Core MVC API surface
Verify all external service endpoints are reachable from the Linux target and that no Windows-specific networking assumptions are encoded in HTTP client configurations
Test file permission operations on Linux hosts for all three projects to confirm functional equivalence with existing Windows behavior
Dependencies Review
Packages requiring replacement or verification across the portfolio:

| Package | Affected Projects | Status | |---|---|---| | Antlr-3.5.0.2 | eShopLegacyMVC, eShopPorted | Cross-platform incompatible | | Autofac.Mvc5-4.0.2 | eShopLegacyMVC, eShopPorted | Cross-platform incompatible | | autofac.webapi2-6.0.1 | eShopLegacyMVC | Cross-platform incompatible | | WebGrease-1.6.0 | eShopLegacyMVC, eShopPorted | Cross-platform incompatible | | Microsoft.AspNet.Web.Optimization-1.1.3 | eShopLegacyMVC | Cross-platform incompatible | | System.Configuration.ConfigurationManager-10.0.0 | eShopLegacyMVC | Cross-platform incompatible | | System.Diagnostics.DiagnosticSource-4.5.1 | eShopLegacyMVC | Cross-platform incompatible | | Microsoft.ApplicationInsights.Agent.Intercept-2.4.0 | eShopLegacyMVC | Cross-platform incompatible | | Microsoft.AspNetCore.Mvc.Core-10.0.0-SDK | eShopLegacyMVC, eShopPorted | Flagged APIs require verification | | Microsoft.AspNetCore.Mvc.ViewFeatures-10.0.0-SDK | eShopLegacyMVC, eShopPorted | Flagged APIs require verification |

Recommended Updates:

Consolidate Autofac package replacement across eShopLegacyMVC and eShopPorted simultaneously, targeting Autofac.Extensions.DependencyInjection as the single ASP.NET Core-compatible integration point
Replace Microsoft.AspNet.Web.Optimization and WebGrease with ASP.NET Core-native bundling and minification support in eShopLegacyMVC
Transition System.Configuration.ConfigurationManager to Microsoft.Extensions.Configuration in eShopLegacyMVC, aligning with the configuration model already present in Program.cs
Evaluate System.Diagnostics.DiagnosticSource-4.5.1 for an update compatible with the ASP.NET Core 10.0 SDK in use
Evaluate Microsoft.ApplicationInsights.Agent.Intercept-2.4.0 against the current Application Insights SDK for ASP.NET Core as a cross-platform replacement path
Confirm whether flagged SDK package findings for Microsoft.AspNetCore.Mvc.Core and Microsoft.AspNetCore.Mvc.ViewFeatures represent false positives before treating them as active blockers
Transformation Overview
PARTIALLY TRANSFORMED
Detailed transformation results overview

Projects
Total
3
Completely Transformed
2
Partially Transformed
1
Skipped
0
NuGet Packages
Added
43
Removed
15
Updated
22
Files
Added
4
Removed
9
Updated
29
Moved
56
Projects (3)
List of projects in the transformation


1


Project Name
	
Status
	
Files Updated
	
Files Removed
	
Files Added
	
NuGet Added
	
NuGet Removed
	
NuGet Updated
	
Build Errors

Project Name
	
Status
	
Files Updated
	
Files Removed
	
Files Added
	
NuGet Added
	
NuGet Removed
	
NuGet Updated
	
Build Errors

eShopLegacy.Utilities
COMPLETELY TRANSFORMED
2
1
0
0
0
0
0
eShopLegacyMVC
COMPLETELY TRANSFORMED
16
8
3
38
15
13
0
eShopPorted
PARTIALLY TRANSFORMED
11
0
1
5
0
9
1
eShopLegacy.Utilities
COMPLETELY TRANSFORMED
Transformation details for eShopLegacyMVCSolution/eShopLegacy.Utilities

Project Summary
eShopLegacy.Utilities Modernization Summary
The eShopLegacy.Utilities project has been modernized from .NET Framework 4.6.1 to .NET 10.0, converting the project file to SDK-style format and replacing BinaryFormatter serialization with System.Text.Json.

eShopLegacy.Utilities.csproj
Converted from legacy MSBuild format (ToolsVersion="15.0") to SDK-style (Microsoft.NET.Sdk)
Replaced <TargetFrameworkVersion>v4.6.1</TargetFrameworkVersion> with <TargetFramework>net10.0</TargetFramework>
Removed explicit <Reference> entries for BCL assemblies (e.g., System, System.Core, System.Xml) — resolved automatically by the SDK
Removed manual <Compile> item entries — SDK-style projects include .cs files by default
Removed per-configuration PropertyGroup blocks (Debug/Release output paths, debug symbols)
Properties/AssemblyInfo.cs — Deleted
File removed; assembly metadata attributes (AssemblyTitle, AssemblyVersion, AssemblyCopyright, etc.) are now managed directly in the .csproj via SDK-generated assembly info
Serializing.cs
Breaking change: BinaryFormatter replaced with System.Text.Json.JsonSerializer
SerializeBinary: serializes object to JSON string, writes UTF-8 bytes to MemoryStream
DeserializeBinary: reads UTF-8 bytes from stream, deserializes via JsonSerializer.Deserialize(json, typeof(object))
Security: BinaryFormatter is obsolete and disabled by default in .NET 5+ due to deserialization vulnerabilities (remote code execution risk)
using var memoryStream uses declaration syntax (C# 8+)
⚠️ Note: JsonSerializer.Deserialize(..., typeof(object)) returns JsonElement, not the original type — callers relying on strongly-typed deserialization will need to update to use the generic JsonSerializer.Deserialize<T>() overload
File Changes (3)
List of file changes in the transformation


1


File(s)
	
Change Type
	
Summary
	
Code Diff

File(s)
	
Change Type
	
Summary
	
Code Diff

eShopLegacyMVCSolution/eShopLegacy.Utilities/Properties/AssemblyInfo.cs
removed
The manual AssemblyInfo.cs file was deleted because the SDK-style project format automatically generates assembly metadata from the .csproj properties, making the explicit file redundant and eliminating the maintenance burden of keeping two sources of truth synchronized.
View file
eShopLegacyMVCSolution/eShopLegacy.Utilities/Serializing.cs
updated
BinaryFormatter was removed because it is obsolete and disabled by default in .NET 5+ due to critical deserialization security vulnerabilities. The replacement uses System.Text.Json, which is the modern, secure serialization API built into .NET, eliminating the security risk while preserving the stream-based interface contract.
View file
eShopLegacyMVCSolution/eShopLegacy.Utilities/eShopLegacy.Utilities.csproj
updated
The project file was migrated from the legacy MSBuild format targeting .NET Framework 4.6.1 to the SDK-style format targeting net10.0. This removes verbose boilerplate including explicit file references, build configuration blocks, and GAC assembly references, which are all handled automatically by the modern SDK.
View file
eShopLegacyMVC
COMPLETELY TRANSFORMED
Transformation details for eShopLegacyMVCSolution/src/eShopLegacyMVC/wwwroot/favicon.ico

Project Summary
eShopLegacyMVC → ASP.NET Core (.NET 10) Modernization Summary
The project modernizes from ASP.NET MVC 5 / .NET Framework 4.7.2 to ASP.NET Core on .NET 10. The following sections consolidate all changes across the codebase.

Project Structure
Project SDK changed from legacy MSBuild web format to Microsoft.NET.Sdk.Web; OutputType set to Exe
Target framework updated from net472 to net10.0
packages.config removed; all dependencies consolidated into <PackageReference> entries in the .csproj
Explicit <Compile> and <Content> item declarations removed in favor of SDK-style auto-discovery
Global.asax.cs explicitly excluded via <Compile Remove="Global.asax.cs" />
Updated package versions:

| Package | New Version | |---|---| | Autofac | 8.4.0 | | EntityFramework | 6.5.1 | | log4net | 3.2.0 | | bootstrap | 5.3.1 | | jQuery | 3.7.0 | | Newtonsoft.Json | 13.0.4 | | Microsoft.AspNetCore.SystemWebAdapters | 2.3.0 (new) |

Deleted Files
| File | Reason | |---|---| | Global.asax / Global.asax.cs | Replaced by Program.cs | | App_Start/BundleConfig.cs | System.Web.Optimization bundle pipeline removed | | App_Start/FilterConfig.cs | HandleErrorAttribute global filter removed | | App_Start/RouteConfig.cs | RouteCollection-based routing removed | | App_Start/WebApiConfig.cs | HttpConfiguration-based Web API routing removed | | Properties/AssemblyInfo.cs | Replaced by SDK-style assembly metadata | | packages.config | Replaced by .csproj <PackageReference> entries |

New Files
Program.cs — WebApplication builder pattern replaces HttpApplication. Configures middleware pipeline, DI, session, localization, routing, and CatalogDBContext initialization. Includes a static ConfigurationManager shim bridging IConfiguration to legacy ConfigurationManager call sites
appsettings.json — Replaces Web.config, Web.Debug.config, and Web.Release.config for connection strings and app settings
Views/_ViewImports.cshtml — Global @using directives and @addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers
Controller Changes
System.Web.Mvc / System.Web.Http namespaces replaced with Microsoft.AspNetCore.Mvc
ApiController base class replaced with ControllerBase decorated with [ApiController] and [Route("[controller]")]
IHttpActionResult return type replaced with IActionResult
API response method replacements:

| Legacy | ASP.NET Core | |---|---| | HttpStatusCodeResult(HttpStatusCode.BadRequest) | StatusCode(StatusCodes.Status400BadRequest) | | HttpNotFound() | NotFound() | | ResponseMessage(new HttpResponseMessage(...)) | NotFound() / Ok() | | [Bind(Include = "...")] | [Bind("...")] |

PicController: IWebHostEnvironment injected; Server.MapPath("~/Pics") replaced with _webHostEnvironment.WebRootPath
AddUriPlaceHolder: Url.RouteUrl(...) call body removed — pending reimplementation
View Changes
@Styles.Render() / @Scripts.Render() bundle helper calls replaced with explicit <link> and <script> tags
_Layout.cshtml: HttpContext.Current.Session["key"] replaced with Context.Session.GetString("key"); @using Microsoft.AspNetCore.Http added
Create.cshtml / Edit.cshtml: @Scripts.Render("~/bundles/jqueryval") replaced with direct jQuery Validation script references
Static Assets
All static assets relocated from project root-level directories to wwwroot/, aligning with ASP.NET Core's IStaticFileMiddleware convention, which serves content from wwwroot by default when app.UseStaticFiles() is configured.

| Source | Destination | |---|---| | Content/ | wwwroot/Content/ | | Scripts/ | wwwroot/Scripts/ | | Images/ | wwwroot/Images/ | | favicon.ico | wwwroot/favicon.ico |

Affected script files include: bootstrap.min.js, jQuery variants (3.3.1, slim, intellisense), jquery.validate and jquery.validate.unobtrusive variants, modernizr variants, popper.js and related ESM/UMD variants, respond.js variants, and index.d.ts.

Note: Any hardcoded paths referencing ~/Scripts/ require updating to resolve correctly under the wwwroot root. Files outside wwwroot are not served without an explicit PhysicalFileProvider configuration. Verify that any remaining <Content> or <None> MSBuild items referencing old paths are updated to avoid build warnings or missing file errors.

NuGet Package Changes (66)
List of nuget package changes in the transformation


1
2
3
4


Package Name
	
Change Type
	
Old Version
	
New Version

Package Name
	
Change Type
	
Old Version
	
New Version

Microsoft.AspNetCore.SystemWebAdapters
added
-
2.3.0
System.Configuration.ConfigurationManager
added
-
*
Autofac
added
-
8.4.0
EntityFramework
added
-
6.5.1
microsoft.aspnet.webapi.client
added
-
6.0.0
System.IO.Compression
added
-
4.3.0
log4net
added
-
3.2.0
Microsoft.ApplicationInsights
added
-
2.23.0
microsoft.applicationinsights.windowsserver.telemetrychannel
added
-
2.23.0
bootstrap
added
-
5.3.1
jQuery
added
-
3.7.0
jQuery.Validation
added
-
1.19.5
Microsoft.ApplicationInsights.DependencyCollector
added
-
2.23.0
Microsoft.ApplicationInsights.PerfCounterCollector
added
-
2.23.0
Microsoft.ApplicationInsights.Web
added
-
2.21.0
Microsoft.ApplicationInsights.WindowsServer
added
-
2.23.0
Microsoft.jQuery.Unobtrusive.Validation
added
-
4.0.0
popper.js
added
-
1.16.1
autofac.webapi2
added
-
6.0.1
Microsoft.AspNet.WebApi
added
-
5.2.7
File Changes (33)
List of file changes in the transformation


1
2


File(s)
	
Change Type
	
Summary
	
Code Diff

File(s)
	
Change Type
	
Summary
	
Code Diff

eShopLegacyMVCSolution/src/eShopLegacyMVC/Program.cs
added
This file was created to migrate the application from ASP.NET MVC (using Global.asax and Web.config) to ASP.NET Core's minimal hosting model. Connection strings and settings previously in Web.config are hardcoded into in-memory configuration. A static ConfigurationManager shim bridges legacy code that relied on System.Configuration.ConfigurationManager.
View file
eShopLegacyMVCSolution/src/eShopLegacyMVC/Views/_ViewImports.cshtml
added
Created as the ASP.NET Core replacement for the legacy `Web.config` namespace registrations and global imports. It centralizes namespace imports for all views and explicitly enables Tag Helpers via `@addTagHelper`, which replaces HTML Helpers as the convention in ASP.NET Core Razor views.
View file
eShopLegacyMVCSolution/src/eShopLegacyMVC/appsettings.json
added
Introduced as the ASP.NET Core configuration replacement for Web.config/App.config. Connection strings, logging configuration, and application settings previously stored in `<appSettings>` and `<connectionStrings>` XML sections are migrated to this JSON format consumed by the ASP.NET Core configuration system.
View file
Files moved to wwwroot directory related changes
moved
The favicon.ico file was relocated to the `wwwroot` directory, aligning with ASP.NET Core's static file serving convention. In ASP.NET Core, only files within `wwwroot` are served directly by the static file middleware, unlike classic ASP.NET MVC. This move ensures the favicon remains publicly accessible without custom routing configuration.
View 2 files
Files moved to Content directory related changes
moved
These CSS files were relocated to the `wwwroot/Content/` directory to align with ASP.NET Core's static file serving conventions, where `wwwroot` is the designated web root. This consolidation groups all stylesheet assets — Bootstrap framework files, reboot/grid variants, and application-specific styles — under a single, conventionally structured path, enabling the static file middleware to serve them correctly.
View 9 files
Files moved to Images directory related changes
moved
These four UI image assets — brand logo, dark brand variant, main banner, and footer text — were relocated into a dedicated `Images` directory within `wwwroot`. This consolidates all static image resources under a single, organized folder, establishing a consistent convention for static asset management that separates images from other asset types like scripts and stylesheets.
View 4 files
Files moved to Scripts directory related changes
moved
These JavaScript library files—jQuery, Bootstrap, Popper, Modernizr, Respond, and jQuery Validation—were consolidated into a single `wwwroot/Scripts` directory, establishing a standardized static asset layout consistent with ASP.NET Core conventions. Moving all client-side scripts together enforces a clear separation between server-side code and front-end resources, simplifying asset management and reference paths across the application.
View 27 files
Files moved to esm directory related changes
moved
The Popper.js library files (both full and minified versions of popper.js and popper-utils.js) were relocated into a dedicated `esm` subdirectory, separating the ES Module format builds from other script formats. This organizes client-side dependencies by module system, enabling cleaner import paths and distinguishing ESM-compatible builds from UMD or CommonJS variants within the project's static asset structure.
View 4 files
Files moved to umd directory related changes
moved
Four Popper.js files (full and minified versions of both `popper.js` and `popper-utils.js`) were moved into a dedicated `umd` subdirectory within the Scripts folder. This reorganization groups the Universal Module Definition (UMD) formatted builds together, separating them from other script formats and improving asset organization to reflect the specific module format these files use.
View 4 files
eShopLegacyMVCSolution/src/eShopLegacyMVC/App_Start/BundleConfig.cs
removed
Deleted because ASP.NET Core does not include the System.Web.Optimization bundling framework. Static asset bundling is handled through alternative tooling (Webpack, LibMan, etc.), which is why the views switched to explicit script tags.
View file
eShopLegacyMVCSolution/src/eShopLegacyMVC/App_Start/FilterConfig.cs
removed
Deleted because ASP.NET Core's middleware pipeline replaces the MVC filter registration pattern. Global error handling is now configured directly in the middleware pipeline via `UseExceptionHandler`, making this class redundant.
View file
eShopLegacyMVCSolution/src/eShopLegacyMVC/App_Start/RouteConfig.cs
removed
Deleted because ASP.NET Core handles routing through middleware configuration in Startup.cs/Program.cs using attribute routing and conventional routing, replacing the legacy System.Web.Routing/System.Web.Mvc RouteCollection registration pattern.
View file
eShopLegacyMVCSolution/src/eShopLegacyMVC/App_Start/WebApiConfig.cs
removed
Deleted because ASP.NET Core integrates API routing directly into the unified MVC middleware pipeline, eliminating the need for a separate Web API configuration class that was specific to the legacy System.Web.Http framework.
View file
eShopLegacyMVCSolution/src/eShopLegacyMVC/Global.asax
removed
Deleted because ASP.NET Core eliminates the HttpApplication lifecycle model entirely, replacing it with the Startup.cs/Program.cs middleware pipeline, making this file architecturally obsolete.
View file
eShopLegacyMVCSolution/src/eShopLegacyMVC/Global.asax.cs
removed
Deleted entirely because ASP.NET Core eliminates the `HttpApplication`-based startup lifecycle. Application bootstrapping, DI registration, routing, and session initialization move to `Startup.cs`/`Program.cs`.
View file
eShopLegacyMVCSolution/src/eShopLegacyMVC/Properties/AssemblyInfo.cs
removed
Deleted because .NET Core/.NET 5+ projects use the SDK-style .csproj format, which auto-generates assembly metadata from project file properties, making a manual AssemblyInfo.cs redundant. The log4net XML configurator attribute also signals a logging framework replacement.
View file
eShopLegacyMVCSolution/src/eShopLegacyMVC/packages.config
removed
Deleted because ASP.NET Core uses SDK-style `.csproj` `<PackageReference>` elements for dependency management, making the NuGet `packages.config` format obsolete.
View file
eShopLegacyMVCSolution/src/eShopLegacyMVC/Controllers/Api/CatalogController.cs
updated
Replaced System.Web.Mvc dependencies with ASP.NET Core equivalents. `HttpStatusCodeResult` became `StatusCodeResult`, `HttpNotFound()` became `NotFound()`, and the `Bind(Include=...)` syntax was updated to ASP.NET Core's `Bind(...)`. The `AddUriPlaceHolder` method body was cleared because `Request.Url.Scheme` doesn't exist in ASP.NET Core's HttpRequest API.
View file
eShopLegacyMVCSolution/src/eShopLegacyMVC/Controllers/CatalogController.cs
updated
Replaced System.Web.Mvc dependencies with ASP.NET Core equivalents. `HttpStatusCodeResult` became `StatusCodeResult`, `HttpNotFound()` became `NotFound()`, and the `Bind(Include=...)` syntax was updated to ASP.NET Core's `Bind(...)`. The `AddUriPlaceHolder` method body was cleared because `Request.Url.Scheme` doesn't exist in ASP.NET Core's HttpRequest API.
View file
eShopLegacyMVCSolution/src/eShopLegacyMVC/Controllers/PicController.cs
updated
Migrated from ASP.NET MVC 5 (`System.Web.Mvc`) to ASP.NET Core MVC. `IWebHostEnvironment` replaces `Server.MapPath()` for physical path resolution, `StatusCodeResult`/`NotFound()` replace legacy `HttpStatusCodeResult`/`HttpNotFound()`, and `Microsoft.AspNetCore.Hosting/Http` namespaces replace `System.Web` equivalents.
View file
eShopPorted
PARTIALLY TRANSFORMED
Transformation details for eShopLegacyMVCSolution/eShopPorted/Controllers/PicController.cs

Project Summary
eShopLegacyMVC → ASP.NET Core Modernization Summary
The diff reflects a modernization of the eShopPorted project from .NET Framework 4.6.1 to .NET 10.0, replacing System.Web.Mvc dependencies with ASP.NET Core equivalents and updating the full NuGet package graph.

eShopPorted.csproj
Target framework: net461 → net10.0
Migrations folder excluded from all build actions (Compile, Content, EmbeddedResource, None)
Package updates (selected):
| Package | Old Version | New Version | |---|---|---| | Autofac | 4.9.1 | 8.4.0 | | Autofac.Extensions.DependencyInjection | 4.4.0 | 10.0.0 | | EntityFrameworkCore | 2.2.6 | 9.0.10 | | EFCore.Relational | 2.2.6 | 10.0.0 | | EFCore.SqlServer | 2.2.6 | 9.0.9 | | log4net | 2.0.10 | 3.2.0 | | Newtonsoft.Json | 13.0.2 | 13.0.4 |

Added: Microsoft.AspNetCore.SystemWebAdapters v2.3.0 — bridges System.Web APIs during incremental modernization
Removed: Autofac.Mvc5 retained but note it targets MVC5; verify compatibility with ASP.NET Core pipeline
Controllers/PicController.cs
Replaced System.Web.Mvc with Microsoft.AspNetCore.Mvc and Microsoft.AspNetCore.Http
new HttpStatusCodeResult(HttpStatusCode.BadRequest) → new StatusCodeResult(StatusCodes.Status400BadRequest)
HttpNotFound() → NotFound()
Views/_ViewImports.cshtml (new file)
Centralizes @using directives across all views, replacing per-view namespace declarations
Registers ASP.NET Core Tag Helpers globally:
@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers
.cshtml View Files (Catalog + Shared)
Removed UTF-8 BOM (﻿) from all Razor view files:
CatalogTable.cshtml, Create.cshtml, Delete.cshtml, Details.cshtml, Edit.cshtml, Index.cshtml, Error.cshtml, _Layout.cshtml, _ViewStart.cshtml
No logic changes; strictly encoding normalization
Key Breaking Changes
System.Web.Mvc types are no longer available; all MVC primitives now resolve from Microsoft.AspNetCore.Mvc
EF Core version mismatch between packages (Relational at 10.0.0 vs SqlServer at 9.0.9) — verify runtime compatibility
Migrations are excluded from build; re-scaffolding may be required against the updated EF Core version
Autofac.Mvc5 is an MVC5-specific package and is not compatible with ASP.NET Core's DI pipeline without Autofac.Extensions.DependencyInjection
Build Errors Summary
Build Errors Summary
Package Version Conflict
Error: NU1605 - Package downgrade detected for Microsoft.EntityFrameworkCore.

| Detail | Value | |--------|-------| | Affected Project | eShopPorted.csproj | | Conflicting Package | Microsoft.EntityFrameworkCore | | Required Version | 10.0.0 (via Microsoft.EntityFrameworkCore.Relational) | | Resolved Version | 9.0.10 (direct project reference) |

Root Cause
Microsoft.EntityFrameworkCore.Relational 10.0.0 requires Microsoft.EntityFrameworkCore >= 10.0.0, but the project explicitly references Microsoft.EntityFrameworkCore 9.0.10, creating a downgrade conflict treated as an error.

Resolution
Add an explicit package reference to Microsoft.EntityFrameworkCore 10.0.0 in eShopPorted.csproj:

<ItemGroup>
    <!-- Align EF Core version with Microsoft.EntityFrameworkCore.Relational requirement -->
    <PackageReference Include="Microsoft.EntityFrameworkCore" Version="10.0.0" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Relational" Version="10.0.0" />
</ItemGroup>
* Builds are performed on a Linux machine and may not match the errors if you build on a Windows environment.
NuGet Package Changes (14)
List of nuget package changes in the transformation


1


Package Name
	
Change Type
	
Old Version
	
New Version

Package Name
	
Change Type
	
Old Version
	
New Version

Autofac.Mvc5
added
-
4.0.2
log4net
added
-
3.2.0
Newtonsoft.Json
added
-
13.0.4
WebGrease
added
-
1.6.0
Microsoft.AspNetCore.SystemWebAdapters
added
-
2.3.0
Microsoft.EntityFrameworkCore
updated
2.2.6
9.0.10
Autofac.Extensions.DependencyInjection
updated
4.4.0
10.0.0
Microsoft.AspNetCore
updated
2.2.0
2.3.9
Autofac
updated
4.9.1
8.4.0
Microsoft.AspNetCore.Mvc
updated
2.2.0
2.3.0
Microsoft.AspNetCore.StaticFiles
updated
2.2.0
2.3.9
Microsoft.EntityFrameworkCore.Design
updated
2.2.6
9.0.10
Microsoft.EntityFrameworkCore.Relational
updated
2.2.6
10.0.0
Microsoft.EntityFrameworkCore.SqlServer
updated
2.2.6
9.0.9
File Changes (12)
List of file changes in the transformation


1


File(s)
	
Change Type
	
Summary
	
Code Diff

File(s)
	
Change Type
	
Summary
	
Code Diff

eShopLegacyMVCSolution/eShopPorted/Views/_ViewImports.cshtml
added
Introduced as a new ASP.NET Core convention replacing the global namespace registrations previously defined in Web.config's `<system.web.webPages.razor>` section. It centralizes namespace imports and activates Tag Helpers via `@addTagHelper`, enabling HTML-native syntax for form elements and anchor tags instead of legacy `Html.BeginForm()` and `Html.ActionLink()` helpers.
View file
eShopLegacyMVCSolution/eShopPorted/Controllers/PicController.cs
updated
The controller was migrated from ASP.NET MVC 5 to ASP.NET Core MVC. `System.Web.Mvc` was replaced with `Microsoft.AspNetCore.Mvc`, and `HttpStatusCodeResult`/`HttpNotFound()` were replaced with their ASP.NET Core equivalents `StatusCodeResult`/`NotFound()`, aligning return types with the Core middleware pipeline and HTTP abstraction model.
View file
eShopLegacyMVCSolution/eShopPorted/Views/Catalog/CatalogTable.cshtml
updated
BOM character removed to prevent Razor parsing issues during the ASP.NET Core migration.
View file
eShopLegacyMVCSolution/eShopPorted/Views/Catalog/Create.cshtml
updated
BOM character removed to prevent Razor parsing issues during the ASP.NET Core migration.
View file
eShopLegacyMVCSolution/eShopPorted/Views/Catalog/Delete.cshtml
updated
BOM character removed to prevent Razor parsing issues during the ASP.NET Core migration.
View file
eShopLegacyMVCSolution/eShopPorted/Views/Catalog/Details.cshtml
updated
BOM character removed to prevent Razor parsing issues during the ASP.NET Core migration.
View file
eShopLegacyMVCSolution/eShopPorted/Views/Catalog/Edit.cshtml
updated
BOM character removed to prevent Razor parsing issues during the ASP.NET Core migration.
View file
eShopLegacyMVCSolution/eShopPorted/Views/Catalog/Index.cshtml
updated
BOM character removed to prevent Razor parsing issues during the ASP.NET Core migration.
View file
eShopLegacyMVCSolution/eShopPorted/Views/Shared/Error.cshtml
updated
BOM character removed for consistent UTF-8 encoding without BOM, required by ASP.NET Core's Razor engine.
View file
eShopLegacyMVCSolution/eShopPorted/Views/Shared/_Layout.cshtml
updated
BOM character removed for the same cross-platform encoding compatibility reason.
View file
eShopLegacyMVCSolution/eShopPorted/Views/_ViewStart.cshtml
updated
BOM (Byte Order Mark) character removed to ensure cross-platform compatibility in ASP.NET Core, which is sensitive to encoding artifacts that can cause parse errors.
View file
eShopLegacyMVCSolution/eShopPorted/eShopPorted.csproj
updated
The project was upgraded from .NET Framework 4.6.1 to .NET 10.0, requiring updated package versions compatible with modern .NET. Legacy migration files were excluded, `Autofac.Mvc5` was retained for compatibility, `Microsoft.AspNetCore.SystemWebAdapters` was added to bridge legacy `System.Web` dependencies, and inconsistent package versioning was normalized.
View file
Build Errors (1)
List of build errors in the transformation. Builds are performed on a Linux machine and may not match the errors if you build on a Windows environment.


1


Error Code
	
Description
	
Location

Error Code
	
Description
	
Location

NU1605
Warning As Error: Detected package downgrade: Microsoft.EntityFrameworkCore from 10.0.0 to 9.0.10. Reference the package directly from the project to select a different version. eShopPorted -> Microsoft.EntityFrameworkCore.Relational 10.0.0 -> Microsoft.EntityFrameworkCore (>= 10.0.0) eShopPorted -> Microsoft.EntityFrameworkCore (>= 9.0.10)
eShopPorted.csproj
Displaying items 1 to 1 of 1Displaying items 1 to 20 of 134Displaying items 1 to 20 of 43Displaying items 1 to 2 of 2Displaying items 1 to 10 of 10Displaying items 1 to 4 of 4Displaying items 1 to 7 of 7Displaying items 1 to 5 of 5Displaying items 1 to 2 of 2Displaying items 1 to 7 of 7Displaying items 1 to 5 of 5Displaying items 1 to 8 of 8Displaying items 1 to 2 of 2Displaying items 1 to 1 of 1Displaying items 1 to 1 of 1Displaying items 1 to 20 of 47Displaying items 1 to 20 of 64Displaying items 1 to 1 of 1Displaying items 1 to 1 of 1Displaying items 1 to 20 of 127Displaying items 1 to 20 of 68Displaying items 1 to 1 of 1Displaying items 1 to 3 of 3Displaying items 1 to 3 of 3Displaying items 1 to 20 of 66Displaying items 1 to 20 of 33Displaying items 1 to 14 of 14Displaying items 1 to 12 of 12Displaying items 1 to 1 of 1