using System;
using System.Threading.Tasks;

namespace LightHouse.Lib
{
    public interface IWatchBuilds
    {
        Task Watch(
            BuildService buildService,
            BuildProviderSettings buildProviderSettings,
            double refreshInterval,
            Action<LastBuildsStatus> onRefreshAction);

        void Stop();
    }
}