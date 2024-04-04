// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Notification.Fcm;

public class FcmAuthenticationService : IAuthenticationMethodService
{
    public string Amr => Constants.NotificationName;
    public string Name => "Firebase";
    public Type? OptionsType => typeof(FcmOptions);
    public AuthenticationMethodCapabilities Capabilities => AuthenticationMethodCapabilities.PUSHNOTIFICATION;
    public bool IsCredentialExists(User user)
    {
        return true;
    }
}
