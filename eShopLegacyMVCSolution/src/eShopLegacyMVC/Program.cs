
    using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using eShopLegacyMVC.Models;
using eShopLegacyMVC.Models.Infrastructure;
using eShopLegacyMVC.Modules;

namespace eShopLegacyMVC
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Migrate connection strings from Web.config
            builder.Configuration.AddInMemoryCollection(new Dictionary<string, string>
            {
                ["ConnectionStrings:CatalogDBContext"] = "Data Source=(localdb)\\MSSQLLocalDB; Initial Catalog=Microsoft.eShopOnContainers.Services.CatalogDb; Integrated Security=True; MultipleActiveResultSets=True;",
                ["UseMockData"] = "false",
                ["UseCustomizationData"] = "false"
            });

            // Store configuration in static ConfigurationManager
            ConfigurationManager.Configuration = builder.Configuration;

            // Add logging
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            builder.Logging.AddDebug();

            // Add services to the container (formerly ConfigureServices)
            builder.Services.AddControllersWithViews();

            // Configure globalization (en-US culture from Web.config)
            builder.Services.Configure<Microsoft.AspNetCore.Builder.RequestLocalizationOptions>(options =>
            {
                var culture = new CultureInfo("en-US");
                options.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture(culture);
                options.SupportedCultures = new List<CultureInfo> { culture };
                options.SupportedUICultures = new List<CultureInfo> { culture };
            });

            // Add session support (InProc session from Web.config)
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession();

            var useMockData = bool.Parse(builder.Configuration["UseMockData"] ?? "false");

            if (!useMockData)
            {
                builder.Services.AddScoped<IDatabaseInitializer<CatalogDBContext>, CatalogDBInitializer>();
            }

            // Register application module services


            var app = builder.Build();

            // Configure the database initializer
            if (!useMockData)
            {
using (var scope = app.Services.CreateScope())
                {
                    var initializer = scope.ServiceProvider.GetService<IDatabaseInitializer<CatalogDBContext>>();
                    Database.SetInitializer(initializer);
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

            app.UseRequestLocalization();

            app.UseRouting();

            app.UseSession();

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
