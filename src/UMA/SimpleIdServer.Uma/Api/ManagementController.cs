using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using SimpleIdServer.OAuth.Jwt;
using SimpleIdServer.Uma.Extensions;
using SimpleIdServer.Uma.Persistence;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.Uma.Api
{
    [Route(UMAConstants.EndPoints.ManagementAPI)]
    public class ManagementController : BaseAPIController
    {
        private readonly IUMAResourceRepository _umaResourceRepository;
        private readonly IUMAPendingRequestRepository _umaPendingRequestRepository;

        public ManagementController(
            IUMAResourceRepository umaResourceRepository,
            IUMAPendingRequestRepository umaPendingRequestRepository,
            IJwtParser jwtParser, 
            IOptions<UMAHostOptions> umaHostoptions): base(jwtParser, umaHostoptions)
        {
            _umaResourceRepository = umaResourceRepository;
            _umaPendingRequestRepository = umaPendingRequestRepository;
        }

        #region Resource Operations

        [HttpGet("rreguri/.search")]
        [Authorize("ManageResources")]
        public async Task<IActionResult> SearchResources(CancellationToken cancellationToken)
        {
            var searchUMAResourceParameter = new SearchUMAResourceParameter();
            EnrichSearchRequestParameter(searchUMAResourceParameter);
            var searchResult = await _umaResourceRepository.Find(searchUMAResourceParameter, cancellationToken);
            var result = new JObject
            {
                { "totalResults", searchResult.TotalResults },
                { "count", searchUMAResourceParameter.Count },
                { "startIndex", searchUMAResourceParameter.StartIndex },
                { "data", new JArray(searchResult.Records.Select(rec => ResourcesAPIController.Serialize(rec))) }
            };
            return new OkObjectResult(result);
        }

        [HttpGet("rreguri/{id}/permissions")]
        [Authorize("ManageResources")]
        public async Task<IActionResult> GetResourcePermissions(string id, CancellationToken cancellationToken)
        {
            var umaResource = await _umaResourceRepository.FindByIdentifier(id, cancellationToken);
            if (umaResource == null)
            {
                return this.BuildError(HttpStatusCode.NotFound, UMAErrorCodes.NOT_FOUND);
            }

            return new OkObjectResult(ResourcesAPIController.Serialize(umaResource.Permissions));
        }

        [HttpGet("rreguri/{id}/reqs/.search")]
        [Authorize("ManageResources")]
        public async Task<IActionResult> SearchRequestsOneGivenResource(string id, CancellationToken cancellationToken)
        {
            var searchRequestParameter = ExtractSearchRequestParameter();
            var searchResult = await _umaPendingRequestRepository.FindByResource(id, searchRequestParameter, cancellationToken);
            return new OkObjectResult(RequestsAPIController.Serialize(searchRequestParameter, searchResult));
        }

        #endregion

        #region Request Operations

        [HttpGet("reqs/.search")]
        [Authorize("ManageRequests")]
        public async Task<IActionResult> SearchRequests(CancellationToken cancellationToken)
        {
            var searchRequestParameter = ExtractSearchRequestParameter();
            var searchResult = await _umaPendingRequestRepository.Find(searchRequestParameter, cancellationToken);
            return new OkObjectResult(RequestsAPIController.Serialize(searchRequestParameter, searchResult));
        }

        #endregion
    }
}
