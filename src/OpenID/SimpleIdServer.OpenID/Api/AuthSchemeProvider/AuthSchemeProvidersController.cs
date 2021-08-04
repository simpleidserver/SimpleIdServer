// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using SimpleIdServer.OAuth.Api.Token.Handlers;
using SimpleIdServer.OAuth.Exceptions;
using SimpleIdServer.OpenID.Api.AuthSchemeProvider.Handlers;
using SimpleIdServer.OpenID.Exceptions;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenID.Api.AuthSchemeProvider
{
    [Route(SIDOpenIdConstants.EndPoints.AuthSchemeProviders)]
    public class AuthSchemeProvidersController : Controller
    {
        private readonly IDisableAuthSchemeProviderHandler _disableAuthSchemeProviderHandler;
        private readonly IEnableAuthSchemeProviderHandler _enableAuthSchemeProviderHandler;
        private readonly IGetAllAuthSchemeProvidersHandler _getAllAuthSchemeProvidersHandler;
        private readonly IUpdateAuthSchemeProviderOptionsHandler _updateAuthSchemeProviderOptionsHandler;
        private readonly IGetAuthSchemeProviderHandler _getAuthSchemeProviderHandler;

        public AuthSchemeProvidersController(
            IDisableAuthSchemeProviderHandler disableAuthSchemeProviderHandler,
            IEnableAuthSchemeProviderHandler enableAuthSchemeProviderHandler,
            IGetAllAuthSchemeProvidersHandler getAllAuthSchemeProvidersHandler,
            IUpdateAuthSchemeProviderOptionsHandler updateAuthSchemeProviderOptionsHandler,
            IGetAuthSchemeProviderHandler getAuthSchemeProviderHandler)
        {
            _disableAuthSchemeProviderHandler = disableAuthSchemeProviderHandler;
            _enableAuthSchemeProviderHandler = enableAuthSchemeProviderHandler;
            _getAllAuthSchemeProvidersHandler = getAllAuthSchemeProvidersHandler;
            _updateAuthSchemeProviderOptionsHandler = updateAuthSchemeProviderOptionsHandler;
            _getAuthSchemeProviderHandler = getAuthSchemeProviderHandler;
        }

        [HttpGet("{id}")]
        [Authorize("ManageAuthSchemeProviders")]
        public async Task<IActionResult> Get(string id, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _getAuthSchemeProviderHandler.Handle(id, cancellationToken);
                return new OkObjectResult(result);
            }
            catch (UnknownAuthSchemeProviderException ex)
            {
                return BaseCredentialsHandler.BuildError(HttpStatusCode.NotFound, ex.Code, ex.Message);
            }
        }

        [HttpGet("{id}/enable")]
        [Authorize("ManageAuthSchemeProviders")]
        public async Task<IActionResult> Enable(string id, CancellationToken cancellationToken)
        {
            try
            {
                await _enableAuthSchemeProviderHandler.Handle(id, cancellationToken);
                return new NoContentResult();
            }
            catch(UnknownAuthSchemeProviderException ex)
            {
                return BaseCredentialsHandler.BuildError(HttpStatusCode.NotFound, ex.Code, ex.Message);
            }
        }

        [HttpGet("{id}/disable")]
        [Authorize("ManageAuthSchemeProviders")]
        public async Task<IActionResult> Disable(string id, CancellationToken cancellationToken)
        {
            try
            {
                await _disableAuthSchemeProviderHandler.Handle(id, cancellationToken);
                return new NoContentResult();
            }
            catch(UnknownAuthSchemeProviderException ex)
            {
                return BaseCredentialsHandler.BuildError(HttpStatusCode.NotFound, ex.Code, ex.Message);
            }
        }

        [HttpGet]
        [Authorize("ManageAuthSchemeProviders")]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var result = await _getAllAuthSchemeProvidersHandler.Handle(cancellationToken);
            return new OkObjectResult(result);
        }

        [HttpPut("{id}/options")]
        [Authorize("ManageAuthSchemeProviders")]
        public async Task<IActionResult> UpdateOptions(string id, [FromBody] JObject jObj, CancellationToken cancellationToken)
        {
            try
            {
                await _updateAuthSchemeProviderOptionsHandler.Handle(id, jObj, cancellationToken);
                return new NoContentResult();
            }
            catch (UnknownAuthSchemeProviderException ex)
            {
                return BaseCredentialsHandler.BuildError(HttpStatusCode.NotFound, ex.Code, ex.Message);
            }
            catch (OAuthException ex)
            {
                return BaseCredentialsHandler.BuildError(HttpStatusCode.BadRequest, ex.Code, ex.Message);
            }
        }
    }
}
