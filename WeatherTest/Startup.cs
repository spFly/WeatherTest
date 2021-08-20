using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Polly;
using StackExchange.Redis;
using System;
using WeatherTest.Services;

namespace WeatherTest
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClient("WeatherClient", client =>
            {
                client.BaseAddress = new Uri(Configuration.GetValue<string>("WeatherClient"));
            });

            var multiplexer = ConnectionMultiplexer.Connect(Configuration.GetConnectionString("redis"));
            services.AddSingleton<IConnectionMultiplexer>(multiplexer);

            services.AddScoped<IWeatherService, WeatherService>();
            services.AddScoped<ICityService, CityService>();

            services.AddMassTransit(x =>
            {
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(Configuration.GetValue<string>("RabbitMQHost"), "/", h =>
                    {
                        h.Username(Configuration.GetValue<string>("RabbitMQUser"));
                        h.Password(Configuration.GetValue<string>("RabbitMQPassword"));
                    });
                    cfg.ConfigureEndpoints(context);
                });
            });
            services.AddMassTransitHostedService();

            services.AddHostedService<TimedHostedService>();

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "WeatherTest", Version = "v1" });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WeatherTest v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
