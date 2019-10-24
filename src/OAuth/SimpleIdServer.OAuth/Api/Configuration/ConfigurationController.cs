// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using SimpleIdServer.OAuth.DTOs;
using SimpleIdServer.OAuth.Extensions;
using System.Threading.Tasks;

namespace SimpleIdServer.OAuth.Api.Configuration
{
    /// <summary>
    /// Implementation : https://tools.ietf.org/html/draft-ietf-oauth-discovery-10
    /// </summary>
    [Route(Constants.EndPoints.OAuthConfiguration)]
    public class ConfigurationController : Controller
    {
        private readonly IConfigurationRequestHandler _configurationRequestHandler;

        public ConfigurationController(IConfigurationRequestHandler configurationRequestHandler)
        {
            _configurationRequestHandler = configurationRequestHandler;
        }

        [HttpGet]
        public virtual async Task<IActionResult> Get()
        {
            return new OkObjectResult(await Build());
        }

        protected async Task<JObject> Build()
        {
            var issuer = Request.GetAbsoluteUriWithVirtualPath();
            var jObj = new JObject
            {
                { OAuthConfigurationNames.Issuer, issuer },
                { OAuthConfigurationNames.AuthorizationEndpoint, $"{issuer}/{Constants.EndPoints.Authorization}" },
                { OAuthConfigurationNames.RegistrationEndpoint, $"{issuer}/{Constants.EndPoints.Registration}" },
                { OAuthConfigurationNames.TokenEndpoint, $"{issuer}/{Constants.EndPoints.Token}" },
                { OAuthConfigurationNames.RevocationEndpoint, $"{issuer}/{Constants.EndPoints.Token}/revoke" },
                { OAuthConfigurationNames.JwksUri, $"{issuer}/{Constants.EndPoints.Jwks}" }
            };
            await _configurationRequestHandler.Enrich(jObj, issuer);
            return jObj;
        }
    }
}