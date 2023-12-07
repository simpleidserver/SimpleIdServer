// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using System;

namespace SimpleIdServer.IdServer.Notifications;

public class ConsoleAuthenticationService : IAuthenticationMethodService
{
    public string Amr => "console";

    public string Name => "Console";

    public Type OptionsType => null;

    public AuthenticationMethodCapabilities Capabilities => AuthenticationMethodCapabilities.PUSHNOTIFICATION;

    public bool IsCredentialExists(User user) => true;
}
