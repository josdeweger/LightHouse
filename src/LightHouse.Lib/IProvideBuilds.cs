using System.Collections.Generic;
using System.Threading.Tasks;

namespace LightHouse.Lib
{
    public interface IProvideBuilds
    {
        Task<List<Build>> GetWithStatus(BuildStatus statusFilter);
    }
}
