using System;
using System.Threading.Tasks;

namespace LightHouse.Lib
{
    public interface ITimeBuildStatusRefresh
    {
        ITimeBuildStatusRefresh OnElapsed(Func<Task> eventHandler);
        Task Start();
    }
}