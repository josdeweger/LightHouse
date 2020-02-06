using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using LightHouse.Lib;
using LightHouse.UI.Logging;
using LightHouse.UI.Persistence;
using LightHouse.UI.ViewModels;
using LightHouse.UI.Views;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace LightHouse.UI
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var serviceProvider = Bootstrapper.InitServices();

                desktop.MainWindow = new MainWindow
                {
                    DataContext = new LighthouseViewModel(
                        serviceProvider.GetService<Db>(),
                        serviceProvider.GetService<ILogger>(),
                        serviceProvider.GetService<InMemorySink>(),
                        serviceProvider.GetService<IWatchBuilds>(),
                        serviceProvider.GetService<IControlBuildStatusLight>(),
                        serviceProvider.GetService<IControlSignalLight>()
                    )
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
