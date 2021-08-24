using System.Collections.Generic;
using System.Globalization;
using AutoMapper;
using Backend.Api.Extensions;
//using Backend.Models.Config;
using Backend.Setup;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Backend.Api
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(o => o.AddPolicy(name: MyAllowSpecificOrigins, builder =>
            {
                builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            }));

            services.AddOptions();
            services.AddLocalization();

            var configuration = new MapperConfiguration(cfg =>
                cfg.AddMaps(new[] {
                    "Backend.Api",
                    "Backend.Infrastructure"
                })
            );
            var mapper = configuration.CreateMapper();
            services.AddSingleton(mapper);

            services.AddAppMVC();
            services.AddFluent();
            services.AddAppLogging();

            services.AddApiVersioning(o =>
            {
                o.AssumeDefaultVersionWhenUnspecified = true;
                o.DefaultApiVersion = new ApiVersion(1, 0);
                o.ReportApiVersions = true;
            });
            
            
            services.AddAppSwaggerGen();
            //services.Configure<AutoAperturaConfig>(Configuration.GetSection("AutoApertura"));
            //services.Configure<ByPassConfig>(Configuration.GetSection("ByPass"));
            
            services.AddAppDependencies(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            else
                app.UseHsts();

            app.UseAppExceptionsMiddleware();
            
            app.UseRequestLocalization(BuildLocalizationOptions());

            app.UseCors(MyAllowSpecificOrigins);

            app.UseStaticFiles();  //Added Code
            if (!env.IsProduction())
                app.UseAppSwagger();
            app.UseStatusCodePages();
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();
            app.UseAuthentication();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            
        }
        
        private RequestLocalizationOptions BuildLocalizationOptions()
        {
            var supportedCultures = new List<CultureInfo>
            {
                new CultureInfo("es-CL")
            };

            var options = new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("es-CL"),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures
            };

            return options;
        }
    }

    
}