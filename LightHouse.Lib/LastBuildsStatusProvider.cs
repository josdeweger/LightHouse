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
            var lastBuildStatus = new LastBuildsStatus();

            var builds = await _buildsProvider.GetAllAsync();

            if (!builds.Any())
                return new LastBuildsStatus{AggregatedBuildStatus = AggregatedBuildStatus.None};

            lastBuildStatus.LastBuilds = builds.ToList();

            lastBuildStatus.AggregatedBuildStatus = DetermineAggregatedBuildStatus(lastBuildStatus.LastBuilds);
            lastBuildStatus.AggregatedBuildResult = DetermineAggregatedBuildResult(lastBuildStatus.LastBuilds);

            return lastBuildStatus;
        }

        private AggregatedBuildStatus DetermineAggregatedBuildStatus(List<Build> lastBuilds)
        {
            //var randomBit = _random.Next(0, 2);
            //lastBuilds[_random.Next(lastBuilds.Count)].Status = randomBit == 1 ? BuildStatus.Completed : BuildStatus.InProgress;

            if (lastBuilds.Any(b => b.Status.Equals(BuildStatus.InProgress)))
            {
                return AggregatedBuildStatus.InProgress;
            }

            return AggregatedBuildStatus.Completed;
        }

        private AggregatedBuildResult DetermineAggregatedBuildResult(List<Build> lastBuilds)
        {
            //lastBuilds[_random.Next(lastBuilds.Count)].Result = GetRandomEnumValue<BuildResult>();

            if (lastBuilds.Any(b => b.Result.Equals(BuildResult.Failed)))
            {
                return AggregatedBuildResult.Failed;
            }

            if (lastBuilds.Any(b =>
                b.Result.Equals(BuildResult.PartiallySucceeded)))
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