// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using System;

namespace SimpleIdServer.IdServer
{
    public interface IAuthenticationMethodService
    {
        string Amr { get; }
        string Name { get; }
        Type? OptionsType { get; }
        AuthenticationMethodCapabilities Capabilities { get; }
        bool IsCredentialExists(User user);
    }

    [Flags]
    public enum AuthenticationMethodCapabilities
    {
        USERAUTHENTICATION = 1,
        PUSHNOTIFICATION = 2,
        USERREGISTRATION = 4
    }
}