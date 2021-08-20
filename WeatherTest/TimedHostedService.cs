using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using WeatherTest.Services;

namespace WeatherTest
{
    public class TimedHostedService : IHostedService, IDisposable
    {
        private readonly IServiceProvider services;
        private readonly IConfiguration configuration;
        private readonly ILogger<TimedHostedService> _logger;
        private Timer _timer;

        public TimedHostedService(IServiceProvider services, IConfiguration configuration, ILogger<TimedHostedService> logger)
        {           
            this.services = services;
            this.configuration = configuration;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Timed Hosted Service running.");

            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromSeconds(configuration.GetValue<int>("CachingTime")));

            return Task.CompletedTask;
        }

        private async void DoWork(object state)
        {
            using (var scope = services.CreateScope())
            {
                var weatherService =
                    scope.ServiceProvider
                        .GetRequiredService<IWeatherService>();

                await weatherService.CachingWeather();
            }            
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Timed Hosted Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
