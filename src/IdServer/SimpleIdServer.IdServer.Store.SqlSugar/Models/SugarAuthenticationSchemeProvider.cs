// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Microsoft.Extensions.Logging.Abstractions;
using SimpleIdServer.IdServer.Domains;
using SqlSugar;

namespace SimpleIdServer.IdServer.Store.SqlSugar.Models;

[SugarTable("AuthenticationSchemeProviders")]
public class SugarAuthenticationSchemeProvider
{
    [SugarColumn(IsPrimaryKey = true)]
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    [SugarColumn(IsNullable = true)]
    public string? DisplayName { get; set; }
    [SugarColumn(IsNullable = true)]
    public string? Description { get; set; }
    public DateTime CreateDateTime { get; set; }
    public DateTime UpdateDateTime { get; set; }
    public string AuthSchemeProviderDefinitionName { get; set; } = null!;
    [Navigate(NavigateType.OneToMany, nameof(SugarAuthenticationSchemeProviderMapper.IdProviderId))]
    public List<SugarAuthenticationSchemeProviderMapper> Mappers { get; set; }
    [Navigate(typeof(SugarAuthenticationSchemeProviderRealm), nameof(SugarAuthenticationSchemeProviderRealm.AuthenticationSchemeProvidersId), nameof(SugarAuthenticationSchemeProviderRealm.RealmsName))]
    public List<SugarRealm> Realms { get; set; }
    [Navigate(NavigateType.ManyToOne, nameof(AuthSchemeProviderDefinitionName))]
    public SugarAuthenticationSchemeProviderDefinition AuthSchemeProviderDefinition { get; set; }

    public static SugarAuthenticationSchemeProvider Transform(AuthenticationSchemeProvider provider)
    {
        return new SugarAuthenticationSchemeProvider
        {
            Id = provider.Id,
            Name = provider.Name,
            DisplayName = provider.DisplayName,
            Description = provider.Description,
            CreateDateTime = provider.CreateDateTime,
            UpdateDateTime = provider.UpdateDateTime,
            Mappers = provider.Mappers == null ? new List<SugarAuthenticationSchemeProviderMapper>() : provider.Mappers.Select(m => SugarAuthenticationSchemeProviderMapper.Transform(m)).ToList(),
            Realms = provider.Realms == null ? new List<SugarRealm>() : provider.Realms.Select(r => new SugarRealm
            {
                RealmsName = r.Name
            }).ToList()
        };
    }

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
            Realms = Realms == null ? Realms.Select(r => r.ToDomain()).ToList() : new List<Realm>(),
            Mappers = Mappers == null ? new List<AuthenticationSchemeProviderMapper>() : Mappers.Select(m => m.ToDomain()).ToList()
        };
    }
}
