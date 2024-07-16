// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        /// <summary>
        /// Get authorization server metadata.
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet]
        public virtual async Task<IActionResult> Get([FromRoute] string prefix, CancellationToken token)
        {
            return new OkObjectResult(await Build(prefix, token));
        }

        protected Task<JsonObject> Build(string prefix, CancellationToken cancellationToken)
        {
            var issuer = Request.GetAbsoluteUriWithVirtualPath();
            return _configurationRequestHandler.Handle(prefix, issuer, cancellationToken);
        }
    }
}