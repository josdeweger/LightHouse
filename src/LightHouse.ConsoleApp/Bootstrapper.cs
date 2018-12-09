using AutoMapper;
using LightHouse.BuildProviders.DevOps;
using LightHouse.Delcom.SignalLight;
using LightHouse.Lib;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace LightHouse.ConsoleApp
{
    public class Bootstrapper
    {
        public static ServiceProvider ServiceProvider;

        public static ServiceProvider InitServiceProvider(DevOpsOptions options)
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddSingleton<ILogger>(
                new LoggerConfiguration()
                    .MinimumLevel
                    .Debug()
                    .WriteTo.Console()
                    .CreateLogger());
            serviceCollection.AddAutoMapper();
            serviceCollection.AddTransient<IProvideBuilds, BuildProvider>();
            serviceCollection.AddTransient<IDevOpsClient, DevOpsClient>(provider =>
                new DevOpsClient(
                    provider.GetService<ILogger>(), 
                    options.PersonalToken, 
                    options.Instance, 
                    options.Collection, 
                    options.TeamProjects));
            serviceCollection.AddTransient<IWatchBuilds, BuildsWatcher>();
            serviceCollection.AddTransient<ITimeBuildStatusRefresh>(x => new BuildStatusRefreshTimer(options.RefreshInterval));
            serviceCollection.AddTransient<IProvideLastBuildsStatus, LastBuildsStatusProvider>();
            serviceCollection.AddSingleton<IControlBuildStatusLight, BuildStatusLightController>();
            serviceCollection.AddSingleton<IControlSignalLight, SignalLightController>();

            ServiceProvider = serviceCollection.BuildServiceProvider();

            return ServiceProvider;
        }
    }
}