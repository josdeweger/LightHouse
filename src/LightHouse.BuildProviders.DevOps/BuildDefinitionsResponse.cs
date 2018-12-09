using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace LightHouse.BuildProviders.DevOps
{
    public class BuildDefinitionsResponse
    {
        [JsonProperty("value")]
        public List<BuildDefinition> BuildDefinitions { get; set; }
    }
    
    public class BuildDefinition
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("latestBuild")]
        public LatestBuild LatestBuild { get; set; }

        [JsonProperty("latestCompletedBuild")]
        public LatestBuild LatestCompletedBuild { get; set; }
    }

    public class LatestBuild
    {
        [JsonProperty("buildNumber")]
        public string BuildNumber { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("result")]
        public string Result { get; set; }

        [JsonProperty("queueTime")]
        public DateTimeOffset QueueTime { get; set; }

        [JsonProperty("startTime")]
        public DateTimeOffset StartTime { get; set; }

        [JsonProperty("finishTime")]
        public DateTimeOffset FinishTime { get; set; }

        [JsonProperty("requestedFor")]
        public AuthoredBy RequestedFor { get; set; }

        [JsonProperty("requestedBy")]
        public AuthoredBy RequestedBy { get; set; }

        [JsonProperty("lastChangedDate")]
        public DateTimeOffset LastChangedDate { get; set; }

        [JsonProperty("lastChangedBy")]
        public AuthoredBy LastChangedBy { get; set; }
    }

    public class AuthoredBy
    {
        [JsonProperty("displayName")]
        public string DisplayName { get; set; }
    }
}