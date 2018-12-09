using System;
using System.Threading.Tasks;
using System.Timers;

namespace LightHouse.Lib
{
    public class BuildStatusRefreshTimer : ITimeBuildStatusRefresh
    {
        private readonly double _intervalInSeconds;
        private Timer _timer;
        private Func<Task> _eventHandler;

        public BuildStatusRefreshTimer(double intervalInSeconds)
        {
            _intervalInSeconds = intervalInSeconds;
        }

        public ITimeBuildStatusRefresh OnElapsed(Func<Task> eventHandler)
        {
            _eventHandler = eventHandler;
            return this;
        }

        public async Task Start()
        {
            await _eventHandler();

            _timer = new Timer(_intervalInSeconds * 1000);
            _timer.Elapsed += async (sender, args) => { await _eventHandler(); };
            _timer.Enabled = true;
            _timer.Start();
        }
    }
}