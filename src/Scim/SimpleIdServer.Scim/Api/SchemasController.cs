// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using SimpleIdServer.Scim.Extensions;
using SimpleIdServer.Scim.Persistence;
using SimpleIdServer.Scim.Resources;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace SimpleIdServer.Scim.Api
{
    [Route(SCIMConstants.SCIMEndpoints.Schemas)]
    public class SchemasController : Controller
    {
        private readonly ISCIMSchemaQueryRepository _scimSchemaQueryRepository;
        private readonly ILogger _logger;

        public SchemasController(ISCIMSchemaQueryRepository scimSchemaQueryRepository, ILogger<SchemasController> logger)
        {
            _scimSchemaQueryRepository = scimSchemaQueryRepository;
            _logger = logger;
        }

        [HttpGet]
        public async virtual Task<IActionResult> Get()
        {
            _logger.LogInformation(Global.StartGetSchemas);
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
        public async virtual Task<IActionResult> Get(string id)
        {
            var schema = await _scimSchemaQueryRepository.FindSCIMSchemaById(id);
            if (schema == null)
            {
                _logger.LogError(string.Format(Global.SchemaNotFound, id));
                return this.BuildError(HttpStatusCode.NotFound, string.Format(Global.SchemaNotFound, id));
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
