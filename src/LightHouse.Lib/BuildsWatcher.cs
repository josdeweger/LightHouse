using System;
using System.Threading.Tasks;

namespace LightHouse.Lib
{
    public class BuildsWatcher : IWatchBuilds
    {
        private readonly IProvideLastBuildsStatus _buildsStatusProvider;
        private readonly ITimeBuildStatusRefresh _buildStatusRefreshTimer;

        public BuildsWatcher(IProvideLastBuildsStatus buildsStatusProvider, ITimeBuildStatusRefresh buildStatusRefreshTimer)
        {
            _buildsStatusProvider = buildsStatusProvider;
            _buildStatusRefreshTimer = buildStatusRefreshTimer;
        }

        public async Task Watch(Action<LastBuildsStatus> onRefreshAction)
        {
            _buildStatusRefreshTimer.OnElapsed(async () =>
            {
                var buildsStatus = await _buildsStatusProvider.DetermineBuildStatus();
                onRefreshAction(buildsStatus);
            });

            await _buildStatusRefreshTimer.Start();
        }
    }
}
