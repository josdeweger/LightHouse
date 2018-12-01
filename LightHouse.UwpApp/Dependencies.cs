using AutoMapper;
using LightHouse.BuildProviders.Vsts;
using LightHouse.Lib;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace LightHouse.UwpApp
{
    public class Dependencies
    {
        private static ServiceProvider _serviceProvider;

        public static ServiceProvider ServiceProvider => _serviceProvider ?? (_serviceProvider = InitServiceProvider());

        private static ServiceProvider InitServiceProvider()
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
                    "<token>", 
                    "<instance>",
                    "<collection>",
                    new [] {"<team project>"}));
            serviceCollection.AddTransient<IProvideLastBuildsStatus, LastBuildsStatusProvider>();

            return serviceCollection.BuildServiceProvider();
        }
    }
}