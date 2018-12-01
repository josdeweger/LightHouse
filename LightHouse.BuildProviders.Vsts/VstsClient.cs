using System.Collections.Generic;
using System.Threading.Tasks;
using Flurl.Http;
using Serilog;

namespace LightHouse.BuildProviders.Vsts
{
    public class VstsClient : IVstsClient
    {
        private readonly List<string> _vstsUrls;
        private readonly ILogger _logger;
        private readonly string _accessToken;

        public VstsClient(ILogger logger, string accessToken, string instance, string collection, IEnumerable<string> teamProjects)
        {
            _logger = logger;
            _accessToken = accessToken;
            _vstsUrls = new List<string>();

            foreach (var teamProject in teamProjects)
            {
                _vstsUrls.Add($"https://{instance.Trim()}/{collection.Trim()}/{teamProject.Trim()}/_apis/");
            }
        }

        public async Task<List<BuildDefinitionsResponse>> GetCompletedBuildsAsync()
        {
            try
            {
                var responses = new List<BuildDefinitionsResponse>();

                foreach (var vstsUrl in _vstsUrls)
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

                return responses;
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

            return new List<BuildDefinitionsResponse>();
        }
    }
}