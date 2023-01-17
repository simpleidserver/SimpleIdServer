// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleIdServer.IdServer.Api.BCAuthorize
{
    public interface IBCNotificationService
    {
        Task Notify(HandlerContext handlerContext, string authReqId, IEnumerable<BCAuthorizePermission> permissions, CancellationToken cancellationToken);
    }

    public class BCNotificationService : IBCNotificationService
    {
        public Task Notify(HandlerContext handlerContext, string authReqId, IEnumerable<BCAuthorizePermission> permissions, CancellationToken cancellationToken)
        {
            // A user can have a fake authentication device.
            // poll...
            // continue...
            // Firebase messaging...
            /*
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
            */
            return Task.CompletedTask;
        }
    }
}
