using eShopModernizedWebForms.Middleware;
using Microsoft.AspNetCore.Owin;

using Microsoft.AspNetCore.Builder;


[assembly: OwinStartup(typeof(eShopModernizedWebForms.Startup))]

namespace eShopModernizedWebForms
{
    public partial class Startup
    {
        public void Configuration(IApplicationBuilder app)
        {
            if (CatalogConfiguration.UseAzureActiveDirectory)
            {
                ConfigureAuth(app);
            }
            else
            {
                app.UseMiddleware<AuthenticationMiddleware>();
            }
        }
    }
}
