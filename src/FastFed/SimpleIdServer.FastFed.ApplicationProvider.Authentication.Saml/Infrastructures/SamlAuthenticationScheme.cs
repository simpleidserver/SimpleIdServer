// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using SimpleIdServer.IdServer.Saml.Sp;

namespace SimpleIdServer.FastFed.ApplicationProvider.Authentication.Saml.Infrastructures;

public class SamlAuthenticationScheme
{
    public SamlAuthenticationScheme(AuthenticationScheme authScheme)
    {
        AuthScheme = authScheme;
    }

    public SamlAuthenticationScheme(AuthenticationScheme authScheme, IOptionsMonitor<SamlSpOptions> samlSpOptions) : this(authScheme)
    {
        SamlSpOptions = samlSpOptions;
    }


    public AuthenticationScheme AuthScheme { get; set; }
    public IOptionsMonitor<SamlSpOptions> SamlSpOptions { get; set; }
}
