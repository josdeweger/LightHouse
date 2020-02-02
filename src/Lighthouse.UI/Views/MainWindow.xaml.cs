using System;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using LightHouse.Lib;
using Lighthouse.UI.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;

namespace Lighthouse.UI.Views
{
    public class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
