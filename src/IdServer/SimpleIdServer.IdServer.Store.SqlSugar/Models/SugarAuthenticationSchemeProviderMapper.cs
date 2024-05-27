// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;
using SqlSugar;

namespace SimpleIdServer.IdServer.Store.SqlSugar.Models;

[SugarTable("AuthenticationSchemeProviderMapper")]
public class SugarAuthenticationSchemeProviderMapper
{
    [SugarColumn(IsPrimaryKey = true)]
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? SourceClaimName { get; set; } = null;
    public MappingRuleTypes MapperType { get; set; }
    public string? TargetUserAttribute { get; set; } = null;
    public string? TargetUserProperty { get; set; } = null;
    public string IdProviderId { get; set; } = null!;
    [Navigate(NavigateType.ManyToOne, nameof(IdProviderId))]
    public AuthenticationSchemeProvider IdProvider { get; set; } = null!;

    public AuthenticationSchemeProviderMapper ToDomain()
    {
        return new AuthenticationSchemeProviderMapper
        {
            Id = Id,
            Name = Name,
            SourceClaimName = SourceClaimName,
            MapperType = MapperType,
            TargetUserAttribute = TargetUserAttribute,
            TargetUserProperty = TargetUserProperty
        };
    }
}