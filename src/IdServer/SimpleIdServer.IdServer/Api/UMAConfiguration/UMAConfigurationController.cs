// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Mvc;
using SimpleIdServer.IdServer.Api.Configuration;
using SimpleIdServer.IdServer.DTOs;
using SimpleIdServer.IdServer.Extensions;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Api.UMAConfiguration
{
    public class UMAConfigurationController : OAuthConfigurationController
    {
        public UMAConfigurationController(IOAuthConfigurationRequestHandler configurationRequestHandler) : base(configurationRequestHandler)
        {
        }

        [HttpGet]
        public override async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            var issuer = Request.GetAbsoluteUriWithVirtualPath();
            var result = await Build(cancellationToken);
            result.Add(UMAConfigurationNames.PermissionEndpoint, $"{issuer}/{Constants.EndPoints.UMAPermissions}");
            result.Add(UMAConfigurationNames.ResourceRegistrationEndpoint, $"{issuer}/{Constants.EndPoints.UMAResources}");
            return new OkObjectResult(result);
        }
    }
}
