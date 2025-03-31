// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authentication.Negotiate;
using SimpleIdServer.IdServer.Builders;
using SimpleIdServer.IdServer.Domains;
using System.Collections.Generic;
using System.Security.Claims;

namespace IdserverWindowsauth;

public class Config
{
    private static AuthenticationSchemeProviderDefinition Negotiate = AuthenticationSchemeProviderDefinitionBuilder.Create("negotiate", "Negotiate", typeof(NegotiateHandler), typeof(NegotiateOptionsLite)).Build();

    public static List<AuthenticationSchemeProvider> AuthenticationSchemes => new List<AuthenticationSchemeProvider>
    {
        AuthenticationSchemeProviderBuilder.Create(Negotiate, "Negotiate", "Negotiate", "Negotiate").SetSubject(ClaimTypes.Name).Build()
    };

    public static List<AuthenticationSchemeProviderDefinition> AuthenticationSchemeDefinitions => new List<AuthenticationSchemeProviderDefinition>
    {
        Negotiate
    };
}
