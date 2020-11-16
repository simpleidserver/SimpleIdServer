// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using SimpleIdServer.Scim.Domain;
using SimpleIdServer.Scim.Extensions;
using SimpleIdServer.Scim.Persistence;
using SimpleIdServer.Scim.Resources;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleIdServer.Scim.Api
{
    [Route(SCIMConstants.SCIMEndpoints.ResourceType)]
    public class ResourceTypesController : Controller
    {
        private readonly ISCIMSchemaQueryRepository _scimSchemaQueryRepository;
        private readonly ILogger _logger;

        public ResourceTypesController(ISCIMSchemaQueryRepository scimSchemaQueryRepository, ILogger<ResourceTypesController> logger)
        {
            _scimSchemaQueryRepository = scimSchemaQueryRepository;
            _logger = logger;
        }

        [HttpGet]
        public async virtual Task<IActionResult> Get()
        {
            _logger.LogInformation(Global.StartGetResourceTypes);
            var result = await _scimSchemaQueryRepository.GetAllRoot();
            return new OkObjectResult(new JArray(result.Select(s => ToDto(s))));
        }

        protected JObject ToDto(SCIMSchema schema)
        {
            var location = $"{Request.GetAbsoluteUriWithVirtualPath()}/{SCIMConstants.SCIMEndpoints.ResourceType}/{schema.ResourceType}";
            return new JObject
            {
                { SCIMConstants.ResourceTypeAttribute.Schemas, new JArray(new List<string>  { SCIMConstants.StandardSchemas.ResourceTypeSchema.Id }) },
                { SCIMConstants.ResourceTypeAttribute.Id, schema.ResourceType },
                { SCIMConstants.ResourceTypeAttribute.Name, schema.Name },
                { SCIMConstants.ResourceTypeAttribute.Description, schema.Description },
                { SCIMConstants.ResourceTypeAttribute.Endpoint, $"/{schema.ResourceType}" },
                { SCIMConstants.ResourceTypeAttribute.Schema, schema.Id },
                { SCIMConstants.ResourceTypeAttribute.SchemaExtensions, new JArray(schema.SchemaExtensions.Select(s => new JObject
                {
                    { SCIMConstants.ResourceTypeAttribute.Schema, s.Schema },
                    { SCIMConstants.ResourceTypeAttribute.Required, s.Required }
                })) },
                { SCIMConstants.ResourceTypeAttribute.Meta, new JObject
                {
                    { SCIMConstants.StandardSCIMMetaAttributes.Location,  location },
                    { SCIMConstants.StandardSCIMMetaAttributes.ResourceType, schema.ResourceType }
                }}
            };
        }
    }
}