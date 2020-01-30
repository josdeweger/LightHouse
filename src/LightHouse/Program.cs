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
        private static IControlSignalLight _signalLightController;
        private static ILogger _logger;
        private static ServiceProvider _serviceProvider;

        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.ProcessExit += OnProcessExit;
            AppDomain.CurrentDomain.UnhandledException += UnhandledException;
            
            var parser = new Parser(cfg => cfg.CaseInsensitiveEnumValues = true);
            
            parser
                .ParseArguments<Options>(args)
                .WithParsed(async options =>
                {
                    _serviceProvider = Bootstrapper.InitServices(options);
                    _logger = _serviceProvider.GetService<ILogger>();
                    _buildsWatcher = _serviceProvider.GetService<IWatchBuilds>();
                    _signalLightController = _serviceProvider.GetService<IControlSignalLight>();
                    _buildStatusLightController = _serviceProvider.GetService<IControlBuildStatusLight>();

                    if (_buildStatusLightController?.IsConnected == true)
                        await Start(options);

                    _logger.Information("Press any key to exit");
                });

            Console.ReadKey();
        }

        private static async Task Start(Options options)
        {
            _logger.Information("Starting LightHouse with the following properties:");
            _logger.Information($"Service: {options.Service}");
            _logger.Information($"Refresh interval: {options.RefreshInterval}");
            _logger.Information($"Brightness: {options.Brightness}");

            _logger.Information("Start sequence...");

            _signalLightController.Test();
                
            _logger.Information("Starting to watch build status...");

            await _buildsWatcher.Watch(lastBuildStatus =>
                ProcessBuildsStatus(lastBuildStatus, Convert.ToByte(options.Brightness), options.EnableFlashing));
        }

        private static void ProcessBuildsStatus(LastBuildsStatus buildsStatus, byte brightness, bool? enableFlashing)
        {
            _buildStatusLightController?.SetSignalLight(buildsStatus, enableFlashing, brightness);

            _logger?.Information(
                $"Build status: {buildsStatus.AggregatedBuildStatus.ToString()} | " +
                $"Build result: {buildsStatus.AggregatedBuildResult.ToString()}");
        }

        private static void UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            _logger?.Information("An unhandled exception occured.");
        }

        static void OnProcessExit(object sender, EventArgs e)
        {
            _logger?.Information("Exiting process");
            Bootstrapper.ServiceProvider?.GetService<IControlSignalLight>()?.TurnOffAll();
        }
    }
}
