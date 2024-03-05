// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Notification.Gotify;

public class GotifyAuthenticationService : IAuthenticationMethodService
{
    public string Amr => Constants.NotificationName;

    public string Name => "Gotify";

    public Type? OptionsType => typeof(GotifyOptions);

    public AuthenticationMethodCapabilities Capabilities => AuthenticationMethodCapabilities.PUSHNOTIFICATION;

    public bool IsRegistrationSupported => false;

    public bool IsCredentialExists(User user)
    {
        return true;
    }
}
