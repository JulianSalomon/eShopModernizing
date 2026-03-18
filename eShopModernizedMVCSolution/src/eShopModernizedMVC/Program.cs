
    using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using eShopModernizedMVC.Models;
using eShopModernizedMVC.Models.Infrastructure;
using eShopModernizedMVC.Services;

namespace eShopModernizedMVC
{
    public static class CatalogConfiguration
    {
        public static bool UseMockData { get; set; } = false;
    }

    public class Program
    {
        public static void Main(string[] args)
        {
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpContextAccessor();

            // Store configuration in static ConfigurationManager
            ConfigurationManager.Configuration = builder.Configuration;

            // Add services to the container (formerly ConfigureServices)
            builder.Services.AddControllersWithViews();

            // Register database initializer
            if (!CatalogConfiguration.UseMockData)
            {
                builder.Services.AddScoped<IDatabaseInitializer<CatalogDBContext>, CatalogDBInitializer>();
            }

            //Added Services

            var app = builder.Build();

            // Configure database initializer
            if (!CatalogConfiguration.UseMockData)
            {
using (var scope = app.Services.CreateScope())
                {
                    var initializer = scope.ServiceProvider.GetService<IDatabaseInitializer<CatalogDBContext>>();
                    Database.SetInitializer(initializer);
                }
            }

            // Initialize catalog images
using (var scope = app.Services.CreateScope())
            {
                var imageService = scope.ServiceProvider.GetService<IImageService>();
                if (imageService != null)
                {
                    imageService.InitializeCatalogImages();
                }
            }

            // Configure the HTTP request pipeline (formerly Configure method)
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            // Configure globalization (en-US culture from Web.config)
            var supportedCultures = new[] { new CultureInfo("en-US") };
            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("en-US"),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures
            });

            //Added Middleware

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }

    public class ConfigurationManager
    {
        public static IConfiguration Configuration { get; set; }
    }
}
