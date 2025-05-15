// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Infastructures;
using SimpleIdServer.IdServer.Options;

namespace SimpleIdServer.IdServer.Notification.Gotify;

public static class WebApplicationExtensions
{
    public static WebApplication UseGotifyNotification(this WebApplication webApplication)
    {
        var opts = webApplication.Services.GetRequiredService<IOptions<IdServerHostOptions>>().Value;
        var usePrefix = opts.RealmEnabled;

        webApplication.SidMapControllerRoute("createGotifyConnection",
            pattern: (usePrefix ? "{prefix}/" : string.Empty) + Constants.EndPoints.Connections,
            defaults: new { controller = "GotifyConnections", action = "Add" });

        return webApplication;
    }
}
