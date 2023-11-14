// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SimpleIdServer.IdServer.DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace SimpleIdServer.IdServer.Api.Configuration
{
    public class IdServerConfigurationController : Controller
    {
        private readonly IEnumerable<IAuthenticationMethodService> _authMethodServices;
        private readonly Helpers.IUrlHelper _urlHelper;

        public IdServerConfigurationController(
            IEnumerable<IAuthenticationMethodService> authMethodServices,
            Helpers.IUrlHelper urlHelper)
        {
            _authMethodServices = authMethodServices;
            _urlHelper = urlHelper;
        }

        [HttpGet]
        public IActionResult Get([FromRoute] string prefix)
        {
            var subUrl = string.Empty;
            var issuer = _urlHelper.GetAbsoluteUriWithVirtualPath(Request);
            var issuerStr = issuer;
            if (!string.IsNullOrWhiteSpace(prefix))
            {
                issuerStr = $"{issuer}/{prefix}";
                subUrl = $"{prefix}/";
            }

            var amrs = _authMethodServices.Select(m => m.Amr);
            var jObj = new JsonObject
            {
                [IdServerConfigurationNames.Amrs] = JsonSerializer.SerializeToNode(amrs)
            };
            return new OkObjectResult(jObj);
        }
    }
}