using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using SimpleIdServer.Scim.Commands;
using SimpleIdServer.Scim.Commands.Handlers;
using SimpleIdServer.Scim.Domain;
using SimpleIdServer.Scim.Exceptions;
using SimpleIdServer.Scim.Extensions;
using System.Net;
using System.Threading.Tasks;

namespace SimpleIdServer.Scim.Api.User
{
    [Route(SCIMConstants.SCIMEndpoints.Users)]
    public class UsersController : Controller
    {
        private readonly IAddRepresentationCommandHandler _addRepresentationCommandHandler;
        private readonly SCIMHostOptions _options;

        public UsersController(IAddRepresentationCommandHandler addRepresentationCommandHandler, IOptionsMonitor<SCIMHostOptions> options)
        {
            _addRepresentationCommandHandler = addRepresentationCommandHandler;
            _options = options.CurrentValue;
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] JObject jobj)
        {
            try
            {
                var command = new AddRepresentationCommand("Users", _options.UserSchemasIds, jobj);
                var scimRepresentation = await _addRepresentationCommandHandler.Handle(command);
                Request.GetAbsoluteUriWithVirtualPath();
                return new ContentResult
                {
                    StatusCode = (int)HttpStatusCode.Created,
                    Content = scimRepresentation.ToResponse($"{Request.GetAbsoluteUriWithVirtualPath()}/{SCIMConstants.SCIMEndpoints.Users}/{scimRepresentation.Id}", false).ToString(),
                    ContentType = "application/json"
                };
            }
            catch(SCIMBadRequestException)
            {
                return this.BuildError(HttpStatusCode.BadRequest, "invalidSyntax", "Request is unparsable, syntactically incorrect, or violates schema.");
            }
            catch(SCIMUniquenessAttributeException)
            {
                return this.BuildError(HttpStatusCode.Conflict, "uniqueness", "One or more of the attribute values are already in use or are reserved.");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            return null;
        }
    }
}