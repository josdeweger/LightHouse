using System;
using System.Threading.Tasks;

namespace LightHouse.Lib
{
    public interface IWatchBuilds
    {
        Task Watch(Action<LastBuildsStatus> onRefreshAction, int intervalInMilliSeconds);
    }
}