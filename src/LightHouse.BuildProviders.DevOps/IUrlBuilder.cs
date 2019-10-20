using System.Collections.Generic;

namespace LightHouse.BuildProviders.DevOps
{
    public interface IUrlBuilder
    {
        List<string> Build(string instance, string collection, IEnumerable<string> teamProjects);
    }
}