﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PlaneService.Domain;
using System.Threading;
using System.Threading.Tasks;
using Utils;

namespace PlaneService
{
    public class PlaneBackgroundService : BackgroundService
    {
        private readonly ILogger<PlaneBackgroundService> _logger;
        private readonly PlaneLifetimeManager _planeLifetimeManager;
        
        public PlaneBackgroundService(
            ILogger<PlaneBackgroundService> logger,
            IConfiguration configuration, 
            IHostEnvironment hostEnvironment)
        {
            _logger = logger;

            var name = HostServiceNameSelector.AssignName("Plane", hostEnvironment.EnvironmentName, configuration.GetValue<string>("name"));
            var updatePlaneUrl = configuration.GetValue<string>("UpdatePlaneUrl");
            var addPlaneUrl = configuration.GetValue<string>("AddPlaneUrl");
            var getAirportUrl = configuration.GetValue<string>("GetAirportUrl");
            var getAirportsUrl = configuration.GetValue<string>("GetAirportsUrl");

            _planeLifetimeManager = new PlaneLifetimeManager(name, updatePlaneUrl, addPlaneUrl,
                getAirportUrl, getAirportsUrl);

            _logger.LogInformation("Created PlaneBackgroundService for: " + name);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _planeLifetimeManager.Start();

            while (!stoppingToken.IsCancellationRequested)
            {
                await _planeLifetimeManager.Loop();
                await Task.Delay(1000, stoppingToken);
            }

            _logger.LogInformation("CancellationRequested");
        }
    }
}
