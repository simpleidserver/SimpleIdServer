// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SimpleIdServer.IdServer.DTOs;
using SimpleIdServer.IdServer.Exceptions;
using System.Linq;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Api.TokenIntrospection
{
    public class TokenIntrospectionController : Controller
    {
        private readonly ITokenIntrospectionRequestHandler _requestHandler;
        private readonly Helpers.IUrlHelper _urlHelper;

        public TokenIntrospectionController(ITokenIntrospectionRequestHandler requestHandler, Helpers.IUrlHelper urlHelper)
        {
            _requestHandler = requestHandler;
            _urlHelper = urlHelper;
        }

        [HttpPost]
        public async Task<IActionResult> Introspect([FromRoute] string prefix, CancellationToken cancellationToken)
        {
            prefix = prefix ?? Constants.DefaultRealm;
            var clientCertificate = await Request.HttpContext.Connection.GetClientCertificateAsync();
            var claimName = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier);
            var userSubject = claimName == null ? string.Empty : claimName.Value;
            var jObjHeader = Request.Headers.ToJsonObject();
            var jObjBody = Request.Form.ToJsonObject();
            try
            {
                var context = new HandlerContext(new HandlerContextRequest(_urlHelper.GetAbsoluteUriWithVirtualPath(Request), userSubject, jObjBody, jObjHeader, Request.Cookies, clientCertificate, HttpContext.Request.Method), prefix);
                return await _requestHandler.Handle(context, cancellationToken);
            }
            catch (OAuthUnauthorizedException ex)
            {
                var jObj = new JsonObject
                {
                    [ErrorResponseParameters.Error] = ex.Code,
                    [ErrorResponseParameters.ErrorDescription] = ex.Message
                };
                return new UnauthorizedObjectResult(jObj);
            }
            catch (OAuthException ex)
            {
                var o = new JsonObject
                {
                    [ErrorResponseParameters.Error] = ex.Code,
                    [ErrorResponseParameters.ErrorDescription] = ex.Message
                };
                return new BadRequestObjectResult(o);
            }
        }
    }
}
