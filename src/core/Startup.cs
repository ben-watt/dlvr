using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net;
using System.IO;
using Microsoft.Extensions.Configuration;
using messaging_sidecar_interfaces;
using service_bus_dependency_injection;

namespace messaging_sidecar
{
    public class Startup
    {
        private readonly IConfiguration _config;

        public Startup(IConfiguration config)
        {
            _config = config;
        }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClient("app", x =>
            {
                const string appPort = "5000";
                x.BaseAddress = new Uri($"http://localhost:{appPort}");
                x.Timeout = new TimeSpan(0, 0, 5);
            });

            var connectionString = "";

            services.AddServiceBusPublisher("test", config =>
            {
                config.ConnectionString = connectionString;
            });

            services.AddHttpHandler(name: "default", clientName: "app", endpoint: "/app-endpoint");

            services.AddServiceBusProcessor(config =>
            {
                config.ConnectionString = connectionString;
                config.TopcicName = "";
                config.SubscriptionName = "";
                config.HandlerName = "default";
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();

            _ = app.UseEndpoints(endpoints =>
              {
                // Handles messages from the sidecar application to service bus
                endpoints.MapPost("/v1/{topic:required}", async context =>
                  {
                      var topic = context.Request.RouteValues["topic"].ToString();
                      var publisher = context.RequestServices.GetRequiredService<IPublish>();

                      using var streamReader = new StreamReader(context.Request.Body);
                      var body = await streamReader.ReadToEndAsync();
                      await publisher.Publish(topic, body);

                      try
                      {
                          context.Response.StatusCode = (int)HttpStatusCode.OK;
                      }
                      catch
                      {
                          context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                      }
                  });
              });
        }
    }
}
