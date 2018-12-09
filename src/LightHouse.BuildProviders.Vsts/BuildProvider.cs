using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using LightHouse.Lib;

namespace LightHouse.BuildProviders.Vsts
{
    public class BuildProvider : IProvideBuilds
    {
        private readonly IVstsClient _vstsClient;
        private readonly IMapper _mapper;

        public BuildProvider(IVstsClient vstsClient, IMapper mapper)
        {
            _vstsClient = vstsClient;
            _mapper = mapper;
        }

        public async Task<IEnumerable<Build>> GetAllAsync()
        {
            var responses = await _vstsClient.GetCompletedBuildsAsync();

            return responses.SelectMany(response => response.BuildDefinitions.Select(_mapper.Map<BuildDefinition, Build>));
        }
    }
}
