// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authentication;
using SimpleIdServer.IdServer.Saml.Sp;

namespace SimpleIdServer.FastFed.ApplicationProvider.Authentication.Saml.Infrastructures;

public class SamlAuthenticationScheme
{
    public SamlAuthenticationScheme(AuthenticationScheme authScheme)
    {
        AuthScheme = authScheme;
    }

    public SamlAuthenticationScheme(AuthenticationScheme authScheme, SamlSpOptions samlSpOptions) : this(authScheme)
    {
        SamlSpOptions = samlSpOptions;
    }


    public AuthenticationScheme AuthScheme { get; set; }
    public SamlSpOptions SamlSpOptions { get; set; }
}
