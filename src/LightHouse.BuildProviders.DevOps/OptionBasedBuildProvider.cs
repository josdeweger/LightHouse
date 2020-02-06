using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LightHouse.Lib;

namespace LightHouse.BuildProviders.DevOps
{
    public class OptionBasedBuildProvider : IProvideBuilds
    {
        private readonly TfsClient _tfsClient;
        private readonly DevOpsClient _devOpsClient;

        public OptionBasedBuildProvider(TfsClient tfsClient, DevOpsClient devOpsClient)
        {
            _tfsClient = tfsClient;
            _devOpsClient = devOpsClient;
        }

        public Task<List<Lib.Build>> GetWithStatus(BuildService buildService, BuildStatus statusFilter, BuildProviderSettings buildProviderSettings)
        {
            switch (buildService)
            {
                case BuildService.Tfs:
                    return _tfsClient.GetWithStatus(statusFilter, buildProviderSettings);
                case BuildService.DevOps:
                    return _devOpsClient.GetWithStatus(statusFilter, buildProviderSettings);
                default:
                    throw new Exception($"Build service {buildService} not implemented!");
            }
        }
    }
}