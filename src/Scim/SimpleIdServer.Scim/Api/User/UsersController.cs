using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using SimpleIdServer.Scim.Commands;
using SimpleIdServer.Scim.Commands.Handlers;
using SimpleIdServer.Scim.Domain;
using SimpleIdServer.Scim.Exceptions;
using SimpleIdServer.Scim.Extensions;
using SimpleIdServer.Scim.Persistence;
using System.Net;
using System.Threading.Tasks;

namespace SimpleIdServer.Scim.Api.User
{
    [Route(SCIMConstants.SCIMEndpoints.Users)]
    public class UsersController : Controller
    {
        private readonly IAddRepresentationCommandHandler _addRepresentationCommandHandler;
        private readonly IDeleteRepresentationCommandHandler _deleteRepresentationCommandHandler;
        private readonly ISCIMRepresentationQueryRepository _scimRepresentationQueryRepository;
        private readonly SCIMHostOptions _options;

        public UsersController(IAddRepresentationCommandHandler addRepresentationCommandHandler, IDeleteRepresentationCommandHandler deleteRepresentationCommandHandler, ISCIMRepresentationQueryRepository scimRepresentationQueryRepository, IOptionsMonitor<SCIMHostOptions> options)
        {
            _addRepresentationCommandHandler = addRepresentationCommandHandler;
            _deleteRepresentationCommandHandler = deleteRepresentationCommandHandler;
            _scimRepresentationQueryRepository = scimRepresentationQueryRepository;
            _options = options.CurrentValue;
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] JObject jobj)
        {
            try
            {
                var command = new AddRepresentationCommand(SCIMConstants.SCIMEndpoints.Users, _options.UserSchemasIds, jobj);
                var scimRepresentation = await _addRepresentationCommandHandler.Handle(command);
                return BuildHTTPResult(scimRepresentation, HttpStatusCode.Created, false);
            }
            catch(SCIMBadRequestException)
            {
                return this.BuildError(HttpStatusCode.BadRequest, "Request is unparsable, syntactically incorrect, or violates schema.", "invalidSyntax");
            }
            catch(SCIMUniquenessAttributeException)
            {
                return this.BuildError(HttpStatusCode.Conflict, "One or more of the attribute values are already in use or are reserved.", "uniqueness");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var queryCollection = Request.Query;
            // startIndex
            // count
            // sortBy
            // sortOrder
            // filter=(meta.resourceType eq User) or (meta.resourceType eq Group)
            // attributes
            // excludedAttributes    
            return null;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var representation = await _scimRepresentationQueryRepository.FindSCIMRepresentationById(id, SCIMConstants.SCIMEndpoints.Users);
            if (representation == null)
            {
                return this.BuildError(HttpStatusCode.NotFound, $"Resource {id} not found.");
            }

            return BuildHTTPResult(representation, HttpStatusCode.OK, true);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                await _deleteRepresentationCommandHandler.Handle(new DeleteRepresentationCommand(id, SCIMConstants.SCIMEndpoints.Users));
                return new StatusCodeResult((int)HttpStatusCode.NoContent);
            }
            catch(SCIMNotFoundException)
            {
                return this.BuildError(HttpStatusCode.NotFound, $"Resource {id} not found.");
            }
        }

        private IActionResult BuildHTTPResult(SCIMRepresentation representation, HttpStatusCode status, bool isGetRequest)
        {
            var location = $"{Request.GetAbsoluteUriWithVirtualPath()}/{SCIMConstants.SCIMEndpoints.Users}/{representation.Id}";
            HttpContext.Response.Headers.Add("Location", location);
            HttpContext.Response.Headers.Add("ETag", representation.Version);
            return new ContentResult
            {
                StatusCode = (int)status,
                Content = representation.ToResponse(location, isGetRequest).ToString(),
                ContentType = "application/scim+json"
            };
        }
    }
}