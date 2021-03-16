// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using SimpleIdServer.OAuth.Api;
using SimpleIdServer.OAuth.Extensions;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenID.Api.BCDeviceRegistration
{
    [Route(SIDOpenIdConstants.EndPoints.BCDeviceRegistration)]
    public class BCDeviceRegistrationController : Controller
    {
        private readonly IBCDeviceRegistrationHandler _bcDeviceRegistrationHandler;

        public BCDeviceRegistrationController(IBCDeviceRegistrationHandler bcDeviceRegistrationHandler)
        {
            _bcDeviceRegistrationHandler = bcDeviceRegistrationHandler;
        }

        public Task<IActionResult> Add([FromBody] JObject jObjBody, CancellationToken cancellationToken)
        {
            var jObjHeader = Request.Headers.ToJObject();
            var context = new HandlerContext(new HandlerContextRequest(Request.GetAbsoluteUriWithVirtualPath(), string.Empty, jObjBody, jObjHeader, null, null));
            return _bcDeviceRegistrationHandler.Handle(context, cancellationToken);
        }
    }
}
