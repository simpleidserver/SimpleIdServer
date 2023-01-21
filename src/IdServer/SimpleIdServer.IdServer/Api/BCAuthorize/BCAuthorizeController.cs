// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SimpleIdServer.IdServer.Extensions;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Api.BCAuthorize
{
    /// <summary>
    /// Backchannel Authentication Endpoit MUST utilize TLS.
    /// Recommendation : FAPI suggests to use "tls_client_auth" or "self_signed_tls_client_auth".
    /// https://openid.net/specs/openid-client-initiated-backchannel-authentication-core-1_0.html#auth_backchannel_endpoint
    /// </summary>
    public class BCAuthorizeController : Controller
    {
        private readonly IBCAuthorizeHandler _bcAuthorizeHandler;

        public BCAuthorizeController(IBCAuthorizeHandler bcAuthorizeHandler)
        {
            _bcAuthorizeHandler = bcAuthorizeHandler;
        }

        [HttpPost]
        public async Task<IActionResult> Post(CancellationToken cancellationToken)
        {
            var jObjBody = Request.Form.ToJsonObject();
            var jObjHeader = Request.Headers.ToJsonObject();
            var clientCertificate = await Request.HttpContext.Connection.GetClientCertificateAsync();
            var context = new HandlerContext(new HandlerContextRequest(Request.GetAbsoluteUriWithVirtualPath(), string.Empty, jObjBody, jObjHeader, null, clientCertificate));
            context.SetUrlHelper(Url);
            return await _bcAuthorizeHandler.Create(context, cancellationToken);
        }
    }
}
