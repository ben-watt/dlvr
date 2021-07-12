using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.IO;
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

        public void Configure(IApplicationBuilder app)
        {
            app.UseRouting();

            _ = app.UseEndpoints(endpoints =>
            {
                endpoints.MapPost("/{publisher:required}/{topic:required}", SaveMessage());
            });
        }

        private static RequestDelegate SaveMessage()
        {
            return async context =>
            {
                var topic = context.Request.RouteValues["topic"].ToString();
                var publisherName = context.Request.RouteValues["publisher"].ToString();           

                using var streamReader = new StreamReader(context.Request.Body);
                var body = await streamReader.ReadToEndAsync();

                var store = context.RequestServices.GetRequiredService<InMemoryStore>();

                try
                {
                    await store.Add(new Message(publisherName, topic, body));
                    context.Response.StatusCode = (int)HttpStatusCode.OK;
                }
                catch(Exception ex)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                }
            };
        }
    }
}
