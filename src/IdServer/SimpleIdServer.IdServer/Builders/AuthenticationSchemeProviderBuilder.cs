// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.IdentityModel.JsonWebTokens;
using SimpleIdServer.IdServer.Domains;
using System;
using System.Collections.Generic;

namespace SimpleIdServer.IdServer.Builders
{
    public class AuthenticationSchemeProviderBuilder
    {
        private AuthenticationSchemeProvider _provider;

        private AuthenticationSchemeProviderBuilder(AuthenticationSchemeProvider provider) 
        {
            _provider = provider;
        }

        public static AuthenticationSchemeProviderBuilder Create(AuthenticationSchemeProviderDefinition definition, string name, string displayName, string description)
        {
            return new AuthenticationSchemeProviderBuilder(new AuthenticationSchemeProvider
            {
                Id = Guid.NewGuid().ToString(),
                Name = name,
                DisplayName = displayName,
                Description = description,
                CreateDateTime = DateTime.UtcNow,
                UpdateDateTime = DateTime.UtcNow,
                Mappers = Constants.GetDefaultIdProviderMappers(),
                AuthSchemeProviderDefinition = definition,
                Realms = new List<Domains.Realm>
                {
                    Constants.StandardRealms.Master
                }
            });
        }

        public AuthenticationSchemeProviderBuilder SetMappers(ICollection<AuthenticationSchemeProviderMapper> mappers)
        {
            _provider.Mappers = mappers;
            return this;
        }

        public AuthenticationSchemeProvider Build() => _provider;
    }
}
