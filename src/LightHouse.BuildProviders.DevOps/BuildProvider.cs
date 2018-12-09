using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using LightHouse.Lib;

namespace LightHouse.BuildProviders.DevOps
{
    public class BuildProvider : IProvideBuilds
    {
        private readonly IDevOpsClient _devOpsClient;
        private readonly IMapper _mapper;

        public BuildProvider(IDevOpsClient devOpsClient, IMapper mapper)
        {
            _devOpsClient = devOpsClient;
            _mapper = mapper;
        }

        public async Task<IEnumerable<Build>> GetAllAsync()
        {
            return (await _devOpsClient.GetCompletedBuildsAsync())
                .SelectMany(response => 
                    response.BuildDefinitions.Select(_mapper.Map<BuildDefinition, Build>));
        }
    }
}
