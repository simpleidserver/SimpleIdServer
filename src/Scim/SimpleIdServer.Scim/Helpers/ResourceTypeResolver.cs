// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;
using SimpleIdServer.Scim.Api;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleIdServer.Scim.Helpers
{
    public class ResourceTypeResolver : IResourceTypeResolver
    {
        private List<ResourceTypeResolutionResult> _resolutionResults;
        private readonly IServiceProvider _serviceProvider;

        public ResourceTypeResolver(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public List<ResourceTypeResolutionResult> ResolveAll()
        {
            if(_resolutionResults != null)
            {
                return _resolutionResults;
            }

            _resolutionResults = new List<ResourceTypeResolutionResult>();
            var controllers = _serviceProvider.GetService(typeof(IEnumerable<BaseApiController>)) as IEnumerable<BaseApiController>;
            foreach (var controller in controllers)
            {
                var type = controller.GetType();
                var routeAttrs = type.GetCustomAttributes(typeof(RouteAttribute), true);
                var name = type.Name.Replace("Controller", string.Empty);
                if (routeAttrs.Any())
                {
                    name = (routeAttrs.First() as RouteAttribute).Template;
                }

                _resolutionResults.Add(new ResourceTypeResolutionResult { ControllerName = name, ResourceType = controller.ResourceType });
            }

            return _resolutionResults;
        }

        public ResourceTypeResolutionResult ResolveByResourceType(string resourceType)
        {
            var result = ResolveAll();
            return result.FirstOrDefault(r => r.ResourceType == resourceType);
        }
    }
}
