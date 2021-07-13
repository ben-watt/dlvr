using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;

namespace messaging_sidecar
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration(configBuilder =>
                {
                    configBuilder.AddInMemoryCollection(
                        new Dictionary<string, string>()
                        {
                            { "Logging:LogLevel:Default", "Information" },
                            { "Logging:LogLevel:Microsoft", "Warning" },
                            { "Logging:LogLevel:Microsoft.Hosting.Lifetime", "Information" }
                        });

                    configBuilder.AddYamlFile("./config.yaml");
                    configBuilder.AddEnvironmentVariables();
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
