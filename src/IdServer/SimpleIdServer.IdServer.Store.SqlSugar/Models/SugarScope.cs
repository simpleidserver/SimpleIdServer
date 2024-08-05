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
    [SugarColumn(IsNullable = true)]
    public string? Description { get; set; } = null;
    public bool IsExposedInConfigurationEdp { get; set; }
    public DateTime CreateDateTime { get; set; }
    public DateTime UpdateDateTime { get; set; }
    public ComponentActions? Action { get; set; }
    public string? Component { set; get; } = null;

    [Navigate(NavigateType.OneToMany, nameof(SugarScopeClaimMapper.ScopeId))]
    public List<SugarScopeClaimMapper> ClaimMappers { get; set; }
    [Navigate(typeof(SugarApiResourceScope), nameof(SugarApiResourceScope.ScopesId), nameof(SugarApiResourceScope.ApiResourcesId))]
    public List<SugarApiResource> ApiResources { get; set; }
    [Navigate(typeof(SugarScopeRealm), nameof(SugarScopeRealm.ScopesId), nameof(SugarScopeRealm.RealmsName))]
    public List<SugarRealm> Realms { get; set; }

    public static SugarScope Transform(Scope scope)
    {
        return new SugarScope
        {
            ScopesId = scope.Id,
            Name = scope.Name,
            CreateDateTime = scope.CreateDateTime,
            Description = scope.Description,
            IsExposedInConfigurationEdp = scope.IsExposedInConfigurationEdp,
            Type = scope.Type,
            UpdateDateTime = scope.UpdateDateTime,
            Component = scope.Component,
            Action = scope.Action,
            Protocol = scope.Protocol,
            Realms = scope.Realms == null ? new List<SugarRealm>() : scope.Realms.Select(r => new SugarRealm
            {
                RealmsName = r.Name
            }).ToList(),
            ApiResources = scope.ApiResources == null ? new List<SugarApiResource>() : scope.ApiResources.Select(c => SugarApiResource.Transform(c)).ToList(),
            ClaimMappers = scope.ClaimMappers == null ? new List<SugarScopeClaimMapper>() : scope.ClaimMappers.Select(c => SugarScopeClaimMapper.Transform(c)).ToList()
        };
    }

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
            Component = Component,
            Action = Action,
            ClaimMappers = ClaimMappers == null ? new List<ScopeClaimMapper>() : ClaimMappers.Select(m => m.ToDomain()).ToList(),
            Realms = Realms == null ? new List<Realm>() : Realms.Select(r => r.ToDomain()).ToList(),
            ApiResources = ApiResources == null ? new List<ApiResource>() : ApiResources.Select(r => r.ToDomain()).ToList()
        };
    }
}