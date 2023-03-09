// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;
using SimpleIdServer.IdServer.DTOs;
using SimpleIdServer.IdServer.Extensions;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Api.Configuration
{
    /// <summary>
    /// Implementation : https://tools.ietf.org/html/draft-ietf-oauth-discovery-10
    /// </summary>
    public class OAuthConfigurationController : Controller
    {
        private readonly IOAuthConfigurationRequestHandler _configurationRequestHandler;

        public OAuthConfigurationController(IOAuthConfigurationRequestHandler configurationRequestHandler)
        {
            _configurationRequestHandler = configurationRequestHandler;
        }

        [HttpGet]
        public virtual async Task<IActionResult> Get([FromRoute] string prefix, CancellationToken token)
        {
            return new OkObjectResult(await Build(prefix, token));
        }

        protected async Task<JsonObject> Build(string prefix, CancellationToken cancellationToken)
        {
            var subUrl = string.Empty;
            var issuer = Request.GetAbsoluteUriWithVirtualPath();
            if (!string.IsNullOrWhiteSpace(prefix))
                subUrl = $"{prefix}/";
            var jObj = new JsonObject
            {
                [OAuthConfigurationNames.Issuer] = issuer,
                [OAuthConfigurationNames.AuthorizationEndpoint] = $"{issuer}/{subUrl}{Constants.EndPoints.Authorization}",
                [OAuthConfigurationNames.RegistrationEndpoint] = $"{issuer}/{subUrl}{Constants.EndPoints.Registration}",
                [OAuthConfigurationNames.TokenEndpoint] = $"{issuer}/{subUrl}{Constants.EndPoints.Token}",
                [OAuthConfigurationNames.RevocationEndpoint] = $"{issuer}/{subUrl}{Constants.EndPoints.Token}/revoke",
                [OAuthConfigurationNames.JwksUri] = $"{issuer}/{subUrl}{Constants.EndPoints.Jwks}",
                [OAuthConfigurationNames.IntrospectionEndpoint] = $"{issuer}/{subUrl}{Constants.EndPoints.TokenInfo}"
            };
            await _configurationRequestHandler.Enrich(prefix, jObj, issuer, cancellationToken);
            return jObj;
        }
    }
}