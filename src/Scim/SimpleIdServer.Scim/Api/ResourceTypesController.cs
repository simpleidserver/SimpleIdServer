// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using SimpleIdServer.Scim.Domains;
using SimpleIdServer.Scim.Extensions;
using SimpleIdServer.Scim.Persistence;
using SimpleIdServer.Scim.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleIdServer.Scim.Api
{
    [Route(SCIMEndpoints.ResourceType)]
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
            var location = $"{Request.GetAbsoluteUriWithVirtualPath()}/{SCIMEndpoints.ResourceType}/{schema.ResourceType}";
            var endpoint = string.Empty;
            if (controllerEndpoints.ContainsKey(schema.ResourceType))
            {
                endpoint = controllerEndpoints[schema.ResourceType];
            }

            return new JObject
            {
                { ResourceTypeAttribute.Schemas, new JArray(new List<string>  { StandardSchemas.ResourceTypeSchema.Id }) },
                { ResourceTypeAttribute.Id, schema.ResourceType },
                { ResourceTypeAttribute.Name, schema.Name },
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
                    { SCIMConstants.StandardSCIMMetaAttributes.ResourceType, schema.ResourceType }
                }}
            };
        }
    }
}