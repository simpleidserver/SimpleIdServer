// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using FirebaseAdmin.Messaging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SimpleIdServer.OAuth.Api;
using SimpleIdServer.OAuth.Options;
using SimpleIdServer.OpenID.Domains;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.OpenID.Api.BCAuthorize
{
    public interface IBCNotificationService
    {
        Task Notify(HandlerContext handlerContext, string authReqId, IEnumerable<BCAuthorizePermission> permissions, CancellationToken cancellationToken);
    }

    public class BCNotificationService : IBCNotificationService
    {
        private readonly OAuthHostOptions _options;

        public BCNotificationService(IOptions<OAuthHostOptions> options)
        {
            _options = options.Value;
        }

        public async Task Notify(HandlerContext handlerContext, string authReqId, IEnumerable<BCAuthorizePermission> permissions, CancellationToken cancellationToken)
        {
            var deviceRegistrationToken = handlerContext.User.DeviceRegistrationToken;
            await FirebaseMessaging.DefaultInstance.SendAsync(new Message
            {   
                Token = deviceRegistrationToken,
                Android = new AndroidConfig
                {
                    Data = new Dictionary<string, string>
                    {
                        { "title", _options.GetFcmTitle()},
                        { "body", _options.GetFcmBody() },
                        { "authReqId", authReqId },
                        { "permissions", JsonConvert.SerializeObject(permissions) }
                    }
                }
            }, cancellationToken);
        }
    }
}
