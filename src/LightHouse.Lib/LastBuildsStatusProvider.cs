using System;
using System.Linq;
using System.Threading.Tasks;

namespace LightHouse.Lib
{
    public class LastBuildsStatusProvider : IProvideLastBuildsStatus
    {
        private readonly IProvideBuilds _buildsProvider;
        private readonly Random _random = new Random();

        public LastBuildsStatusProvider(IProvideBuilds buildsProvider)
        {
            _buildsProvider = buildsProvider;
        }

        public async Task<LastBuildsStatus> DetermineBuildStatus(BuildService buildService, BuildProviderSettings buildProviderSettings)
        {
            return new LastBuildsStatus
            {
                AggregatedBuildStatus = await DetermineAggregatedBuildStatus(buildService, buildProviderSettings),
                AggregatedBuildResult = await DetermineAggregatedBuildResult(buildService, buildProviderSettings)
            };
        }

        private async Task<AggregatedBuildStatus> DetermineAggregatedBuildStatus(BuildService buildService, BuildProviderSettings buildProviderSettings)
        {
            //var randomBit = _random.Next(0, 2);
            //lastBuilds[_random.Next(lastBuilds.Count)].Status = randomBit == 1 ? BuildStatus.Completed : BuildStatus.InProgress;
            var inProgressBuilds = await _buildsProvider.GetWithStatus(buildService, BuildStatus.InProgress, buildProviderSettings);

            return inProgressBuilds.Any() ? AggregatedBuildStatus.InProgress : AggregatedBuildStatus.Completed;
        }

        private async Task<AggregatedBuildResult> DetermineAggregatedBuildResult(BuildService buildService, BuildProviderSettings buildProviderSettings)
        {
            //lastBuilds[_random.Next(lastBuilds.Count)].Result = GetRandomEnumValue<BuildResult>();
            var completedBuilds = await _buildsProvider.GetWithStatus(buildService, BuildStatus.Completed, buildProviderSettings);

            if (!completedBuilds.Any())
            {
                return AggregatedBuildResult.None;
            }

            if (completedBuilds.Any(b => b.Result.Equals(BuildResult.Failed)))
            {
                return AggregatedBuildResult.Failed;
            }

            if (completedBuilds.Any(b => b.Result.Equals(BuildResult.PartiallySucceeded)))
            {
                return AggregatedBuildResult.PartiallySucceeded;
            }

            return AggregatedBuildResult.Succeeded;
        }

        private T GetRandomEnumValue<T>()
        {
            var enumValues = Enum.GetValues(typeof(T));
            return (T)enumValues.GetValue(_random.Next(enumValues.Length));
        }
    }
}