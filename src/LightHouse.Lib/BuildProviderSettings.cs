using System.Collections.Generic;

namespace LightHouse.Lib
{
    public class BuildProviderSettings
    {
        public string AccessToken { get; set; }
        public string Instance { get; set; }
        public string Collection { get; set; }
        public List<string> TeamProjects { get; set; }
        public List<long> ExcludedBuildDefinitionIds { get; set; }
    }
}