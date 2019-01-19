using LightHouse.Lib;
using Moq;
using Xunit;

namespace LightHouseSpecs.Lib
{
    public class BuildStatusLightControllerSpecs
    {
        [Fact]
        public void WhenThereAreNoLastBuildStatusResults_SignalLightIsTurnedOff()
        {
            var signalLightControllerMock = new Mock<IControlSignalLight>();

            var controller = new BuildStatusLightController(signalLightControllerMock.Object);

            var lastBuildStatusses = new LastBuildsStatus
            {
                AggregatedBuildStatus = AggregatedBuildStatus.None
            };

            controller.SetSignalLight(lastBuildStatusses);

            signalLightControllerMock.Verify(x => x.TurnOffAll(), Times.Once);
        }

        [Fact]
        public void WhenLastBuildFailedAndNoBuildInProgress_SignalLightIsTurnedRed()
        {
            var signalLightControllerMock = new Mock<IControlSignalLight>();

            var controller = new BuildStatusLightController(signalLightControllerMock.Object);

            var lastBuildStatusses = new LastBuildsStatus
            {
                AggregatedBuildResult = AggregatedBuildResult.Failed,
                AggregatedBuildStatus = AggregatedBuildStatus.Completed
            };

            controller.SetSignalLight(lastBuildStatusses);

            signalLightControllerMock.Verify(x => x.TurnOffAll(), Times.Once);
            signalLightControllerMock.Verify(x => x.TurnOnColor(SignalLightColor.Red, It.IsAny<byte>(), false), Times.Once);
        }

        [Fact]
        public void WhenLastBuildFailedAndBuildInProgress_SignalLightIsFlashingRed()
        {
            var signalLightControllerMock = new Mock<IControlSignalLight>();

            var controller = new BuildStatusLightController(signalLightControllerMock.Object);

            var lastBuildStatusses = new LastBuildsStatus
            {
                AggregatedBuildResult = AggregatedBuildResult.Failed,
                AggregatedBuildStatus = AggregatedBuildStatus.InProgress
            };

            controller.SetSignalLight(lastBuildStatusses);

            signalLightControllerMock.Verify(x => x.TurnOffAll(), Times.Once);
            signalLightControllerMock.Verify(x => x.TurnOnColor(SignalLightColor.Red, It.IsAny<byte>(), true), Times.Once);
        }

        [Fact]
        public void WhenLastBuildSucceededAndNoBuildInProgress_SignalLightIsTurnedGreen()
        {
            var signalLightControllerMock = new Mock<IControlSignalLight>();

            var controller = new BuildStatusLightController(signalLightControllerMock.Object);

            var lastBuildStatusses = new LastBuildsStatus
            {
                AggregatedBuildResult = AggregatedBuildResult.Succeeded,
                AggregatedBuildStatus = AggregatedBuildStatus.Completed
            };

            controller.SetSignalLight(lastBuildStatusses);

            signalLightControllerMock.Verify(x => x.TurnOffAll(), Times.Once);
            signalLightControllerMock.Verify(x => x.TurnOnColor(SignalLightColor.Green, It.IsAny<byte>(), false), Times.Once);
        }

        [Fact]
        public void WhenLastBuildSucceededAndBuildInProgress_SignalLightIsFlashingGreen()
        {
            var signalLightControllerMock = new Mock<IControlSignalLight>();

            var controller = new BuildStatusLightController(signalLightControllerMock.Object);

            var lastBuildStatusses = new LastBuildsStatus
            {
                AggregatedBuildResult = AggregatedBuildResult.Succeeded,
                AggregatedBuildStatus = AggregatedBuildStatus.InProgress
            };

            controller.SetSignalLight(lastBuildStatusses);

            signalLightControllerMock.Verify(x => x.TurnOffAll(), Times.Once);
            signalLightControllerMock.Verify(x => x.TurnOnColor(SignalLightColor.Green, It.IsAny<byte>(), true), Times.Once);
        }

        [Fact]
        public void WhenLastBuildPartiallySucceededAndNoBuildInProgress_SignalLightIsTurnedOrange()
        {
            var signalLightControllerMock = new Mock<IControlSignalLight>();

            var controller = new BuildStatusLightController(signalLightControllerMock.Object);

            var lastBuildStatusses = new LastBuildsStatus
            {
                AggregatedBuildResult = AggregatedBuildResult.PartiallySucceeded,
                AggregatedBuildStatus = AggregatedBuildStatus.Completed
            };

            controller.SetSignalLight(lastBuildStatusses);

            signalLightControllerMock.Verify(x => x.TurnOffAll(), Times.Once);
            signalLightControllerMock.Verify(x => x.TurnOnColor(SignalLightColor.Orange, It.IsAny<byte>(), false), Times.Once);
        }

        [Fact]
        public void WhenLastBuildPartiallySucceededAndBuildInProgress_SignalLightIsFlashingOrange()
        {
            var signalLightControllerMock = new Mock<IControlSignalLight>();

            var controller = new BuildStatusLightController(signalLightControllerMock.Object);

            var lastBuildStatusses = new LastBuildsStatus
            {
                AggregatedBuildResult = AggregatedBuildResult.PartiallySucceeded,
                AggregatedBuildStatus = AggregatedBuildStatus.InProgress
            };

            controller.SetSignalLight(lastBuildStatusses);

            signalLightControllerMock.Verify(x => x.TurnOffAll(), Times.Once);
            signalLightControllerMock.Verify(x => x.TurnOnColor(SignalLightColor.Orange, It.IsAny<byte>(), true), Times.Once);
        }
    }
}
