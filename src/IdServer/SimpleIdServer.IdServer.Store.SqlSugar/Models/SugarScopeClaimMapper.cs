// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;
using SqlSugar;

namespace SimpleIdServer.IdServer.Store.SqlSugar.Models;


[SugarTable("ScopeClaimMapper")]
public class SugarScopeClaimMapper
{
    [SugarColumn(IsPrimaryKey = true, ColumnName = "Id")]
    public string ScopeClaimMapperId { get; set; } = null!;
    public string Name { get; set; } = null!;
    public MappingRuleTypes MapperType { get; set; }
    [SugarColumn(IsNullable = true)]
    public string? SourceUserAttribute { get; set; } = null;
    [SugarColumn(IsNullable = true)]
    public string? SourceUserProperty { get; set; } = null;
    [SugarColumn(IsNullable = true)]
    public string? TargetClaimPath { get; set; } = null;
    public bool IncludeInAccessToken { get; set; } = false;
    [SugarColumn(IsNullable = true)]
    public string? SAMLAttributeName { get; set; } = null;
    [SugarColumn(IsNullable = true)]
    public TokenClaimJsonTypes? TokenClaimJsonType { get; set; } = null;
    public bool IsMultiValued { get; set; } = false;
    public string ScopeId { get; set; }

    public static SugarScopeClaimMapper Transform(ScopeClaimMapper c)
    {
        return new SugarScopeClaimMapper
        {
            IncludeInAccessToken = c.IncludeInAccessToken,
            IsMultiValued = c.IsMultiValued,
            MapperType = c.MapperType,
            Name = c.Name,
            SAMLAttributeName = c.SAMLAttributeName,
            ScopeClaimMapperId = c.Id,
            SourceUserAttribute = c.SourceUserAttribute,
            SourceUserProperty = c.SourceUserProperty,
            TargetClaimPath = c.TargetClaimPath,
            TokenClaimJsonType = c.TokenClaimJsonType
        };
    }

    public ScopeClaimMapper ToDomain()
    {
        return new ScopeClaimMapper
        {
            Id = ScopeClaimMapperId,
            Name = Name,
            MapperType = MapperType,
            SourceUserAttribute = SourceUserAttribute,
            SourceUserProperty = SourceUserProperty,
            TargetClaimPath = TargetClaimPath,
            IncludeInAccessToken = IncludeInAccessToken,
            SAMLAttributeName = SAMLAttributeName,
            TokenClaimJsonType = TokenClaimJsonType,
            IsMultiValued = IsMultiValued
        };
    }
}
