namespace LightHouse.Lib
{
    public class BuildStatusLightController : IControlBuildStatusLight
    {
        private readonly IControlSignalLight _signalLightController;

        public bool IsConnected => _signalLightController.IsConnected;

        public BuildStatusLightController(IControlSignalLight signalLightController)
        {
            _signalLightController = signalLightController;
        }

        public void SetSignalLight(LastBuildsStatus buildsStatus, byte brightness = 5)
        {
            var isBuildInProgress = buildsStatus.AggregatedBuildStatus.Equals(AggregatedBuildStatus.InProgress);

            if (buildsStatus.AggregatedBuildStatus.Equals(AggregatedBuildStatus.None))
            {
                _signalLightController.TurnOffAll();
            }
            else
            {
                _signalLightController.TurnOffAll();

                switch (buildsStatus.AggregatedBuildResult)
                {
                    case AggregatedBuildResult.Failed:
                        _signalLightController.TurnOnColor(SignalLightColor.Red, brightness, isBuildInProgress);
                        break;
                    case AggregatedBuildResult.PartiallySucceeded:
                        _signalLightController.TurnOnColor(SignalLightColor.Orange, brightness, isBuildInProgress);
                        break;
                    case AggregatedBuildResult.Succeeded:
                        _signalLightController.TurnOnColor(SignalLightColor.Green, brightness, isBuildInProgress);
                        break;
                }
            }
        }
    }
}
