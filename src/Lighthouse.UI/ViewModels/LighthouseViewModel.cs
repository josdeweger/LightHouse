using System;
using System.Collections.Specialized;
using System.Reactive;
using System.Threading.Tasks;
using LightHouse.Lib;
using Lighthouse.UI.Logging;
using Lighthouse.UI.Models;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Abstractions;
using ReactiveUI.Validation.Contexts;
using ReactiveUI.Validation.Extensions;
using Serilog;

namespace Lighthouse.UI.ViewModels
{
    public class LighthouseViewModel : ViewModelBase, IValidatableViewModel
    {
        private InMemorySink _inMemorySink;
        private static ILogger _logger;
        private static IWatchBuilds _buildsWatcher;
        private static IControlBuildStatusLight _buildStatusLightController;
        private static IControlSignalLight _signalLightController;

        public ValidationContext ValidationContext { get; } = new ValidationContext();

        [Reactive]
        public bool IsRunning { get; set; }

        [Reactive]
        public string ButtonText { get; set; }

        [Reactive]
        public string Logs { get; set; }

        [Reactive]
        public LighthouseSettings LighthouseSettings { get; set; }

        public ReactiveCommand<Unit, Unit> StartStopLighthouse { get; }

        public LighthouseViewModel()
        {
            IsRunning = false;
            ButtonText = "Start";
            LighthouseSettings = new LighthouseSettings();

            SetValidationRules();

            var canStart = this.IsValid();

            StartStopLighthouse = ReactiveCommand.Create(OnStartStopClick, canStart);
        }

        private void OnStartStopClick()
        {
            if (!IsRunning)
            {
                RunLighthouse();
                IsRunning = true;
                ButtonText = "Stop";
            }
            else
            {
                StopLighthouse();
                IsRunning = false;
                ButtonText = "Start";
            }
        }

        private void RunLighthouse()
        {
            Task.Run(async () =>
            {
                var serviceProvider = Bootstrapper.InitServices(LighthouseSettings);
                _logger = serviceProvider.GetService<ILogger>();
                _inMemorySink = serviceProvider.GetService<InMemorySink>();
                
                _inMemorySink.Events.CollectionChanged += LogEventsCollectionChangedHandler;
                
                _buildsWatcher = serviceProvider.GetService<IWatchBuilds>();
                _signalLightController = serviceProvider.GetService<IControlSignalLight>();
                _buildStatusLightController = serviceProvider.GetService<IControlBuildStatusLight>();

                //if (_buildStatusLightController?.IsConnected == true)
                await Start();
            });
        }

        private void LogEventsCollectionChangedHandler(object sender, NotifyCollectionChangedEventArgs e)
        {
            Logs = string.Join('\n', _inMemorySink.Events);
        }

        private void StopLighthouse()
        {
            Task.Run(() =>
            {
                Bootstrapper.ServiceProvider?.GetService<IControlSignalLight>()?.TurnOffAll();
                Bootstrapper.ServiceProvider?.GetService<IWatchBuilds>()?.Stop();
            });
        }

        private async Task Start()
        {
            _logger.Information("Starting LightHouse with the following properties:");
            _logger.Information($"Service: {LighthouseSettings.Service}");
            _logger.Information($"Refresh interval: {LighthouseSettings.RefreshInterval}");
            _logger.Information($"Brightness: {LighthouseSettings.Brightness}");
            _logger.Information($"Flashing enabled: {LighthouseSettings.EnableFlashing}");

            _logger.Information("Start sequence...");

            _signalLightController.Test();

            _logger.Information("Starting to watch build status...");

            await _buildsWatcher.Watch(
                onRefreshAction: ProcessBuildsStatus,
                refreshInterval: LighthouseSettings.RefreshInterval);
        }

        private void ProcessBuildsStatus(LastBuildsStatus buildsStatus)
        {
            _buildStatusLightController?.SetSignalLight(
                buildsStatus, 
                LighthouseSettings.EnableFlashing,
                Convert.ToByte(LighthouseSettings.Brightness));

            _logger?.Information(
                $"Build status: {buildsStatus.AggregatedBuildStatus.ToString()} | " +
                $"Build result: {buildsStatus.AggregatedBuildResult.ToString()}");
        }

        private void SetValidationRules()
        {
            this.ValidationRule(
                x => x.LighthouseSettings.Instance,
                instance => !string.IsNullOrWhiteSpace(instance),
                "You must specify a valid instance");

            this.ValidationRule(
                x => x.LighthouseSettings.Collection,
                collection => !string.IsNullOrWhiteSpace(collection),
                "You must specify a valid collection");

            this.ValidationRule(
                x => x.LighthouseSettings.Service,
                service => service.GetType() == typeof(BuildService),
                "You must specify a valid service");

            this.ValidationRule(
                x => x.LighthouseSettings.Projects,
                projects => !string.IsNullOrWhiteSpace(projects),
                "You must specify a valid service");

            this.ValidationRule(
                x => x.LighthouseSettings.Token,
                token => !string.IsNullOrWhiteSpace(token),
                "You must specify a valid service");
        }
    }
}
