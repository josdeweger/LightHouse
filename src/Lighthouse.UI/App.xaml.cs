using System;
using System.Linq;
using System.Reflection;
using AutoMapper;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using LightHouse.BuildProviders.DevOps;
using LightHouse.Delcom.SignalLight;
using LightHouse.Lib;
using Lighthouse.UI.ViewModels;
using Lighthouse.UI.Views;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Lighthouse.UI
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
                desktop.MainWindow = new MainWindow
                {
                    DataContext = new LighthouseViewModel(),
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
