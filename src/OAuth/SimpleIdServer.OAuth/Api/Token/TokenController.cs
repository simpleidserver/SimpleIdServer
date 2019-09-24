// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using SimpleIdServer.OAuth.DTOs;
using SimpleIdServer.OAuth.Exceptions;
using SimpleIdServer.OAuth.Extensions;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SimpleIdServer.OAuth.Api.Token
{
    [Route(Constants.EndPoints.Token)]
    public class TokenController : Controller
    {
        private readonly ITokenRequestHandler _tokenRequestHandler;
        private readonly IRevokeTokenRequestHandler _revokeTokenRequestHandler;

        public TokenController(ITokenRequestHandler tokenRequestHandler, IRevokeTokenRequestHandler revokeTokenRequestHandler)
        {
            _tokenRequestHandler = tokenRequestHandler;
            _revokeTokenRequestHandler = revokeTokenRequestHandler;
        }

        [HttpPost]
        public async Task<IActionResult> Post()
        {
            try
            {
                var claimName = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                var claimAuthTime = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.AuthenticationInstant);
                var userSubject = claimName == null ? string.Empty : claimName.Value;
                DateTime? authTime = null;
                DateTime auth;
                if (claimAuthTime != null && !string.IsNullOrWhiteSpace(claimAuthTime.Value) && DateTime.TryParse(claimAuthTime.Value, out auth))
                {
                    authTime = auth;
                }

                var jObjHeader = Request.Headers.ToJObject();
                var jObjBody = Request.Form.ToJObject();
                var context = new HandlerContext(new HandlerContextRequest(Request.GetAbsoluteUriWithVirtualPath(), userSubject, authTime, jObjBody, jObjHeader));
                var tokenResponse = await _tokenRequestHandler.Handle(context);
                return new OkObjectResult(tokenResponse);
            }
            catch(OAuthException ex)
            {
                var jObj = new JObject
                {
                    { ErrorResponseParameters.Error, ex.Code },
                    { ErrorResponseParameters.ErrorDescription, ex.Message }
                };
                return new BadRequestObjectResult(jObj);
            }
        }

        [HttpPost("revoke")]
        public async Task<IActionResult> Revoke()
        {
            try
            {
                var jObjHeader = Request.Headers.ToJObject();
                var jObjBody = Request.Form.ToJObject();
                await _revokeTokenRequestHandler.Handle(jObjHeader, jObjBody, Request.GetAbsoluteUriWithVirtualPath());
                return new OkResult();
            }
            catch (OAuthException ex)
            {
                var jObj = new JObject
                {
                    { ErrorResponseParameters.Error, ex.Code },
                    { ErrorResponseParameters.ErrorDescription, ex.Message }
                };
                return new BadRequestObjectResult(jObj);
            }
        }
    }
}