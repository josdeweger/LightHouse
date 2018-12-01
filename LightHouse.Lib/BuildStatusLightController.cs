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

        public void SetSignalLight(LastBuildsStatus buildsStatus)
        {
            var isBuildInProgress = buildsStatus.AggregatedBuildStatus.Equals(AggregatedBuildStatus.InProgress);
            _signalLightController.TurnOffAll();

            if (buildsStatus.AggregatedBuildStatus.Equals(AggregatedBuildStatus.None))
            {
                _signalLightController.TurnOffAll();
            }
            else
            {
                switch (buildsStatus.AggregatedBuildResult)
                {
                    case AggregatedBuildResult.Failed:
                        _signalLightController.TurnOnColor(SignalLightColor.Red, isBuildInProgress);
                        if (!isBuildInProgress)
                            _signalLightController.TurnOnBuzzer(4, 1, 0, 2);
                        break;
                    case AggregatedBuildResult.PartiallySucceeded:
                        _signalLightController.TurnOnColor(SignalLightColor.Orange, isBuildInProgress);
                        break;
                    case AggregatedBuildResult.Succeeded:
                        _signalLightController.TurnOnColor(SignalLightColor.Green, isBuildInProgress);
                        break;
                }
            }
        }
    }
}
