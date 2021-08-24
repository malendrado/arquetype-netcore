using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace Backend.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        private static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((builderContext, config) =>
                {
                    var env = builderContext.HostingEnvironment;

                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                        .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                        .AddEnvironmentVariables();
                })
                .UseUrls("http://*:5000")
                #region Kestrel Linux
                /*.UseKestrel(options =>
                {
                    options.Limits.MaxConcurrentConnections = 400;
                    options.Limits.MaxConcurrentUpgradedConnections = 400;
                    options.Limits.MaxRequestBodySize = 10 * 1024;
                })*/
                #endregion
                #region IIS Windows
                .UseIISIntegration()
                #endregion
                .UseStartup<Startup>();
        }
    }
}