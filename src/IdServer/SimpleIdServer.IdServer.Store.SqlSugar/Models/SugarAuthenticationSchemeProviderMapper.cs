// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;
using SqlSugar;
using static MassTransit.Logging.OperationName;

namespace SimpleIdServer.IdServer.Store.SqlSugar.Models;

[SugarTable("AuthenticationSchemeProviderMapper")]
public class SugarAuthenticationSchemeProviderMapper
{
    [SugarColumn(IsPrimaryKey = true)]
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    [SugarColumn(IsNullable = true)]
    public string? SourceClaimName { get; set; } = null;
    public MappingRuleTypes MapperType { get; set; }
    [SugarColumn(IsNullable = true)]
    public string? TargetUserAttribute { get; set; } = null;
    [SugarColumn(IsNullable = true)]
    public string? TargetUserProperty { get; set; } = null;
    public string IdProviderId { get; set; } = null!;
    [Navigate(NavigateType.ManyToOne, nameof(IdProviderId))]
    public AuthenticationSchemeProvider IdProvider { get; set; } = null!;

    public static SugarAuthenticationSchemeProviderMapper Transform(AuthenticationSchemeProviderMapper mapper)
    {
        return new SugarAuthenticationSchemeProviderMapper
        {
            Id = mapper.Id,
            Name = mapper.Name,
            SourceClaimName = mapper.SourceClaimName,
            MapperType = mapper.MapperType,
            TargetUserAttribute = mapper.TargetUserAttribute,
            TargetUserProperty = mapper.TargetUserProperty
        };
    }

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