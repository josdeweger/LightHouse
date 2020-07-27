using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using LightHouse.Lib;
using LightHouse.UI.Logging;
using LightHouse.UI.Models;
using LightHouse.UI.Persistence;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Abstractions;
using ReactiveUI.Validation.Contexts;
using ReactiveUI.Validation.Extensions;
using Serilog;

namespace LightHouse.UI.ViewModels
{
    public class LighthouseViewModel : ViewModelBase, IValidatableViewModel
    {
        private readonly InMemorySink _inMemorySink;
        private readonly ILogger _logger;
        private readonly IWatchBuilds _buildsWatcher;
        private readonly IControlBuildStatusLight _buildStatusLightController;
        private readonly IControlSignalLight _signalLightController;
        private readonly Db _db;
        private readonly string _settingsCollection = "Settings";

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

        public LighthouseViewModel(
            Db db,
            ILogger logger, 
            InMemorySink inMemmorySink,
            IWatchBuilds buildsWatcher, 
            IControlBuildStatusLight buildStatusLightController, 
            IControlSignalLight signalLightController)
        {
            _db = db;
            _logger = logger;
            _inMemorySink = inMemmorySink;
            _inMemorySink.Events.CollectionChanged += LogEventsCollectionChangedHandler;
            _buildsWatcher = buildsWatcher;
            _buildStatusLightController = buildStatusLightController;
            _signalLightController = signalLightController;

            IsRunning = false;
            ButtonText = "Start";
            LighthouseSettings = GetSettings();
            SetValidationRules();
            StartStopLighthouse = ReactiveCommand.Create(OnStartStopClick, this.IsValid());
        }

        private LighthouseSettings GetSettings()
        {
            var maybeSettings = _db.FindById<LighthouseSettings>(_settingsCollection, 1);

            if (maybeSettings.HasValue)
                return maybeSettings.Value;

            return LighthouseSettings.Default();
        }

        private void OnStartStopClick()
        {
            _db.Save(LighthouseSettings, _settingsCollection);

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
                if (_buildStatusLightController?.IsConnected == false)
                    _logger.Information("Signal light not connected! Continuing without device...");

                await Start();
            });
        }

        private void LogEventsCollectionChangedHandler(object sender, NotifyCollectionChangedEventArgs e)
        {
            Logs = string.Join('\n', _inMemorySink.Events.Skip(Math.Max(0, _inMemorySink.Events.Count() - 50)));
        }

        private void StopLighthouse()
        {
            Task.Run(() =>
            {
                _signalLightController?.TurnOffAll();
                _buildsWatcher?.Stop();
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

            var teamProjects = LighthouseSettings.Projects
                .Split(',')
                .Select(p => p.TrimStart().TrimEnd())
                .ToList();

            var excludedBuildDefinitionIds = LighthouseSettings.ExcludeBuildDefinitionIds != null
                ? LighthouseSettings
                    .ExcludeBuildDefinitionIds
                    .Replace(" ", "")
                    .Split(',')
                    .Select(p => p.TrimStart().TrimEnd())
                    .Select(long.Parse)
                    .ToList()
                : new List<long>();

            var buildProviderSettings = new BuildProviderSettings
            {
                AccessToken = LighthouseSettings.Token,
                Collection = LighthouseSettings.Collection,
                Instance = LighthouseSettings.Instance,
                ExcludedBuildDefinitionIds = CreateExcludedBuildDefinitionIds(),
                TeamProjects = CreateTeamProjects()
            };

            await _buildsWatcher.Watch(
                buildService: LighthouseSettings.Service,
                buildProviderSettings: buildProviderSettings,
                refreshInterval: LighthouseSettings.RefreshInterval,
                onRefreshAction: ProcessBuildsStatus);
        }

        private List<string> CreateTeamProjects()
        {
            return LighthouseSettings.Projects
                .Split(',')
                .Select(p => p.TrimStart().TrimEnd())
                .ToList();
        }

        private List<long> CreateExcludedBuildDefinitionIds()
        {
            if(string.IsNullOrWhiteSpace(LighthouseSettings.ExcludeBuildDefinitionIds))
                return new List<long>();

            return LighthouseSettings
                .ExcludeBuildDefinitionIds
                .Replace(" ", "")
                .Split(',')
                .Select(long.Parse)
                .ToList();
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
                "You must specify valid projects (comma seperated, spaces allowed)");

            this.ValidationRule(
                x => x.LighthouseSettings.Token,
                token => !string.IsNullOrWhiteSpace(token),
                "You must specify a valid token");
        }
    }
}
