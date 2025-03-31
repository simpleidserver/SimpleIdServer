// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authentication.Google;
using SimpleIdServer.IdServer.Builders;
using SimpleIdServer.IdServer.Domains;
using System.Collections.Generic;

namespace IdserverIdproviders;

public class Config
{
    private static AuthenticationSchemeProviderDefinition Google = AuthenticationSchemeProviderDefinitionBuilder.Create("google", "Google", typeof(GoogleHandler), typeof(GoogleOptionsLite)).Build();

    public static List<AuthenticationSchemeProvider> AuthenticationSchemes => new List<AuthenticationSchemeProvider>
    {
        AuthenticationSchemeProviderBuilder.Create(Google, "Google", "Google", "Google").Build()
    };

    public static List<AuthenticationSchemeProviderDefinition> AuthenticationSchemeDefinitions => new List<AuthenticationSchemeProviderDefinition>
    {
        Google
    };
}
