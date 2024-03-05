// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Configuration;

namespace SimpleIdServer.IdServer.Notification.Gotify;

public class GotifyOptions
{
    [ConfigurationRecord("Base url", "Url of the gotify service", order: 0)]
    public string BaseUrl { get; set; } = null!;
    [ConfigurationRecord("Login", "Login of the administration account", order: 1)]
    public string AdminLogin { get; set; } = null!;
    [ConfigurationRecord("Password", "Password of the administration account", 2, null, CustomConfigurationRecordType.PASSWORD)]
    public string AdminPassword { get; set; } = null!;
}