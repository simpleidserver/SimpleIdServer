// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SimpleIdServer.IdServer.Exceptions;
using SimpleIdServer.IdServer.ExternalEvents;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Api.DeviceAuthorization
{
    public class DeviceAuthorizationController : BaseController
    {
        private readonly IDeviceAuthorizationRequestHandler _handler;
        private readonly IBusControl _busControl;
        private readonly Helpers.IUrlHelper _urlHelper;

        public DeviceAuthorizationController(
            IDeviceAuthorizationRequestHandler handler, 
            IBusControl busControl,
            Helpers.IUrlHelper urlHelper)
        {
            _handler = handler;
            _busControl = busControl;
            _urlHelper = urlHelper;
        }

        public async Task<IActionResult> Post([FromRoute] string prefix, CancellationToken token)
        {
            var jObjBody = Request.Form.ToJsonObject();
            var context = new HandlerContext(new HandlerContextRequest(_urlHelper.GetAbsoluteUriWithVirtualPath(Request), string.Empty, jObjBody, null, Request.Cookies, string.Empty), prefix ?? Constants.DefaultRealm, new HandlerContextResponse(Response.Cookies));
            context.SetUrlHelper(Url);
            using (var activity = Tracing.IdServerActivitySource.StartActivity("Get Device Authorization"))
            {
                try
                {
                    activity?.SetTag("realm", context.Realm);
                    var result = await _handler.Handle(context, token);
                    activity?.SetStatus(ActivityStatusCode.Ok, $"Device Authorization succeeded");
                    await _busControl.Publish(new DeviceAuthorizationSuccessEvent
                    {
                        ClientId = context.Client?.ClientId,
                        Realm = context.Realm,
                        RequestJSON = jObjBody.ToString(),
                    });
                    return new OkObjectResult(result);
                }
                catch (OAuthException ex)
                {
                    activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                    await _busControl.Publish(new DeviceAuthorizationFailureEvent
                    {
                        ClientId = context.Client?.ClientId,
                        Realm = context.Realm,
                        RequestJSON = jObjBody.ToString(),
                        ErrorMessage = ex.Message
                    });
                    return BuildError(ex);
                }
            }
        }
    }
}
