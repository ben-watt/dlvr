using messaging_sidecar;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;

namespace component_tests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Startup>
    {
        private string _yamlFilePath;
        private readonly List<Action<IServiceCollection>> _servicesToReplace = new();

        public void AddYamlFile(string path)
        {
            _yamlFilePath = path;
        }

        public void ReplaceService<T>(Action<IServiceCollection> addServiceAction)
        {
            _servicesToReplace.Add((IServiceCollection collection) =>
            {
                collection.RemoveAll<T>();
                addServiceAction(collection);
            });
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
            builder.ConfigureServices(config =>
            {
                foreach(var replaceServiceAction in _servicesToReplace)
                {
                    replaceServiceAction(config);
                }
            });

            builder.ConfigureAppConfiguration(configBuilder =>
            {
                configBuilder.SetBasePath(Directory.GetCurrentDirectory());
                if(_yamlFilePath != null)
                    configBuilder.AddYamlFile(_yamlFilePath);
            });

            return base.CreateHost(builder);
        }
    }
}
