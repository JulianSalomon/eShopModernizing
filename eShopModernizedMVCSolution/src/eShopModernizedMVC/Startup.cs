using eShopModernizedMVC.Middleware;
using Microsoft.AspNetCore.Owin;

using Microsoft.AspNetCore.Builder;




namespace eShopModernizedMVC
{
    public partial class Startup
    {
        public void Configuration(IApplicationBuilder app)
        {
            if (false)
            {
                ConfigureAuth(app);
            }
            else
            {
                app.UseMiddleware<AuthenticationMiddleware>();
            }
        }

        private void ConfigureAuth(IApplicationBuilder app)
        {
            app.UseAuthentication();
 }
    }
}
