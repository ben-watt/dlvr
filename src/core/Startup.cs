using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.IO;
using Microsoft.Extensions.Configuration;
using messaging_sidecar_interfaces;
using Microsoft.Extensions.Logging;

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
            services.AddMessageProxy();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            app.UseRouting();

            _ = app.UseEndpoints(endpoints =>
              {
                endpoints.MapPost("/v1/{publisher:required}/{topic:required}", async context =>
                  {
                      var topic = context.Request.RouteValues["topic"].ToString();
                      var publisherName = context.Request.RouteValues["publisher"].ToString();

                      var publisherFactory = context.RequestServices.GetRequiredService<IFactory<IPublish>>();
                      var publisher = publisherFactory.Create(publisherName);

                      using var streamReader = new StreamReader(context.Request.Body);
                      var body = await streamReader.ReadToEndAsync();
                      await publisher.Publish(topic, body);
                      logger.LogInformation("Published message via path: {0}", context.Request.Path);

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
