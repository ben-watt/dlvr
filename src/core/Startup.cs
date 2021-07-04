using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.IO;
using messaging_sidecar_interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System;
using Microsoft.AspNetCore.Http;

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
            services.AddMessageProxy(_config);
        }

        public void Configure(IApplicationBuilder app, ILogger<Startup> logger)
        {
            app.UseRouting();

            _ = app.UseEndpoints(endpoints =>
            {
                endpoints.MapPost("/{publisher:required}/{topic:required}", PublishMessage(logger));
            });
        }

        private static RequestDelegate PublishMessage(ILogger<Startup> logger)
        {
            return async context =>
            {
                var topic = context.Request.RouteValues["topic"].ToString();
                var publisherName = context.Request.RouteValues["publisher"].ToString();

                var publisherFactory = context.RequestServices.GetService<IFactory<IPublish>>();
                using var streamReader = new StreamReader(context.Request.Body);
                var body = await streamReader.ReadToEndAsync();

                var publisher = publisherFactory?.Create(publisherName);
                if (publisher is null)
                {
                    logger.LogInformation("Unable to find {0} with the name: '{1}'", typeof(IPublish), publisherName);
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                }
                else
                {
                    var response = await publisher.Publish(topic, body);

                    if (response == ProcessResponse.Success)
                    {
                        logger.LogInformation("Published message via path: {0}", context.Request.Path);
                        context.Response.StatusCode = (int)HttpStatusCode.OK;
                    }
                    else
                    {
                        logger.LogInformation("Failed to publish message via path: {0}", context.Request.Path);
                        context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    }

                }
            };
        }
    }
}
