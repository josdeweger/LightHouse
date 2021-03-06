﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Flurl.Http;
using LightHouse.Lib;
using Serilog;

namespace LightHouse.BuildProviders.DevOps
{
    public class TfsClient
    {
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly IUrlBuilder _urlBuilder;

        public TfsClient(ILogger logger, IMapper mapper, IUrlBuilder urlBuilder)
        {
            _logger = logger;
            _mapper = mapper;
            _urlBuilder = urlBuilder;
        }

        public async Task<List<Lib.Build>> GetWithStatus(BuildStatus statusFilter, BuildProviderSettings buildProviderSettings)
        {
            try
            {
                var excludedBuildDefinitionIds = buildProviderSettings.ExcludedBuildDefinitionIds ?? new List<long>();
                var urls = _urlBuilder.Build(buildProviderSettings.Instance, buildProviderSettings.Collection, buildProviderSettings.TeamProjects);
                var responses = new List<BuildDefinitionsResponse>();

                foreach (var tfsUrl in urls)
                {
                    var request = tfsUrl
                        .WithBasicAuth(buildProviderSettings.AccessToken, string.Empty)
                        .AppendPathSegment("build")
                        .AppendPathSegment("builds")
                        .SetQueryParam("statusFilter", statusFilter.ToString())
                        .SetQueryParam("maxBuildsPerDefinition", 1)
                        .SetQueryParam("queryOrder", "finishTimeDescending");

                    _logger.Information($"Getting build definitions from url {request.Url}");

                    var res = await request.GetAsync();
                    var response = await request.GetJsonAsync<BuildDefinitionsResponse>();

                    responses.Add(response);
                }

                return responses
                    .SelectMany(response => response
                        .Builds
                        .Where(bd => !excludedBuildDefinitionIds.Contains(bd.Id))
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

            return new List<Lib.Build>();
        }
    }
}