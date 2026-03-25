using CoreWCF;
using CoreWCF.Configuration;
using CoreWCF.Description;
using eShopWCFService;

var builder = WebApplication.CreateBuilder(args);

// Bridge connection string from appsettings.json to environment variable
// that CatalogConfiguration reads from
var connectionString = builder.Configuration.GetConnectionString("EntityModel");
if (!string.IsNullOrEmpty(connectionString))
{
    Environment.SetEnvironmentVariable("ConnectionString", connectionString);
}

// Add CoreWCF services
builder.Services.AddServiceModelServices();
builder.Services.AddServiceModelMetadata();
builder.Services.AddSingleton<IServiceBehavior, UseRequestHeadersForMetadataAddressBehavior>();

var app = builder.Build();

// Configure CoreWCF endpoints
app.UseServiceModel(serviceBuilder =>
{
    serviceBuilder.AddService<CatalogService>(serviceOptions =>
    {
        serviceOptions.DebugBehavior.IncludeExceptionDetailInFaults = true;
    });

    serviceBuilder.AddServiceEndpoint<CatalogService, ICatalogService>(
        new BasicHttpBinding(),
        "/CatalogService/CatalogService.svc");

    var serviceMetadataBehavior = app.Services.GetRequiredService<ServiceMetadataBehavior>();
    serviceMetadataBehavior.HttpGetEnabled = true;
});

app.Run();
