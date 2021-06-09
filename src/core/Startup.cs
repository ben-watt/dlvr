using DotNetCore.CAP;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Savorboard.CAP.InMemoryMessageQueue;
using System;
using System.Net;
using System.IO;
using Microsoft.Extensions.Configuration;

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
            // Add multiple http clients with different retry policies
            services.AddHttpClient("app", x =>
            {
                // Create a default client which will point to the relevant application
                const string appPort = "5000";
                x.BaseAddress = new Uri($"http://localhost:{appPort}");
                x.Timeout = new TimeSpan(0, 0, 5);
            });

            // Add subscribers mapping a subscription to a handler
            // Maybe able to subscribe to all events using the one handler
            // Then I can use the client as the way to specify the endpoint to hit
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                // Handles messages from the sidecar application to service bus
                endpoints.MapPost("/v1/{topic:required}", async context =>
                {
                    var topic = context.Request.RouteValues["topic"].ToString();
                    //var capPublisher = context.RequestServices.GetRequiredService<ICapPublisher>();

                    using var streamReader = new StreamReader(context.Request.Body);
                    var body = await streamReader.ReadToEndAsync();

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
