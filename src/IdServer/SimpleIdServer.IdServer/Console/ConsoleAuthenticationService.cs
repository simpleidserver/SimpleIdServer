// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using System;

namespace SimpleIdServer.IdServer.Console;

public class ConsoleAuthenticationService : IAuthenticationMethodService
{
    public string Amr => Constants.ConsoleAmr;

    public string Name => "Console";

    public Type OptionsType => typeof(IdServerConsoleOptions);

    public AuthenticationMethodCapabilities Capabilities => AuthenticationMethodCapabilities.PUSHNOTIFICATION | AuthenticationMethodCapabilities.USERAUTHENTICATION;

    public bool IsCredentialExists(User user) => true;
}
