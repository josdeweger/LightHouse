using System.Collections.Generic;
using System.Threading.Tasks;

namespace LightHouse.BuildProviders.DevOps
{
    public interface IDevOpsClient
    {
        Task<List<BuildDefinitionsResponse>> GetCompletedBuildsAsync();
    }
}