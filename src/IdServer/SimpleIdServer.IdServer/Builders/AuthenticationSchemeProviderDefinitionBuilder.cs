// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.UI.AuthProviders;
using System;
using System.Linq;

namespace SimpleIdServer.IdServer.Builders
{
    public class AuthenticationSchemeProviderDefinitionBuilder
    {
        private AuthenticationSchemeProviderDefinition _provider;

        private AuthenticationSchemeProviderDefinitionBuilder(AuthenticationSchemeProviderDefinition provider) 
        {
            _provider = provider;
        }

        public static AuthenticationSchemeProviderDefinitionBuilder Create(string name, string description, Type handlerType, Type optionsType)
        {
            var properties = AuthenticationSchemeSerializer.SerializePropertyDefinitions(optionsType);
            return new AuthenticationSchemeProviderDefinitionBuilder(new AuthenticationSchemeProviderDefinition
            {
                Name = name,
                Description = description,
                HandlerFullQualifiedName = handlerType.AssemblyQualifiedName,
                OptionsFullQualifiedName = optionsType.AssemblyQualifiedName,
                Properties = properties.ToList()
            });
        }

        public AuthenticationSchemeProviderDefinition Build() => _provider;
    }
}
