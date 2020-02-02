using System;
using System.Linq;
using System.Reflection;
using AutoMapper;
using LightHouse.BuildProviders.DevOps;
using LightHouse.Delcom.SignalLight;
using LightHouse.Lib;
using Lighthouse.UI.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Lighthouse.UI
{
    public class Bootstrapper
    {
        public static ServiceProvider ServiceProvider;

        public static ServiceProvider InitServices(MainWindowViewModel options)
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddSingleton<ILogger>(
                new LoggerConfiguration()
                    .MinimumLevel
                    .Debug()
                    .WriteTo.Console()
                    .CreateLogger());

            serviceCollection.AddAutoMapper(GetAssembliesStartingWith("Lighthouse."));

            var projects = options.Projects.Replace(" ", "").Split(',').ToList();
            var excludedBuildDefinitionIds = options
                .ExcludeBuildDefinitionIds
                .Replace(" ", "")
                .Split(',')
                .Select(long.Parse)
                .ToList();

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
                            projects,
                            excludedBuildDefinitionIds));
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
                            projects,
                            excludedBuildDefinitionIds));
                    break;
                default:
                    throw new Exception($"Unknown build service {options.Service}");
            }
            
            serviceCollection.AddTransient<IWatchBuilds, BuildsWatcher>();
            serviceCollection.AddTransient<ITimeBuildStatusRefresh>(x => new BuildStatusRefreshTimer(options.RefreshInterval));
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