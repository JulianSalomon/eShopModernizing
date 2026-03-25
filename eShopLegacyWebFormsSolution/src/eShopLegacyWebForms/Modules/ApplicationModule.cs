using Autofac;
using eShopLegacyWebForms.Models;
using eShopLegacyWebForms.Models.Infrastructure;
using eShopLegacyWebForms.Services;

namespace eShopLegacyWebForms.Modules
{
    public class ApplicationModule : Module
    {
        private bool useMockData;
        private string connectionString;

        public ApplicationModule(bool useMockData, string connectionString = null)
        {
            this.useMockData = useMockData;
            this.connectionString = connectionString;
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

            if (!string.IsNullOrEmpty(connectionString))
            {
                builder.Register(c => new CatalogDBContext(connectionString))
                    .InstancePerLifetimeScope();
            }
            else
            {
                builder.RegisterType<CatalogDBContext>()
                    .InstancePerLifetimeScope();
            }

            builder.RegisterType<CatalogDBInitializer>()
                .InstancePerLifetimeScope();

            builder.RegisterType<CatalogItemHiLoGenerator>()
                .SingleInstance();
        }
    }
}