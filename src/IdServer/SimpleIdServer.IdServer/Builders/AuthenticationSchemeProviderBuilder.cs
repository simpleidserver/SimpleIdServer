// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Config;
using SimpleIdServer.IdServer.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace SimpleIdServer.IdServer.Builders;

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
            Mappers = DefaultAuthSchemeProviderMapper.All,
            AuthSchemeProviderDefinition = definition,
            Realms = new List<Realm>
            {
                DefaultRealms.Master
            }
        });
    }

    public AuthenticationSchemeProviderBuilder SetMappers(ICollection<AuthenticationSchemeProviderMapper> mappers)
    {
        _provider.Mappers = mappers;
        return this;
    }

    public AuthenticationSchemeProviderBuilder SetSubject(string source)
    {
        _provider.Mappers = _provider.Mappers.Where(m => m.MapperType != MappingRuleTypes.SUBJECT).ToList();
        _provider.Mappers.Add(new AuthenticationSchemeProviderMapper
        {
            Id = Guid.NewGuid().ToString(),
            MapperType = MappingRuleTypes.SUBJECT,
            Name = "Subject",
            SourceClaimName = source
        });
        return this;
    }

    public AuthenticationSchemeProvider Build() => _provider;
}
