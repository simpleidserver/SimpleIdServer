// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;
using SqlSugar;

namespace SimpleIdServer.IdServer.Store.SqlSugar.Models;

[SugarTable("AuthenticationSchemeProviders")]
public class SugarAuthenticationSchemeProvider
{
    [SugarColumn(IsPrimaryKey = true)]
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? DisplayName { get; set; }
    public string? Description { get; set; }
    public DateTime CreateDateTime { get; set; }
    public DateTime UpdateDateTime { get; set; }
    public string AuthSchemeProviderDefinitionName { get; set; } = null!;
    [Navigate(NavigateType.OneToMany, nameof(SugarAuthenticationSchemeProviderMapper.IdProviderId))]
    public List<SugarAuthenticationSchemeProviderMapper> Mappers { get; set; }
    [Navigate(typeof(SugarAuthenticationSchemeProviderRealm), nameof(SugarAuthenticationSchemeProviderRealm.AuthenticationSchemeProvidersId), nameof(SugarAuthenticationSchemeProviderRealm.RealmsName))]
    public List<SugarRealm> Realms { get; set; }
    public SugarAuthenticationSchemeProviderDefinition AuthSchemeProviderDefinition { get; set; }

    public AuthenticationSchemeProvider ToDomain()
    {
        return new AuthenticationSchemeProvider
        {
            Id = Id,
            Name = Name,
            DisplayName = DisplayName,
            Description = Description,
            CreateDateTime = CreateDateTime,
            UpdateDateTime = UpdateDateTime,
            AuthSchemeProviderDefinition = AuthSchemeProviderDefinition?.ToDomain(),
            Realms = Realms.Select(r => r.ToDomain()).ToList(),
            Mappers = Mappers.Select(m => m.ToDomain()).ToList()
        };
    }
}
