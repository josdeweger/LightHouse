using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Flurl.Http;
using LightHouse.Lib;
using Serilog;

namespace LightHouse.BuildProviders.DevOps
{
    public class TfsClient : IProvideBuilds
    {
        private readonly List<string> _urls;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly string _accessToken;
        private readonly List<long> _excludedBuildDefinitionIds;

        public TfsClient(ILogger logger,
            IMapper mapper,
            IUrlBuilder urlBuilder,
            string accessToken,
            string instance,
            string collection,
            List<string> teamProjects, 
            List<long> excludedBuildDefinitionIds)
        {
            _logger = logger;
            _mapper = mapper;
            _accessToken = accessToken;
            _excludedBuildDefinitionIds = excludedBuildDefinitionIds ?? new List<long>();
            _urls = urlBuilder.Build(instance, collection, teamProjects);
        }

        public async Task<List<Lib.Build>> GetWithStatus(BuildStatus statusFilter)
        {
            try
            {
                var responses = new List<BuildDefinitionsResponse>();

                foreach (var tfsUrl in _urls)
                {
                    var request = tfsUrl
                        .WithBasicAuth(_accessToken, string.Empty)
                        .AppendPathSegment("build")
                        .AppendPathSegment("builds")
                        .SetQueryParam("statusFilter", statusFilter.ToString())
                        .SetQueryParam("maxBuildsPerDefinition", 1)
                        .SetQueryParam("queryOrder", "finishTimeDescending");

                    _logger.Information($"Getting build definitions from url {request.Url}");

                    var response = await request.GetJsonAsync<BuildDefinitionsResponse>();

                    responses.Add(response);
                }

                return responses
                    .SelectMany(response => response
                        .Builds
                        .Where(bd => !_excludedBuildDefinitionIds.Contains(bd.Id))
                        .Select(_mapper.Map<Build, Lib.Build>))
                    .ToList();
            }
            catch (FlurlHttpTimeoutException)
            {
                // FlurlHttpTimeoutException derives from FlurlHttpException; catch here only
                // if you want to handle timeouts as a special case
                _logger.Error("Request timed out.");
            }
            catch (FlurlHttpException ex)
            {
                // ex.Message contains rich details, inclulding the URL, verb, response status,
                // and request and response bodies (if available)
                _logger.Error($"Calling {ex.Call.FlurlRequest.Url} returned the following error:");
                _logger.Error(ex.Message);
                _logger.Error($"Status code: {ex.Call.HttpStatus.ToString()}");
                _logger.Error($"Request Body: {ex.Call.RequestBody}");
                _logger.Error($"Response Body: {await ex.GetResponseStringAsync()}");
            }
            catch (Exception ex)
            {
                _logger.Error($"Something went wrong:");
                _logger.Error(ex.Message);
            }

            return new List<Lib.Build>();
        }
    }
}