using System.Collections.Generic;
using System.Threading.Tasks;

namespace LightHouse.BuildProviders.Vsts
{
    public interface IVstsClient
    {
        Task<List<BuildDefinitionsResponse>> GetCompletedBuildsAsync();
    }
}