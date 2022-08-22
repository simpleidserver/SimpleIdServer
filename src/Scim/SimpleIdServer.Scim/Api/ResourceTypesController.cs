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
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace SimpleIdServer.Scim.Api
{
    [Route(SCIMEndpoints.ResourceType)]
    public class ResourceTypesController : Controller
    {
        private readonly ISCIMSchemaQueryRepository _scimSchemaQueryRepository;
        private readonly ILogger _logger;
        private readonly IResourceTypeResolver _resourceTypeResolver;
        private readonly IUriProvider _uriProvider;

        public ResourceTypesController(
            ISCIMSchemaQueryRepository scimSchemaQueryRepository, 
            ILogger<ResourceTypesController> logger,
            IResourceTypeResolver resourceTypeResolver,
            IUriProvider uriProvider)
        {
            _scimSchemaQueryRepository = scimSchemaQueryRepository;
            _logger = logger;
            _resourceTypeResolver = resourceTypeResolver;
            _uriProvider = uriProvider;
        }

        /// <summary>
        /// Returns metadata about resource types.
        /// </summary>
        /// <response code="200">Metadata are found</response>
        /// <returns></returns>
        [ProducesResponseType(200)]
        [HttpGet]
        public async virtual Task<IActionResult> Get()
        {
            _logger.LogInformation(Global.StartGetResourceTypes);
            var result = await _scimSchemaQueryRepository.GetAllRoot();
            var resolutionResults = _resourceTypeResolver.ResolveAll();
            var getResult = new JObject
            {
                { StandardSCIMRepresentationAttributes.Schemas, new JArray(new [] { StandardSchemas.ListResponseSchemas.Id } ) },
                { StandardSCIMRepresentationAttributes.TotalResults, resolutionResults.Count() },
                { StandardSCIMRepresentationAttributes.ItemsPerPage, resolutionResults.Count() },
                { StandardSCIMRepresentationAttributes.StartIndex, 1 },
                { StandardSCIMRepresentationAttributes.Resources, new JArray(result.Select(s => ToDto(s, resolutionResults)))  }
            };
            return new OkObjectResult(getResult);
        }

        /// <summary>
        /// Retourn metadata of one resource type.
        /// </summary>
        /// <response code="200">Metadata is found</response>
        /// <response code="404">Resource type is not found</response>
        /// <param name="id">Unique identifier of the resource type.</param>
        /// <returns></returns>
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [HttpGet("{id}")]
        public async virtual Task<IActionResult> Get(string id)
        {
            _logger.LogInformation(string.Format(Global.StartGetResourceType, id));
            var result = await _scimSchemaQueryRepository.FindRootSCIMSchemaByName(id);
            if (result == null)
            {
                _logger.LogError(string.Format(Global.ResourceNotFound, id));
                return this.BuildError(HttpStatusCode.NotFound, string.Format(Global.ResourceNotFound, id));
            }

            var resolutionResults = _resourceTypeResolver.ResolveAll();
            return new OkObjectResult(ToDto(result, resolutionResults));
        }

        protected JObject ToDto(SCIMSchema schema, List<ResourceTypeResolutionResult> resolutionResults)
        {
            var location = $"{_uriProvider.GetAbsoluteUriWithVirtualPath()}/{SCIMEndpoints.ResourceType}/{schema.ResourceType}";
            var endpoint = string.Empty;
            var resolutionResult = resolutionResults.FirstOrDefault(r => r.ResourceType == schema.ResourceType);
            if (resolutionResult != null)
            {
                endpoint = $"{_uriProvider.GetRelativePath()}/{resolutionResult.ControllerName}";
            }

            return new JObject
            {
                { ResourceTypeAttribute.Schemas, new JArray(new List<string>  { StandardSchemas.ResourceTypeSchema.Id }) },
                { ResourceTypeAttribute.Id, schema.ResourceType },
                { ResourceTypeAttribute.Name, schema.ResourceType },
                { ResourceTypeAttribute.Description, schema.Description },
                { ResourceTypeAttribute.Endpoint, endpoint },
                { ResourceTypeAttribute.Schema, schema.Id },
                { ResourceTypeAttribute.SchemaExtensions, new JArray(schema.SchemaExtensions.Select(s => new JObject
                {
                    { ResourceTypeAttribute.Schema, s.Schema },
                    { ResourceTypeAttribute.Required, s.Required }
                })) },
                { ResourceTypeAttribute.Meta, new JObject
                {
                    { SCIMConstants.StandardSCIMMetaAttributes.Location,  location },
                    { SCIMConstants.StandardSCIMMetaAttributes.ResourceType, StandardSchemas.ResourceTypeSchema.ResourceType }
                }}
            };
        }
    }
}