﻿// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.VerifiablePresentation;

public class VpAuthenticationMethodService : IAuthenticationMethodService
{
    public string Amr => Constants.AMR;

    public string Name => "Vp";

    public Type? OptionsType => typeof(IdServerVpOptions);

    public AuthenticationMethodCapabilities Capabilities => AuthenticationMethodCapabilities.USERREGISTRATION | AuthenticationMethodCapabilities.USERAUTHENTICATION;

    public bool IsCredentialExists(User user)
    {
        return true;
    }
}