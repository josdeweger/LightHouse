using System;
using System.Linq;
using System.Reflection;
using AutoMapper;
using LightHouse.BuildProviders.DevOps;
using LightHouse.Delcom.SignalLight;
using LightHouse.Lib;
using LightHouse.UI.Logging;
using LightHouse.UI.Models;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace LightHouse.UI
{
    public class Bootstrapper
    {
        public static ServiceProvider ServiceProvider;

        public static ServiceProvider InitServices(LighthouseSettings options)
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddSingleton(new InMemorySink());
            serviceCollection.AddSingleton<ILogger>(provider =>
                new LoggerConfiguration()
                    .MinimumLevel
                    .Debug()
                    .WriteTo.Sink(provider.GetService<InMemorySink>())
                    .CreateLogger());

            serviceCollection.AddAutoMapper(GetAssembliesStartingWith("LightHouse."));

            var projects = options.Projects.Split(',').Select(p => p.TrimStart().TrimEnd()).ToList();
            var excludedBuildDefinitionIds = options
                .ExcludeBuildDefinitionIds
                .Replace(" ", "")
                .Split(',')
                .Select(p => p.TrimStart().TrimEnd())
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
            
            serviceCollection.AddSingleton<IWatchBuilds, BuildsWatcher>();
            serviceCollection.AddSingleton<ITimeBuildStatusRefresh>(x => new BuildStatusRefreshTimer());
            serviceCollection.AddSingleton<IProvideLastBuildsStatus, LastBuildsStatusProvider>();
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