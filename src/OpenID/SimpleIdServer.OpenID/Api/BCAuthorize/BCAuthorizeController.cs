// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SimpleIdServer.OAuth.Api;
using SimpleIdServer.OAuth.Extensions;
using SimpleIdServer.OpenID.Api.BCAuthorize;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenID.Api
{
    public class BCAuthorizeController : Controller
    {
        private readonly IBCAuthorizeHandler _bcAuthorizeHandler;

        public BCAuthorizeController(IBCAuthorizeHandler bcAuthorizeHandler)
        {
            _bcAuthorizeHandler = bcAuthorizeHandler;
        }

        [HttpPost]
        public async Task<IActionResult> Confirm(JsonObject jsonBody, CancellationToken cancellationToken)
        {
            var jObjHeader = Request.Headers.ToJsonObject();
            var context = new HandlerContext(new HandlerContextRequest(Request.GetAbsoluteUriWithVirtualPath(), string.Empty, jsonBody, jObjHeader, null, null));
            return await _bcAuthorizeHandler.Confirm(context, cancellationToken);
        }

        [HttpPost]
        public async Task<IActionResult> Reject(JsonObject jsonBody, CancellationToken cancellationToken)
        {
            var jObjHeader = Request.Headers.ToJsonObject();
            var context = new HandlerContext(new HandlerContextRequest(Request.GetAbsoluteUriWithVirtualPath(), string.Empty, jsonBody, jObjHeader, null, null));
            return await _bcAuthorizeHandler.Reject(context, cancellationToken);
        }
    }
}
