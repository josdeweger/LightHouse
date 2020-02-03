using System;
using System.Collections.Generic;
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

        public async Task<LastBuildsStatus> DetermineBuildStatus()
        {
            return new LastBuildsStatus
            {
                AggregatedBuildStatus = await DetermineAggregatedBuildStatus(),
                AggregatedBuildResult = await DetermineAggregatedBuildResult()
            };
        }

        private async Task<AggregatedBuildStatus> DetermineAggregatedBuildStatus()
        {
            //var randomBit = _random.Next(0, 2);
            //lastBuilds[_random.Next(lastBuilds.Count)].Status = randomBit == 1 ? BuildStatus.Completed : BuildStatus.InProgress;
            var inProgressBuilds = await _buildsProvider.GetWithStatus(BuildStatus.InProgress);

            return inProgressBuilds.Any() ? AggregatedBuildStatus.InProgress : AggregatedBuildStatus.Completed;
        }

        private async Task<AggregatedBuildResult> DetermineAggregatedBuildResult()
        {
            //lastBuilds[_random.Next(lastBuilds.Count)].Result = GetRandomEnumValue<BuildResult>();
            var completedBuilds = await _buildsProvider.GetWithStatus(BuildStatus.Completed);

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