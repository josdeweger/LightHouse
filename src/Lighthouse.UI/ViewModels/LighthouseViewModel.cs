using System;
using System.Collections.Specialized;
using System.Reactive;
using System.Threading.Tasks;
using LightHouse.Lib;
using Lighthouse.UI.Logging;
using Lighthouse.UI.Models;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using Serilog;
using SharpDX.Direct2D1.Effects;

namespace Lighthouse.UI.ViewModels
{
    public class LighthouseViewModel : ViewModelBase
    {
        private InMemorySink _inMemorySink;
        private static ILogger _logger;
        private static IWatchBuilds _buildsWatcher;
        private static IControlBuildStatusLight _buildStatusLightController;
        private static IControlSignalLight _signalLightController;
        
        private bool _isRunning;

        public bool IsRunning
        {
            get => _isRunning;
            set => this.RaiseAndSetIfChanged(ref _isRunning, value);
        }

        private string _buttonText;

        public string ButtonText
        {
            get => _buttonText;
            set => this.RaiseAndSetIfChanged(ref _buttonText, value);
        }

        private string _logs;

        public string Logs
        {
            get => _logs;
            set => this.RaiseAndSetIfChanged(ref _logs, value);
        }

        private LighthouseSettings _lighthouseSettings;

        public LighthouseSettings LighthouseSettings
        {
            get => _lighthouseSettings;
            set => this.RaiseAndSetIfChanged(ref _lighthouseSettings, value);
        }

        public ReactiveCommand<Unit, Unit> StartLighthouse { get; }

        public LighthouseViewModel()
        {
            IsRunning = false;
            ButtonText = "Start";
            LighthouseSettings = new LighthouseSettings();
            StartLighthouse = ReactiveCommand.Create(OnStartStopClick);
        }

        public void OnStartStopClick()
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
    }
}
