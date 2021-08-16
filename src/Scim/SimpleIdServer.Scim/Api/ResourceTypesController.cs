// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using SimpleIdServer.Scim.Domain;
using SimpleIdServer.Scim.Extensions;
using SimpleIdServer.Scim.Persistence;
using SimpleIdServer.Scim.Resources;
using System;
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
        private readonly IServiceProvider _serviceProvider;

        public ResourceTypesController(
            ISCIMSchemaQueryRepository scimSchemaQueryRepository, 
            ILogger<ResourceTypesController> logger,
            IServiceProvider serviceProvider)
        {
            _scimSchemaQueryRepository = scimSchemaQueryRepository;
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        [HttpGet]
        public async virtual Task<IActionResult> Get()
        {
            _logger.LogInformation(Global.StartGetResourceTypes);
            var result = await _scimSchemaQueryRepository.GetAllRoot();
            var controllerEndpoints = ExtractControllerEndpoints();
            return new OkObjectResult(new JArray(result.Select(s => ToDto(s, controllerEndpoints))));
        }

        protected Dictionary<string, string> ExtractControllerEndpoints()
        {
            var dic = new Dictionary<string, string>();
            var controllers = _serviceProvider.GetService(typeof(IEnumerable<BaseApiController>)) as IEnumerable<BaseApiController>;
            foreach(var controller in controllers)
            {
                var type = controller.GetType();
                var routeAttrs = type.GetCustomAttributes(typeof(RouteAttribute), true);
                var name = type.Name.Replace("Controller", string.Empty);
                if (routeAttrs.Any())
                {
                    name = (routeAttrs.First() as RouteAttribute).Template;
                }

                var relativePath = $"{Request.GetRelativePath()}/{name}";
                dic.Add(controller.ResourceType, relativePath);
            }

            return dic;
        }

        protected JObject ToDto(SCIMSchema schema, Dictionary<string, string> controllerEndpoints)
        {
            var location = $"{Request.GetAbsoluteUriWithVirtualPath()}/{SCIMConstants.SCIMEndpoints.ResourceType}/{schema.ResourceType}";
            var endpoint = string.Empty;
            if (controllerEndpoints.ContainsKey(schema.ResourceType))
            {
                endpoint = controllerEndpoints[schema.ResourceType];
            }

            return new JObject
            {
                { SCIMConstants.ResourceTypeAttribute.Schemas, new JArray(new List<string>  { SCIMConstants.StandardSchemas.ResourceTypeSchema.Id }) },
                { SCIMConstants.ResourceTypeAttribute.Id, schema.ResourceType },
                { SCIMConstants.ResourceTypeAttribute.Name, schema.Name },
                { SCIMConstants.ResourceTypeAttribute.Description, schema.Description },
                { SCIMConstants.ResourceTypeAttribute.Endpoint, endpoint },
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