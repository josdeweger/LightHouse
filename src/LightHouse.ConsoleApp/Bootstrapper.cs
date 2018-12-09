using AutoMapper;
using LightHouse.BuildProviders.Vsts;
using LightHouse.Delcom.SignalLight;
using LightHouse.Lib;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace LightHouse.ConsoleApp
{
    public class Bootstrapper
    {
        public static ServiceProvider ServiceProvider;

        public static ServiceProvider InitServiceProvider(VstsOptions options)
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
            serviceCollection.AddTransient<IVstsClient, VstsClient>(provider =>
                new VstsClient(
                    provider.GetService<ILogger>(), 
                    options.PersonalToken, 
                    options.Instance, 
                    options.Collection, 
                    options.TeamProjects));
            serviceCollection.AddTransient<IWatchBuilds, BuildsWatcher>();
            serviceCollection.AddTransient<IProvideLastBuildsStatus, LastBuildsStatusProvider>();
            serviceCollection.AddSingleton<IControlBuildStatusLight, BuildStatusLightController>();
            serviceCollection.AddSingleton<IControlSignalLight, SignalLightController>();

            ServiceProvider = serviceCollection.BuildServiceProvider();

            return ServiceProvider;
        }
    }
}