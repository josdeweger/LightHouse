using System;
using System.Threading.Tasks;
using CommandLine;
using LightHouse.Lib;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace LightHouse
{
    class Program
    {
        private static IWatchBuilds _buildsWatcher;
        private static IControlBuildStatusLight _buildStatusLightController;
        private static ILogger _logger;
        private static ServiceProvider _serviceProvider;

        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.ProcessExit += OnProcessExit;
            AppDomain.CurrentDomain.UnhandledException += UnhandledException;

            Parser
                .Default
                .ParseArguments<DevOpsOptions>(args)
                .WithParsed(async options =>
                {
                    _serviceProvider = Bootstrapper.InitServiceProvider(options);
                    _logger = _serviceProvider.GetService<ILogger>();
                    _buildsWatcher = _serviceProvider.GetService<IWatchBuilds>();
                    _buildStatusLightController = _serviceProvider.GetService<IControlBuildStatusLight>();

                    if (_buildStatusLightController?.IsConnected == true)
                        await Start(options);

                    _logger.Information("Press any key to exit");
                });

            Console.ReadKey();
        }

        private static async Task Start(DevOpsOptions options)
        {
            _logger.Information("Starting LightHouse with the following properties:");
            _logger.Information($"Service: {options.Service}");
            _logger.Information($"Instance: {options.Instance}");
            _logger.Information($"Collection: {options.Collection}");
            _logger.Information($"Team Projects: {string.Join(", ", options.TeamProjects)}");
            _logger.Information($"Personal Token: {options.PersonalToken}");
            _logger.Information($"Refresh interval: {options.RefreshInterval}");
            _logger.Information("Starting to watch build status...");

            await _buildsWatcher.Watch(ProcessBuildsStatus);
        }

        private static void UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            _logger?.Information("An unhandled exception occured.");
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
