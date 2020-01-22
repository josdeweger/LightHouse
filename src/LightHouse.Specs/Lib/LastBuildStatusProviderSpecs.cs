using System.Collections.Generic;
using FluentAssertions;
using LightHouse.Lib;
using Moq;
using Xunit;
using Build = LightHouse.Lib.Build;

namespace LightHouseSpecs.Lib
{
    public class LastBuildStatusProviderSpecs
    {
        [Fact]
        public async void GivenOneOfLastBuildsFailed_WhenDeterminingBuildStatus_AggregatedResultIsFailed()
        {
            var buildsProviderMock = new Mock<IProvideBuilds>();
            var inProgressBuilds = new List<Build>();
            var lastBuildOne = new Build { Result = BuildResult.Failed, Status = BuildStatus.Completed};
            var lastBuildTwo = new Build { Result = BuildResult.Succeeded, Status = BuildStatus.Completed };

            buildsProviderMock.Setup(b => b.GetWithStatus(BuildStatus.InProgress)).ReturnsAsync(inProgressBuilds);
            buildsProviderMock.Setup(b => b.GetWithStatus(BuildStatus.Completed)).ReturnsAsync(new List<Build> { lastBuildOne, lastBuildTwo });

            var provider = new LastBuildsStatusProvider(buildsProviderMock.Object);

            var result = await provider.DetermineBuildStatus();

            result.AggregatedBuildResult.Should().Be(AggregatedBuildResult.Failed);
        }

        [Fact]
        public async void GivenOneOfLastBuildsIsPartiallySuccessful_WhenDeterminingBuildStatus_AggregatedResultIsPartiallySucceeded()
        {
            var buildsProviderMock = new Mock<IProvideBuilds>();
            var inProgressBuilds = new List<Build>();
            var lastBuildOne = new Build {Result = BuildResult.PartiallySucceeded, Status = BuildStatus.Completed };
            var lastBuildTwo = new Build {Result = BuildResult.Succeeded, Status = BuildStatus.Completed };

            buildsProviderMock.Setup(b => b.GetWithStatus(BuildStatus.InProgress)).ReturnsAsync(inProgressBuilds);
            buildsProviderMock.Setup(b => b.GetWithStatus(BuildStatus.Completed)).ReturnsAsync(new List<Build> { lastBuildOne, lastBuildTwo });

            var provider = new LastBuildsStatusProvider(buildsProviderMock.Object);

            var result = await provider.DetermineBuildStatus();

            result.AggregatedBuildResult.Should().Be(AggregatedBuildResult.PartiallySucceeded);
        }

        [Fact]
        public async void GivenAllLastBuildsAreSuccessful_WhenDeterminingBuildStatus_AggregatedResultIsSucceeded()
        {
            var buildsProviderMock = new Mock<IProvideBuilds>();
            var inProgressBuilds = new List<Build>();
            var lastBuildOne = new Build { Result = BuildResult.Succeeded, Status = BuildStatus.Completed };
            var lastBuildTwo = new Build { Result = BuildResult.Succeeded, Status = BuildStatus.Completed };

            buildsProviderMock.Setup(b => b.GetWithStatus(BuildStatus.InProgress)).ReturnsAsync(inProgressBuilds);
            buildsProviderMock.Setup(b => b.GetWithStatus(BuildStatus.Completed)).ReturnsAsync(new List<Build> { lastBuildOne, lastBuildTwo });

            var provider = new LastBuildsStatusProvider(buildsProviderMock.Object);

            var result = await provider.DetermineBuildStatus();

            result.AggregatedBuildResult.Should().Be(AggregatedBuildResult.Succeeded);
        }
    }
}
