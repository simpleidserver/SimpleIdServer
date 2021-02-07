// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using SimpleIdServer.OAuth.Api.Register.Handlers;
using SimpleIdServer.OAuth.DTOs;
using SimpleIdServer.OAuth.Exceptions;
using SimpleIdServer.OAuth.Extensions;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OAuth.Api.Register
{
    [Route(Constants.EndPoints.Registration)]
    public class RegistrationController : Controller
    {
        private readonly IAddOAuthClientHandler _addOAuthClientHandler;
        private readonly IGetOAuthClientHandler _getOAuthClientHandler;

        public RegistrationController(
            IAddOAuthClientHandler registerRequestHandler,
            IGetOAuthClientHandler getOAuthClientHandler)
        {
            _addOAuthClientHandler = registerRequestHandler;
            _getOAuthClientHandler = getOAuthClientHandler;
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] JObject jObj, CancellationToken token)
        {
            try
            {
                var context = new HandlerContext(new HandlerContextRequest(Request.GetAbsoluteUriWithVirtualPath(), string.Empty, jObj, null));
                var result = await _addOAuthClientHandler.Handle(context, token);
                return new ContentResult
                {
                    Content = result.ToString(),
                    ContentType = "application/json",
                    StatusCode = (int)HttpStatusCode.Created
                };
            }
            catch(OAuthException ex)
            {
                var res = new JObject
                {
                    { ErrorResponseParameters.Error, ex.Code },
                    { ErrorResponseParameters.ErrorDescription, ex.Message }
                };
                return new BadRequestObjectResult(res);
            }
        }

        [HttpGet("{clientId}")]
        public async Task<IActionResult> Get(string clientId, CancellationToken cancellationToken)
        {
            try
            {
                var jObjHeader = Request.Headers.ToJObject();
                var context = new HandlerContext(new HandlerContextRequest(Request.GetAbsoluteUriWithVirtualPath(), null, null, jObjHeader));
                var result = await _getOAuthClientHandler.Handle(clientId, context, cancellationToken);
                return new OkObjectResult(result);
            }
            catch (OAuthUnauthorizedException ex)
            {
                var res = new JObject
                {
                    { ErrorResponseParameters.Error, ex.Code },
                    { ErrorResponseParameters.ErrorDescription, ex.Message }
                };
                return new ContentResult
                {
                    Content = res.ToString(),
                    ContentType = "application/json",
                    StatusCode = (int)HttpStatusCode.Unauthorized
                };
            }

        }
    }
}