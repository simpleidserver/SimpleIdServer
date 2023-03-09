// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SimpleIdServer.IdServer.DTOs;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.Extensions;
using System.Linq;
using System.Security.Claims;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Api.Token
{
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
        public async Task<IActionResult> Post([FromRoute] string prefix, CancellationToken token)
        {
            prefix = prefix ?? Constants.DefaultRealm;
            var clientCertificate = await Request.HttpContext.Connection.GetClientCertificateAsync();
            var claimName = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            var userSubject = claimName == null ? string.Empty : claimName.Value;
            var jObjHeader = Request.Headers.ToJsonObject();
            var jObjBody = Request.Form.ToJsonObject();
            var context = new HandlerContext(new HandlerContextRequest(Request.GetAbsoluteUriWithVirtualPath(), userSubject, jObjBody, jObjHeader, Request.Cookies, clientCertificate), prefix);
            var result = await _tokenRequestHandler.Handle(context, token);
            // rfc6749 : the authorization server must include the HTTP "Cache-Control" response header field with a value of "no-store"
            // in any response containing tokens, credentials, or sensitive information.
            Response.SetNoCache();
            return result;
        }

        [HttpPost]
        public async Task<IActionResult> Revoke([FromRoute] string prefix, CancellationToken cancellationToken)
        {
            try
            {
                prefix = prefix ?? Constants.DefaultRealm;
                var clientCertificate = await Request.HttpContext.Connection.GetClientCertificateAsync();
                var jObjHeader = Request.Headers.ToJsonObject();
                var jObjBody = Request.Form.ToJsonObject();
                await _revokeTokenRequestHandler.Handle(prefix, jObjHeader, jObjBody, clientCertificate, Request.GetAbsoluteUriWithVirtualPath(), cancellationToken);
                return new OkResult();
            }
            catch (OAuthException ex)
            {
                var jObj = new JsonObject
                {
                    [ErrorResponseParameters.Error] = ex.Code,
                    [ErrorResponseParameters.ErrorDescription] = ex.Message
                };
                return new BadRequestObjectResult(jObj);
            }
        }
    }
}