using System;
using System.Threading.Tasks;
using System.Timers;

namespace LightHouse.Lib
{
    public class BuildsWatcher : IWatchBuilds
    {
        private readonly IProvideLastBuildsStatus _buildsStatusProvider;

        public BuildsWatcher(IProvideLastBuildsStatus buildsStatusProvider)
        {
            _buildsStatusProvider = buildsStatusProvider;
        }

        public async Task Watch(Action<LastBuildsStatus> onRefreshAction, int intervalInMilliSeconds)
        {
            //first call
            await DetermineBuildStatus(onRefreshAction);

            //start timer for subsequent calls
            var timer = new Timer(intervalInMilliSeconds);
            timer.Elapsed += async (sender, args) => { await DetermineBuildStatus(onRefreshAction); };
            timer.Enabled = true;
        }

        private async Task DetermineBuildStatus(Action<LastBuildsStatus> onRefreshAction)
        {
            var buildsStatus = await _buildsStatusProvider.DetermineBuildStatus();
            onRefreshAction(buildsStatus);
        }
    }
}
