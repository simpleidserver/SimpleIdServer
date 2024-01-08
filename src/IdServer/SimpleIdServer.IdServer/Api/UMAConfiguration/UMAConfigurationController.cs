// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SimpleIdServer.IdServer.Api.Configuration;
using SimpleIdServer.IdServer.DTOs;
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
        public override async Task<IActionResult> Get([FromRoute] string prefix, CancellationToken cancellationToken)
        {
            var issuer = Request.GetAbsoluteUriWithVirtualPath();
            var result = await Build(prefix, cancellationToken);
            if (!string.IsNullOrWhiteSpace(prefix))
                prefix = $"{prefix}/";

            result.Add(UMAConfigurationNames.PermissionEndpoint, $"{issuer}/{prefix}{Constants.EndPoints.UMAPermissions}");
            result.Add(UMAConfigurationNames.ResourceRegistrationEndpoint, $"{issuer}/{prefix}{Constants.EndPoints.UMAResources}");
            return new OkObjectResult(result);
        }
    }
}
