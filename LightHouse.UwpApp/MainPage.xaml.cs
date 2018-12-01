using System;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using LightHouse.Lib;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Microsoft.Toolkit.Uwp.UI.Extensions;

namespace LightHouse.UwpApp
{
    public sealed partial class MainPage : Page
    {
        private readonly IProvideLastBuildsStatus _lastBuildsStatusProvider;
        private ObservableCollection<Build> LastBuilds { get; } = new ObservableCollection<Build>();
        private const int RefreshIntervalInSeconds = 10;

        public MainPage()
        {
            InitializeComponent();

            _lastBuildsStatusProvider = Dependencies.ServiceProvider.GetService<IProvideLastBuildsStatus>();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            await UpdateLastBuilds();

            new Timer(async ev =>
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () => { await UpdateLastBuilds(); });
            }, null, TimeSpan.Zero, TimeSpan.FromSeconds(RefreshIntervalInSeconds));
        }

        private async Task UpdateLastBuilds()
        {
            var result = await _lastBuildsStatusProvider.DetermineBuildStatus();

            foreach (var updatedBuild in result.LastBuilds)
            {
                var build = LastBuilds.FirstOrDefault(b => b.DefinitionId.Equals(updatedBuild.DefinitionId));

                if (build != null)
                {
                    var index = LastBuilds.IndexOf(build);
                    LastBuilds[index] = updatedBuild;
                }
                else
                {
                    LastBuilds.Add(updatedBuild);
                }
            }
        }

        private void LastBuildStackPanel_OnLoaded(object sender, RoutedEventArgs e)
        {
            var stackPanel = sender as StackPanel;

            var definitionIdTextBox = (HeaderedTextBlock)stackPanel?.FindChildByName("DefinitionId");

            if (definitionIdTextBox == null)
                return;

            if (!int.TryParse(definitionIdTextBox.Text, out var definitionId))
                return;

            var lastBuild = LastBuilds.First(b => b.DefinitionId.Equals(definitionId));

            if (lastBuild.Status.Equals(BuildStatus.InProgress))
            {
                var colorAnimation = new ColorAnimation
                {
                    From = Colors.White,
                    To = Colors.Orange,
                    Duration = new Duration(TimeSpan.FromSeconds(1)),
                    AutoReverse = true,
                    RepeatBehavior = RepeatBehavior.Forever
                };

                var storyBoard = new Storyboard();
                storyBoard.Children.Add(colorAnimation);

                Storyboard.SetTarget(colorAnimation, stackPanel);
                Storyboard.SetTargetProperty(colorAnimation, "(ContentControl.Background).(SolidColorBrush.Color)");

                storyBoard.Begin();
            }
        }
    }
}
