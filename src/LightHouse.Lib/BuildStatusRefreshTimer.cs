using System;
using System.Threading.Tasks;
using System.Timers;

namespace LightHouse.Lib
{
    public class BuildStatusRefreshTimer : ITimeBuildStatusRefresh
    {
        private Timer _timer;
        private Func<Task> _eventHandler;

        public ITimeBuildStatusRefresh OnElapsed(Func<Task> eventHandler)
        {
            _eventHandler = eventHandler;
            return this;
        }

        public async Task Start(double intervalInSeconds)
        {
            await _eventHandler();

            _timer = new Timer(intervalInSeconds * 1000);
            _timer.Elapsed += async (sender, args) => { await _eventHandler(); };
            _timer.Enabled = true;
            _timer.Start();
        }

        public void Stop()
        {
            _timer?.Stop();
        }

    }
}