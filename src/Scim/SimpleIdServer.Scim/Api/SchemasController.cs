// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using SimpleIdServer.Scim.Domains;
using SimpleIdServer.Scim.Extensions;
using SimpleIdServer.Scim.Helpers;
using SimpleIdServer.Scim.Persistence;
using SimpleIdServer.Scim.Resources;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace SimpleIdServer.Scim.Api
{
    public class SchemasController : Controller
    {
        private readonly ISCIMSchemaQueryRepository _scimSchemaQueryRepository;
        private readonly ILogger _logger;
        private readonly IUriProvider _uriProvider;

        public SchemasController(ISCIMSchemaQueryRepository scimSchemaQueryRepository, ILogger<SchemasController> logger, IUriProvider uriProvider)
        {
            _scimSchemaQueryRepository = scimSchemaQueryRepository;
            _logger = logger;
            _uriProvider = uriProvider;
        }

        /// <summary>
        /// Returns the schemas.
        /// </summary>
        /// <response code="200">Schemas are found</response>
        /// <returns></returns>
        [ProducesResponseType(200)]
        [HttpGet]
        public async virtual Task<IActionResult> GetAll()
        {
            _logger.LogInformation(Global.StartGetSchemas);
            var schemas = await _scimSchemaQueryRepository.GetAll();
            var jObj = new JObject
            {
                { StandardSCIMRepresentationAttributes.Schemas, new JArray(new [] { StandardSchemas.ListResponseSchemas.Id } ) },
                { StandardSCIMRepresentationAttributes.TotalResults, schemas.Count() },
                { StandardSCIMRepresentationAttributes.ItemsPerPage, schemas.Count()},
                { StandardSCIMRepresentationAttributes.StartIndex, 1 }
            };
            var resources = new JArray();
            foreach(var schema in schemas)
                resources.Add(schema.ToResponse(GetBaseUrl()));
            jObj.Add(StandardSCIMRepresentationAttributes.Resources, resources);
            return new ContentResult
            {
                StatusCode = (int)HttpStatusCode.OK,
                Content = jObj.ToString(),
                ContentType = SCIMConstants.STANDARD_SCIM_CONTENT_TYPE
            };
        }

        /// <summary>
        /// Returns the details of one schema.
        /// </summary>
        /// <response code="200">Schema is found</response>
        /// <response code="404">Schema is not found</response>
        /// <returns>Unique ID of the schema</returns>
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [HttpGet]
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
                Content = schema.ToResponse(GetBaseUrl()).ToString(),
                ContentType = SCIMConstants.STANDARD_SCIM_CONTENT_TYPE
            };
        }

        private string GetBaseUrl()
        {
            var baseUrl = _uriProvider.GetAbsoluteUriWithVirtualPath();
            return $"{baseUrl}/{SCIMEndpoints.Schemas}/{{0}}";
        }
    }
}
