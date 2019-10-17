using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using SimpleIdServer.Scim.Extensions;
using SimpleIdServer.Scim.Persistence;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace SimpleIdServer.Scim.Api
{
    [Route(SCIMConstants.SCIMEndpoints.Schemas)]
    public class SchemasController : Controller
    {
        private readonly ISCIMSchemaQueryRepository _scimSchemaQueryRepository;

        public SchemasController(ISCIMSchemaQueryRepository scimSchemaQueryRepository)
        {
            _scimSchemaQueryRepository = scimSchemaQueryRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var schemas = await _scimSchemaQueryRepository.GetAll();
            var jObj = new JObject
            {
                { SCIMConstants.StandardSCIMRepresentationAttributes.Schemas, new JArray(new [] { SCIMConstants.StandardSchemas.ListResponseSchemas.Id } ) },
                { SCIMConstants.StandardSCIMRepresentationAttributes.TotalResults, schemas.Count() },
                { SCIMConstants.StandardSCIMRepresentationAttributes.ItemsPerPage, schemas.Count()},
                { SCIMConstants.StandardSCIMRepresentationAttributes.StartIndex, 0 }
            };
            var resources = new JArray();
            foreach(var schema in schemas)
            {
                resources.Add(schema.ToResponse());
            }

            jObj.Add(SCIMConstants.StandardSCIMRepresentationAttributes.Resources, resources);
            return new ContentResult
            {
                StatusCode = (int)HttpStatusCode.OK,
                Content = jObj.ToString(),
                ContentType = SCIMConstants.STANDARD_SCIM_CONTENT_TYPE
            };
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var schema = await _scimSchemaQueryRepository.FindSCIMSchemaById(id);
            if (schema == null)
            {
                return this.BuildError(HttpStatusCode.NotFound, $"Schema {id} not found.");
            }

            return new ContentResult
            {
                StatusCode = (int)HttpStatusCode.OK,
                Content = schema.ToResponse().ToString(),
                ContentType = SCIMConstants.STANDARD_SCIM_CONTENT_TYPE
            };
        }
    }
}
