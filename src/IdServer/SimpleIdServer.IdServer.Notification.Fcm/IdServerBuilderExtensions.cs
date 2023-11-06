// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using FirebaseAdmin;
using SimpleIdServer.IdServer;
using SimpleIdServer.IdServer.Api;
using SimpleIdServer.IdServer.Notification.Fcm;

namespace Microsoft.Extensions.DependencyInjection;

public static class IdServerBuilderExtensions
{
    /// <summary>
    /// Add firebase notification support.
    /// </summary>
    /// <param name="idServerBuilder"></param>
    /// <param name="appOptions"></param>
    /// <returns></returns>
    public static IdServerBuilder AddFcmNotification(this IdServerBuilder idServerBuilder)
    {
        idServerBuilder.Services.AddTransient<IUserNotificationService, FcmUserNotificationService>();
        idServerBuilder.Services.AddTransient<IAuthenticationMethodService, FcmAuthenticationService>();
        return idServerBuilder;
    }
}