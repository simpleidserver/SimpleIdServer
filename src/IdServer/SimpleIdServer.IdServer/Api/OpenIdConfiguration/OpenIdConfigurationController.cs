// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SimpleIdServer.IdServer.Api.Configuration;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Api.OpenIdConfiguration
{
    public class OpenIdConfigurationController : OAuthConfigurationController
    {
        private readonly IOpenidConfigurationRequestHandler _openidConfigurationRequestHandler;

        public OpenIdConfigurationController(
            IOAuthConfigurationRequestHandler configurationRequestHandler,
            IOpenidConfigurationRequestHandler openidConfigurationRequestHandler) : base(configurationRequestHandler)
        {
            _openidConfigurationRequestHandler = openidConfigurationRequestHandler;
        }

        /// <summary>
        /// Get OpenID Provider Metadata.
        /// </summary>
        /// <param name="prefix"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        public override async Task<IActionResult> Get([FromRoute] string prefix, CancellationToken cancellationToken)
        {
            var issuer = Request.GetAbsoluteUriWithVirtualPath();
            prefix = prefix ?? Constants.DefaultRealm;
            var result = await _openidConfigurationRequestHandler.Handle(issuer, prefix, cancellationToken);
            return new OkObjectResult(result);
        }
    }
}
