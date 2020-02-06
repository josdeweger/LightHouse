using System.Threading.Tasks;

namespace LightHouse.Lib
{
    public interface IProvideLastBuildsStatus
    {
        Task<LastBuildsStatus> DetermineBuildStatus(BuildService buildService, BuildProviderSettings buildProviderSettings);
    }
}