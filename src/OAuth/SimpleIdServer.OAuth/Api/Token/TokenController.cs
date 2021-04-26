// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using SimpleIdServer.OAuth.DTOs;
using SimpleIdServer.OAuth.Exceptions;
using SimpleIdServer.OAuth.Extensions;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OAuth.Api.Token
{
    [Route(Constants.EndPoints.Token)]
    [Route(Constants.EndPoints.MtlsToken)]
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
        public async Task<IActionResult> Post(CancellationToken token)
        {
            var clientCertificate = await Request.HttpContext.Connection.GetClientCertificateAsync();
            var claimName = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            var userSubject = claimName == null ? string.Empty : claimName.Value;
            var jObjHeader = Request.Headers.ToJObject();
            var jObjBody = Request.Form.ToJObject();
            var context = new HandlerContext(new HandlerContextRequest(Request.GetAbsoluteUriWithVirtualPath(), userSubject, jObjBody, jObjHeader, null, clientCertificate));
            var result = await _tokenRequestHandler.Handle(context, token);
            // rfc6749 : the authorization server must include the HTTP "Cache-Control" response header field with a value of "no-store"
            // in any response containing tokens, credentials, or sensitive information.
            Response.SetNoCache();
            return result;
        }

        [HttpPost("revoke")]
        public async Task<IActionResult> Revoke(CancellationToken cancellationToken)
        {
            try
            {
                var clientCertificate = await Request.HttpContext.Connection.GetClientCertificateAsync();
                var jObjHeader = Request.Headers.ToJObject();
                var jObjBody = Request.Form.ToJObject();
                await _revokeTokenRequestHandler.Handle(jObjHeader, jObjBody, clientCertificate, Request.GetAbsoluteUriWithVirtualPath(), cancellationToken);
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