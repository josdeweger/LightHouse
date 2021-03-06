﻿using System;
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

        public static ServiceProvider InitServices()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddSingleton<ILogger>(
                new LoggerConfiguration()
                    .MinimumLevel
                    .Debug()
                    .WriteTo.Console()
                    .CreateLogger());

            serviceCollection.AddAutoMapper(GetAssembliesStartingWith("LightHouse."));
            serviceCollection.AddTransient<DevOpsClient>();
            serviceCollection.AddTransient<TfsClient>();
            serviceCollection.AddTransient<IProvideBuilds, OptionBasedBuildProvider>();
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