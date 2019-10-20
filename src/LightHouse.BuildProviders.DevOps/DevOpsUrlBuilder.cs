using System.Collections.Generic;
using System.Linq;

namespace LightHouse.BuildProviders.DevOps
{
    public class DevOpsUrlBuilder : IUrlBuilder
    {
        public List<string> Build(string instance, string collection, IEnumerable<string> teamProjects)
        {
            return teamProjects
                .Select(teamProject =>
                {
                    string protocol = "https://";
                    if (instance.StartsWith("http://"))
                    {
                        instance = instance.Remove(0, 7);
                        protocol = "http://";
                    }
                    else if (instance.StartsWith("https://"))
                    {
                        instance = instance.Remove(0, 8);
                        protocol = "https://";
                    }

                    var encodedInstance = instance.Replace(" ", "%20");
                    var encodedCollection = collection.Replace(" ", "%20");
                    var encodedTeamProject = teamProject.Replace(" ", "%20");
                    
                    return $"{protocol}{encodedInstance}/{encodedCollection}/{encodedTeamProject}/_apis/";
                })
                .ToList();
        }
    }
}