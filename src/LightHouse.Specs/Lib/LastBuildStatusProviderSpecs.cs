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
            var lastBuildOne = new Build { Result = BuildResult.Failed };
            var lastBuildTwo = new Build { Result = BuildResult.Succeeded };

            buildsProviderMock.Setup(b => b.GetAllBuilds()).ReturnsAsync(new List<Build> { lastBuildOne, lastBuildTwo });

            var provider = new LastBuildsStatusProvider(buildsProviderMock.Object);

            var result = await provider.DetermineBuildStatus();

            result.LastBuilds.Should().HaveCount(2);
            result.AggregatedBuildResult.Should().Be(AggregatedBuildResult.Failed);
        }

        [Fact]
        public async void GivenOneOfLastBuildsIsPartiallySuccessful_WhenDeterminingBuildStatus_AggregatedResultIsPartiallySucceeded()
        {
            var buildsProviderMock = new Mock<IProvideBuilds>();
            var lastBuildOne = new Build {Result = BuildResult.PartiallySucceeded};
            var lastBuildTwo = new Build {Result = BuildResult.Succeeded};

            buildsProviderMock.Setup(b => b.GetAllBuilds()).ReturnsAsync(new List<Build> { lastBuildOne, lastBuildTwo });

            var provider = new LastBuildsStatusProvider(buildsProviderMock.Object);

            var result = await provider.DetermineBuildStatus();

            result.LastBuilds.Should().HaveCount(2);
            result.AggregatedBuildResult.Should().Be(AggregatedBuildResult.PartiallySucceeded);
        }

        [Fact]
        public async void GivenAllLastBuildsAreSuccessful_WhenDeterminingBuildStatus_AggregatedResultIsSucceeded()
        {
            var buildsProviderMock = new Mock<IProvideBuilds>();
            var lastBuildOne = new Build { Result = BuildResult.Succeeded };
            var lastBuildTwo = new Build { Result = BuildResult.Succeeded };

            buildsProviderMock.Setup(b => b.GetAllBuilds()).ReturnsAsync(new List<Build> { lastBuildOne, lastBuildTwo });

            var provider = new LastBuildsStatusProvider(buildsProviderMock.Object);

            var result = await provider.DetermineBuildStatus();

            result.LastBuilds.Should().HaveCount(2);
            result.AggregatedBuildResult.Should().Be(AggregatedBuildResult.Succeeded);
        }
    }
}
