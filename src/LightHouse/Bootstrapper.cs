using System;
using System.Linq;
using System.Reflection;
using AutoMapper;
using LightHouse.BuildProviders.DevOps;
using LightHouse.Delcom.SignalLight;
using LightHouse.Lib;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace LightHouse
{
    public class Bootstrapper
    {
        public static ServiceProvider ServiceProvider;

        public static ServiceProvider InitServices(Options options)
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddSingleton<ILogger>(
                new LoggerConfiguration()
                    .MinimumLevel
                    .Debug()
                    .WriteTo.Console()
                    .CreateLogger());

            serviceCollection.AddAutoMapper(GetAssembliesStartingWith("Lighthouse."));

            switch (options.Service)
            {
                case BuildService.DevOps:
                    serviceCollection.AddTransient<IProvideBuilds>(provider =>
                        new DevOpsClient(
                            provider.GetService<ILogger>(),
                            provider.GetService<IMapper>(),
                            provider.GetService<IUrlBuilder>(),
                            options.Token,
                            options.Instance,
                            options.Collection,
                            options.TeamProjects.ToList(),
                            options.ExcludeBuildDedfinitionIds.ToList()));
                    break;
                case BuildService.Tfs:
                    serviceCollection.AddTransient<IProvideBuilds>(provider =>
                        new TfsClient(
                            provider.GetService<ILogger>(),
                            provider.GetService<IMapper>(),
                            provider.GetService<IUrlBuilder>(),
                            options.Token,
                            options.Instance,
                            options.Collection,
                            options.TeamProjects.ToList(),
                            options.ExcludeBuildDedfinitionIds.ToList()));
                    break;
                default:
                    throw new Exception($"Unknown build service {options.Service}");
            }
            
            serviceCollection.AddTransient<IWatchBuilds, BuildsWatcher>();
            serviceCollection.AddTransient<ITimeBuildStatusRefresh>(x => new BuildStatusRefreshTimer());
            serviceCollection.AddTransient<IProvideLastBuildsStatus, LastBuildsStatusProvider>();
            serviceCollection.AddSingleton<IControlBuildStatusLight, BuildStatusLightController>();
            serviceCollection.AddSingleton<IControlSignalLight, SignalLightController>();
            serviceCollection.AddSingleton<IUrlBuilder, DevOpsUrlBuilder>();

            ServiceProvider = serviceCollection.BuildServiceProvider();

            return ServiceProvider;
        }

        private static Assembly[] GetAssembliesStartingWith(string assemblyNameStart)
        {
            return Assembly
                .GetExecutingAssembly()
                .GetReferencedAssemblies()
                .Where(a => a.Name.StartsWith(assemblyNameStart, StringComparison.OrdinalIgnoreCase))
                .Select(Assembly.Load)
                .ToArray();
        }
    }
}