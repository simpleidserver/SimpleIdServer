using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using SimpleIdServer.Scim.Commands;
using SimpleIdServer.Scim.Commands.Handlers;
using SimpleIdServer.Scim.Domain;
using SimpleIdServer.Scim.DTOs;
using SimpleIdServer.Scim.Exceptions;
using SimpleIdServer.Scim.Extensions;
using SimpleIdServer.Scim.Helpers;
using SimpleIdServer.Scim.Persistence;
using System.Net;
using System.Threading.Tasks;

namespace SimpleIdServer.Scim.Api
{
    public class BaseApiController : Controller
    {
        private readonly string _scimEndpoint;
        private readonly IAddRepresentationCommandHandler _addRepresentationCommandHandler;
        private readonly IDeleteRepresentationCommandHandler _deleteRepresentationCommandHandler;
        private readonly ISCIMRepresentationQueryRepository _scimRepresentationQueryRepository;
        private readonly SCIMHostOptions _options;

        public BaseApiController(string scimEndpoint, IAddRepresentationCommandHandler addRepresentationCommandHandler, IDeleteRepresentationCommandHandler deleteRepresentationCommandHandler, ISCIMRepresentationQueryRepository scimRepresentationQueryRepository, IOptionsMonitor<SCIMHostOptions> options)
        {
            _scimEndpoint = scimEndpoint;
            _addRepresentationCommandHandler = addRepresentationCommandHandler;
            _deleteRepresentationCommandHandler = deleteRepresentationCommandHandler;
            _scimRepresentationQueryRepository = scimRepresentationQueryRepository;
            _options = options.CurrentValue;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var searchRequest = SearchSCIMResourceParameter.Create(Request.Query);
            try
            { 
                var result = await _scimRepresentationQueryRepository.FindSCIMRepresentations(new SearchSCIMRepresentationsParameter(searchRequest.StartIndex, searchRequest.Count, searchRequest.SortBy, searchRequest.SortOrder, SCIMFilterParser.Parse(searchRequest.Filter)));
                var jObj = new JObject
                {
                    { SCIMConstants.StandardSCIMRepresentationAttributes.Schemas, new JArray(new [] { SCIMConstants.StandardSchemas.ListResponseSchemas.Id } ) },
                    { SCIMConstants.StandardSCIMRepresentationAttributes.TotalResults, result.TotalResults },
                    { SCIMConstants.StandardSCIMRepresentationAttributes.ItemsPerPage, searchRequest.Count },
                    { SCIMConstants.StandardSCIMRepresentationAttributes.StartIndex, searchRequest.StartIndex }
                };
                var resources = new JArray();
                foreach(var record in result.Content)
                {
                    var newJObj = new JObject();
                    SCIMRepresentationExtensions.EnrichResponse(record.Attributes, newJObj, true);
                    resources.Add(newJObj);
                }

                jObj.Add("Resources", resources);
                return new ContentResult
                {
                    StatusCode = (int)HttpStatusCode.OK,
                    Content = jObj.ToString(),
                    ContentType = "application/scim+json"
                };
            }
            catch(SCIMFilterException ex)
            {
                return this.BuildError(HttpStatusCode.BadRequest, ex.Message, "invalidFilter");
            }
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

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] JObject jobj)
        {
            try
            {
                var command = new AddRepresentationCommand(_scimEndpoint, _options.UserSchemasIds, jobj);
                var scimRepresentation = await _addRepresentationCommandHandler.Handle(command);
                return BuildHTTPResult(scimRepresentation, HttpStatusCode.Created, false);
            }
            catch (SCIMBadRequestException)
            {
                return this.BuildError(HttpStatusCode.BadRequest, "Request is unparsable, syntactically incorrect, or violates schema.", "invalidSyntax");
            }
            catch (SCIMUniquenessAttributeException)
            {
                return this.BuildError(HttpStatusCode.Conflict, "One or more of the attribute values are already in use or are reserved.", "uniqueness");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                await _deleteRepresentationCommandHandler.Handle(new DeleteRepresentationCommand(id, SCIMConstants.SCIMEndpoints.Users));
                return new StatusCodeResult((int)HttpStatusCode.NoContent);
            }
            catch (SCIMNotFoundException)
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
