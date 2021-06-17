using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.IO;
using messaging_sidecar_interfaces;
using Microsoft.Extensions.Logging;

namespace messaging_sidecar
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMessageProxy();
        }

        public void Configure(IApplicationBuilder app, ILogger<Startup> logger)
        {
            app.UseRouting();

            _ = app.UseEndpoints(endpoints =>
              {
                endpoints.MapPost("/{publisher:required}/{topic:required}", async context =>
                  {
                      var topic = context.Request.RouteValues["topic"].ToString();
                      var publisherName = context.Request.RouteValues["publisher"].ToString();

                      var publisherFactory = context.RequestServices.GetService<IFactory<IPublish>>();
                      using var streamReader = new StreamReader(context.Request.Body);
                      var body = await streamReader.ReadToEndAsync();

                      var publisher = publisherFactory?.Create(publisherName);
                      if (publisher is null)
                      {
                          logger.LogInformation("Unable to find {0} with the name: {1}", typeof(IPublish), context.Request.Path);
                          context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                          return;
                      }

                      try
                      {
                          await publisher.Publish(topic, body);
                          logger.LogInformation("Published message via path: {0}", context.Request.Path);
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
