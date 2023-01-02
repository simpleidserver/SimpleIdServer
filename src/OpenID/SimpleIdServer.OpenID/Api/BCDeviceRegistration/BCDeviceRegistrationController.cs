// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SimpleIdServer.OAuth.Api;
using SimpleIdServer.OAuth.Extensions;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenID.Api.BCDeviceRegistration
{
    public class BCDeviceRegistrationController : Controller
    {
        private readonly IBCDeviceRegistrationHandler _bcDeviceRegistrationHandler;

        public BCDeviceRegistrationController(IBCDeviceRegistrationHandler bcDeviceRegistrationHandler)
        {
            _bcDeviceRegistrationHandler = bcDeviceRegistrationHandler;
        }

        public Task<IActionResult> Add(JsonObject jObjBody, CancellationToken cancellationToken)
        {
            var jObjHeader = Request.Headers.ToJsonObject();
            var context = new HandlerContext(new HandlerContextRequest(Request.GetAbsoluteUriWithVirtualPath(), string.Empty, jObjBody, jObjHeader, null, null));
            return _bcDeviceRegistrationHandler.Handle(context, cancellationToken);
        }
    }
}
