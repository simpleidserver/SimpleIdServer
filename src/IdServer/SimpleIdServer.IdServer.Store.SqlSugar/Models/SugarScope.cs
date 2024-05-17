// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using SqlSugar;

namespace SimpleIdServer.IdServer.Store.SqlSugar.Models;

[SugarTable("Scopes")]
public class SugarScope
{
    [SugarColumn(IsPrimaryKey = true, ColumnName = "Id")]
    public string ScopesId { get; set; }
    public string Name { get; set; } = null!;
    public ScopeTypes Type { get; set; } = ScopeTypes.IDENTITY;
    public ScopeProtocols Protocol { get; set; } = ScopeProtocols.OPENID;
    public string? Description { get; set; } = null;
    public bool IsExposedInConfigurationEdp { get; set; }
    public DateTime CreateDateTime { get; set; }
    public DateTime UpdateDateTime { get; set; }

    [Navigate(NavigateType.OneToMany, nameof(SugarScopeClaimMapper.ScopeId))]
    public ICollection<SugarScopeClaimMapper> ClaimMappers { get; set; }
    [Navigate(typeof(SugarApiResourceScope), nameof(SugarApiResourceScope.ScopesId), nameof(SugarApiResourceScope.ApiResourcesId))]
    public List<SugarApiResource> ApiResources { get; set; }
    [Navigate(typeof(SugarScopeRealm), nameof(SugarScopeRealm.ScopesId), nameof(SugarScopeRealm.RealmsName))]
    public List<Realm> Realms { get; set; }

    public Scope ToDomain()
    {
        return new Scope
        {
            Id = ScopesId,
            Name = Name,
            Type = Type,
            Protocol = Protocol,
            Description = Description,
            IsExposedInConfigurationEdp = IsExposedInConfigurationEdp,
            CreateDateTime = CreateDateTime,
            UpdateDateTime = UpdateDateTime,
            ClaimMappers = ClaimMappers == null ? new List<ScopeClaimMapper>() : ClaimMappers.Select(m => m.ToDomain()).ToList()
        };
    }
}