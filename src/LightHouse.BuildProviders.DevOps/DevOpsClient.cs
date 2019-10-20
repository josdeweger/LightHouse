using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Flurl.Http;
using LightHouse.Lib;
using Serilog;

namespace LightHouse.BuildProviders.DevOps
{
    public class DevOpsClient : IProvideBuilds
    {
        private readonly List<string> _urls;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly string _accessToken;

        public DevOpsClient(
            ILogger logger, 
            IMapper mapper, 
            IUrlBuilder urlBuilder,
            string accessToken, 
            string instance, 
            string collection, 
            List<string> teamProjects)
        {
            _logger = logger;
            _mapper = mapper;
            _accessToken = accessToken;
            _urls = urlBuilder.Build(instance, collection, teamProjects);
        }

        public async Task<List<Build>> GetAllBuilds()
        {
            try
            {
                var responses = new List<BuildDefinitionsResponse>();

                foreach (var vstsUrl in _urls)
                {
                    var request = vstsUrl
                        .WithBasicAuth(_accessToken, string.Empty)
                        .AppendPathSegment("build")
                        .AppendPathSegment("Definitions")
                        .SetQueryParam("includeLatestBuilds", true);

                    _logger.Information($"Getting build definitions from url {request.Url}");

                    var response = await request.GetJsonAsync<BuildDefinitionsResponse>();

                    responses.Add(response);
                }

                return responses
                    .SelectMany(response => response.BuildDefinitions.Select(_mapper.Map<BuildDefinition, Build>))
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

            return new List<Build>();
        }
    }
}