using Autofac;
using eShopModernizedWebForms.Models;
using eShopModernizedWebForms.Models.Infrastructure;
using eShopModernizedWebForms.Services;
using Microsoft.Azure.Services.AppAuthentication;

public interface ISqlConnectionFactory
{
    string GetConnectionString();
}

public class ManagedIdentitySqlConnectionFactory : ISqlConnectionFactory
{
    private readonly string _baseConnectionString;

    public ManagedIdentitySqlConnectionFactory(string baseConnectionString = "")
    {
        _baseConnectionString = baseConnectionString;
    }

    public string GetConnectionString()
    {
        var tokenProvider = new AzureServiceTokenProvider();
        var accessToken = tokenProvider.GetAccessTokenAsync("https://database.windows.net/").GetAwaiter().GetResult();
        return _baseConnectionString;
    }
}

namespace eShopModernizedWebForms.Modules
{
    public class ApplicationModule : Module
    {
        private bool useMockData;
        private bool useAzureStorage;
        private bool useManagedIdentity;

        public ApplicationModule(bool useMockData, bool useAzureStorage, bool useManagedIdentity)
        {
            this.useMockData = useMockData;
            this.useAzureStorage = useAzureStorage;
            this.useManagedIdentity = useManagedIdentity;
        }

        protected override void Load(ContainerBuilder builder)
        {
            if (this.useMockData)
            {
                builder.RegisterType<CatalogServiceMock>()
                    .As<ICatalogService>()
                    .SingleInstance();
            }
            else
            {
                builder.RegisterType<CatalogService>()
                    .As<ICatalogService>()
                    .InstancePerLifetimeScope();
            }

            if (this.useAzureStorage)
            {
                builder.RegisterType<ImageAzureStorage>()
                    .As<IImageService>()
                    .InstancePerLifetimeScope();
            }
            else
            {
                builder.RegisterType<ImageMockStorage>()
                  .As<IImageService>()
                  .InstancePerLifetimeScope();
            }

            builder.RegisterType<CatalogDBContext>()
                .InstancePerLifetimeScope();

            builder.RegisterType<CatalogDBInitializer>()
                .InstancePerLifetimeScope();

            builder.RegisterType<CatalogItemHiLoGenerator>()
                .SingleInstance();

            if (this.useManagedIdentity)
            {
                builder.RegisterType<ManagedIdentitySqlConnectionFactory>()
                    .As<ISqlConnectionFactory>()
                    .SingleInstance();
            }
            else
            {
                builder.Register(c => "DefaultConnectionString")
                    .As<ISqlConnectionFactory>()
                    .SingleInstance();
            }
        }
    }
}
