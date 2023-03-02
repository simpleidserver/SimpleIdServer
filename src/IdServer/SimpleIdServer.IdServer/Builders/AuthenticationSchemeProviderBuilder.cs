// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.UI.AuthProviders;
using System;
using System.Linq;

namespace SimpleIdServer.IdServer.Builders
{
    public class AuthenticationSchemeProviderBuilder
    {
        private AuthenticationSchemeProvider _provider;

        private AuthenticationSchemeProviderBuilder(AuthenticationSchemeProvider provider) 
        {
            _provider = provider;
        }

        public static AuthenticationSchemeProviderBuilder Create<TOpts>(AuthenticationSchemeProviderDefinition definition, string name, string displayName, string description, IDynamicAuthenticationOptions<TOpts> options) where TOpts : Microsoft.AspNetCore.Authentication.AuthenticationSchemeOptions, new()
        {
            var properties = AuthenticationSchemeSerializer.SerializeProperties(options);
            return new AuthenticationSchemeProviderBuilder(new AuthenticationSchemeProvider
            {
                Name = name,
                DisplayName = displayName,
                Description = description,
                CreateDateTime = DateTime.UtcNow,
                UpdateDateTime = DateTime.UtcNow,
                Properties = properties.ToList(),
                AuthSchemeProviderDefinition = definition
            });
        }

        public AuthenticationSchemeProvider Build() => _provider;
    }
}
