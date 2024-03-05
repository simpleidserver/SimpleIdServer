// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Api;
using SimpleIdServer.IdServer;
using SimpleIdServer.IdServer.Notification.Gotify;

namespace Microsoft.Extensions.DependencyInjection;

public static class IdServerBuilderExtensions
{
    public static IdServerBuilder AddGotifyNotification(this IdServerBuilder idServerBuilder)
    {
        idServerBuilder.Services.AddTransient<IUserNotificationService, GotifyUserNotificationService>();
        idServerBuilder.Services.AddTransient<IAuthenticationMethodService, GotifyAuthenticationService>();
        return idServerBuilder;
    }
}
