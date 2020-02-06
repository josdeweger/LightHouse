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
        private readonly BuildProviderSettings _buildProviderSettings = new BuildProviderSettings
        {
            AccessToken = "12345",
            TeamProjects = new List<string>(new[] { "Some Project" }),
            Collection = "My Collection",
            Instance = "http://MyInstance.somewhere",
            ExcludedBuildDefinitionIds = new List<long>(new[] { 1L, 2L })
        };

        [Fact]
        public async void GivenOneOfLastBuildsFailed_WhenDeterminingBuildStatus_AggregatedResultIsFailed()
        {
            var inProgressBuilds = new List<Build>();
            var completedBuilds = new List<Build>
            {
                new Build {Result = BuildResult.Failed, Status = BuildStatus.Completed},
                new Build {Result = BuildResult.Succeeded, Status = BuildStatus.Completed}
            };

            var buildsProviderMock = CreateBuildsProviderMock(inProgressBuilds, completedBuilds);

            var provider = new LastBuildsStatusProvider(buildsProviderMock.Object);

            var result = await provider.DetermineBuildStatus(BuildService.Tfs, _buildProviderSettings);

            result.AggregatedBuildResult.Should().Be(AggregatedBuildResult.Failed);
        }

        [Fact]
        public async void GivenOneOfLastBuildsIsPartiallySuccessful_WhenDeterminingBuildStatus_AggregatedResultIsPartiallySucceeded()
        {
            var inProgressBuilds = new List<Build>();
            var completedBuilds = new List<Build>
            {
                new Build {Result = BuildResult.PartiallySucceeded, Status = BuildStatus.Completed},
                new Build {Result = BuildResult.Succeeded, Status = BuildStatus.Completed}
            };

            var buildsProviderMock = CreateBuildsProviderMock(inProgressBuilds, completedBuilds);

            var provider = new LastBuildsStatusProvider(buildsProviderMock.Object);

            var result = await provider.DetermineBuildStatus(BuildService.Tfs, _buildProviderSettings);

            result.AggregatedBuildResult.Should().Be(AggregatedBuildResult.PartiallySucceeded);
        }

        [Fact]
        public async void GivenAllLastBuildsAreSuccessful_WhenDeterminingBuildStatus_AggregatedResultIsSucceeded()
        {
            var inProgressBuilds = new List<Build>();
            var completedBuilds = new List<Build>
            {
                new Build {Result = BuildResult.Succeeded, Status = BuildStatus.Completed},
                new Build {Result = BuildResult.Succeeded, Status = BuildStatus.Completed}
            };

            var buildsProviderMock = CreateBuildsProviderMock(inProgressBuilds, completedBuilds);

            var provider = new LastBuildsStatusProvider(buildsProviderMock.Object);

            var result = await provider.DetermineBuildStatus(BuildService.Tfs, _buildProviderSettings);

            result.AggregatedBuildResult.Should().Be(AggregatedBuildResult.Succeeded);
        }

        private Mock<IProvideBuilds> CreateBuildsProviderMock(List<Build> inProgressBuilds, List<Build> completedBuilds)
        {
            var buildsProviderMock = new Mock<IProvideBuilds>();

            buildsProviderMock
                .Setup(b => b.GetWithStatus(BuildService.Tfs, BuildStatus.InProgress, _buildProviderSettings))
                .ReturnsAsync(inProgressBuilds);

            buildsProviderMock
                .Setup(b => b.GetWithStatus(BuildService.Tfs, BuildStatus.Completed, _buildProviderSettings))
                .ReturnsAsync(completedBuilds);

            return buildsProviderMock;
        }
    }
}
