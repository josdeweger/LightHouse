using System.Collections.Generic;
using Newtonsoft.Json;

namespace LightHouse.BuildProviders.DevOps
{
    public class BuildDefinitionsResponse
    {
        [JsonProperty("value")]
        public List<Build> Builds { get; set; }
    }
    
    public class Build
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("result")]
        public string Result { get; set; }

        [JsonProperty("definition")]
        public BuildDefinition BuildDefinition { get; set; }
    }

    public class BuildDefinition
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}