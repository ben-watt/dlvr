﻿using messaging_sidecar;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.IO;

namespace component_tests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Startup>
    {
        private string _yamlFilePath;

        public void AddYamlFile(string path)
        {
            _yamlFilePath = path;
        }

        protected override IHostBuilder CreateHostBuilder()
        {
            return Host.CreateDefaultBuilder()
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
        }

        protected override IHost CreateHost(IHostBuilder builder)
        {
            builder.ConfigureAppConfiguration(configBuilder =>
            {
                configBuilder.SetBasePath(Directory.GetCurrentDirectory());
                configBuilder.AddYamlFile(_yamlFilePath);
            });

            return base.CreateHost(builder);
        }
    }
}
