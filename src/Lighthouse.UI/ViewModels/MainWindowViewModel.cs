using System;
using System.Reactive;
using System.Threading.Tasks;
using LightHouse.Lib;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;

namespace Lighthouse.UI.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private static IWatchBuilds _buildsWatcher;
        private static IControlBuildStatusLight _buildStatusLightController;
        private static IControlSignalLight _signalLightController;

        public string Instance => "http://rhm-p-tfs01.kantoor.tld:8080/tfs";
        public BuildService Service => BuildService.Tfs;
        public string Collection => "RedHotMinute";
        public string Projects => "Landal Evolution Git";
        public string ExcludeBuildDefinitionIds => "562,564";
        public string Token => "dwbufiaabjmezu4myxocdn5ukf7e3azyz3qhir5ydi5mq56mvjxq";
        public int RefreshInterval => 60;
        public int Brightness => 5;
        public bool EnableFlashing => true;

        public ReactiveCommand<Unit, Unit> StartLighthouse { get; }

        public MainWindowViewModel()
        {
            StartLighthouse = ReactiveCommand.Create(RunLighthouse);
        }

        public void RunLighthouse()
        {
            var serviceProvider = Bootstrapper.InitServices(this);
            _buildsWatcher = serviceProvider.GetService<IWatchBuilds>();
            _signalLightController = serviceProvider.GetService<IControlSignalLight>();
            _buildStatusLightController = serviceProvider.GetService<IControlBuildStatusLight>();

            //if (_buildStatusLightController?.IsConnected == true)
                Start(this).GetAwaiter().GetResult();
        }

        private static async Task Start(MainWindowViewModel viewModel)
        {
            //_signalLightController.Test();

            await _buildsWatcher.Watch(lastBuildStatus =>
                ProcessBuildsStatus(lastBuildStatus, Convert.ToByte(viewModel.Brightness), viewModel.EnableFlashing));
        }

        private static void ProcessBuildsStatus(LastBuildsStatus buildsStatus, byte brightness, bool? enableFlashing)
        {
            _buildStatusLightController?.SetSignalLight(buildsStatus, enableFlashing, brightness);
        }
    }
}
