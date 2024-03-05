// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.Extensions.Configuration;
using SimpleIdServer.IdServer.Api;
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Notification.Gotify;

public class GotifyUserNotificationService : IUserNotificationService
{
    private readonly IConfiguration _configuration

    public GotifyUserNotificationService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string Name => Constants.NotificationName;

    public Task Send(string title, string body, Dictionary<string, string> data, User user)
    {
        throw new NotImplementedException();
    }

    public Task Send(string title, string body, Dictionary<string, string> data, string destination)
    {
        throw new NotImplementedException();
    }
}
