using System;
using System.Threading.Tasks;
using CommandLine;
using LightHouse.Lib;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace LightHouse.ConsoleApp
{
    class Program
    {
        private static IWatchBuilds _buildsWatcher;
        private static IControlBuildStatusLight _buildStatusLightController;
        private static ILogger _logger;
        private static ServiceProvider _serviceProvider;
        private const int RefreshIntervalInMilliSeconds = 30000;
        
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.ProcessExit += OnProcessExit;
            AppDomain.CurrentDomain.UnhandledException += UnhandledException;

            Parser
                .Default
                .ParseArguments<VstsOptions>(args)
                .WithParsed(async options =>
                {
                    _serviceProvider = Bootstrapper.InitServiceProvider(options);
                    _logger = _serviceProvider.GetService<ILogger>();
                    _buildsWatcher = _serviceProvider.GetService<IWatchBuilds>();
                    _buildStatusLightController = _serviceProvider.GetService<IControlBuildStatusLight>();

                    LogInitialInfo(options);

                    await Start();
                });

            while (!Console.KeyAvailable) ;
        }

        private static void LogInitialInfo(VstsOptions options)
        {
            _logger.Information("Starting LightHouse with the following properties:");
            _logger.Information($"Service: {options.Service}");
            _logger.Information($"Instance: {options.Instance}");
            _logger.Information($"Collection: {options.Collection}");
            _logger.Information($"Team Projects: {options.TeamProjects}");
            _logger.Information($"Personal Token: {options.PersonalToken}");
            _logger.Information("Starting to watch build status...");
            _logger.Information("Press any key to exit");
        }

        private static async Task Start()
        {
            if (_buildStatusLightController?.IsConnected == true)
                await _buildsWatcher.Watch(ProcessBuildsStatus, RefreshIntervalInMilliSeconds);
        }

        private static void UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            _logger?.Information("An unhandled exception occured with the following message.");
        }

        private static void ProcessBuildsStatus(LastBuildsStatus buildsStatus)
        {
            _buildStatusLightController?.SetSignalLight(buildsStatus);

            _logger?.Information(
                $"Build status: {buildsStatus.AggregatedBuildStatus.ToString()} | " +
                $"Build result: {buildsStatus.AggregatedBuildResult.ToString()}");
        }

        static void OnProcessExit(object sender, EventArgs e)
        {
            _logger?.Information("Exiting process");
            Bootstrapper.ServiceProvider?.GetService<IControlSignalLight>()?.TurnOffAll();
        }
    }
}
