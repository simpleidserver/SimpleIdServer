// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
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
        private IRegisterRequestHandler _registerRequestHandler;

        public RegistrationController(IRegisterRequestHandler registerRequestHandler)
        {
            _registerRequestHandler = registerRequestHandler;
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] JObject jObj, CancellationToken token)
        {
            try
            {
                var context = new HandlerContext(new HandlerContextRequest(Request.GetAbsoluteUriWithVirtualPath(), string.Empty, jObj, null));
                var result = await _registerRequestHandler.Handle(context, token);
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
    }
}