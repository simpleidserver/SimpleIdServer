// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing.Template;
using System.Collections.Generic;

namespace SimpleIdServer.Scim.SwashbuckleV6
{
    internal class ScimApiParameterContext
    {
        public ScimApiParameterContext(
            IModelMetadataProvider metadataProvider,
            ControllerActionDescriptor actionDescriptor,
            IReadOnlyList<TemplatePart> routeParameters)
        {
            MetadataProvider = metadataProvider;
            ActionDescriptor = actionDescriptor;
            RouteParameters = routeParameters;

            Results = new List<ApiParameterDescription>();
        }

        public ControllerActionDescriptor ActionDescriptor { get; }

        public IModelMetadataProvider MetadataProvider { get; }

        public IList<ApiParameterDescription> Results { get; }

        public IReadOnlyList<TemplatePart> RouteParameters { get; }
    }
}
