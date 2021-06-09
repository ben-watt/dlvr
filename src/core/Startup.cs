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
            // Add multiple http clients with different retry policies dependant on config
            // for now just keep a default one
            services.AddHttpClient("default", x =>
            {
                // Create a default client which will point to the relevant application
                const string appPort = "5000";
                x.BaseAddress = new Uri($"http://localhost:{appPort}");
                x.Timeout = new TimeSpan(0, 0, 5);
            });

            // Register all subscribers with a common interface perhaps: 
            // IMessageProcessor or IMessageReceiver for inbuild processors and polling respectively
            // Background service will register all receivers and start listening
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

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
                      await publisher.Publish(body);

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
