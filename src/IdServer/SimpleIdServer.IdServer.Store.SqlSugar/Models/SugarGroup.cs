// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;
using SqlSugar;

namespace SimpleIdServer.IdServer.Store.SqlSugar.Models;

[SugarTable("Groups")]
public class SugarGroup
{
    [SugarColumn(IsPrimaryKey = true)]
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string FullPath { get; set; } = null!;
    public string? Description { get; set; } = null;
    public DateTime CreateDateTime { get; set; }
    public DateTime UpdateDateTime { get; set; }
    public string? ParentGroupId { get; set; } = null;
    [Navigate(NavigateType.ManyToOne, nameof(ParentGroupId))]
    public SugarGroup? ParentGroup { get; set; } = null;
    [Navigate(NavigateType.OneToMany, nameof(ParentGroupId))]
    public List<SugarGroup> Children { get; set; }
    [Navigate(typeof(SugarGroupScope), nameof(SugarGroupScope.GroupsId), nameof(SugarGroupScope.RolesId))]
    public List<SugarScope> Roles { get; set; }
    [Navigate(NavigateType.OneToMany, nameof(SugarGroupUser.GroupsId))]
    public List<SugarGroupUser> Users { get; set; }
    [Navigate(NavigateType.OneToMany, nameof(SugarGroupRealm.GroupsId))]
    public List<SugarGroupRealm> Realms { get; set; }

    public Group ToDomain()
    {
        return new Group
        {
            Id = Id,
            Name = Name,
            FullPath = FullPath,
            Description = Description,
            CreateDateTime = CreateDateTime,
            UpdateDateTime = UpdateDateTime,
            ParentGroupId = ParentGroupId,
            ParentGroup = ParentGroup?.ToDomain(),
            Children = Children.Select(c => c.ToDomain()).ToList(),
            Users = Users.Select(u => u.ToDomain()).ToList(),
            Roles = Roles.Select(r => r.ToDomain()).ToList(),
            Realms = Realms.Select(r => r.ToDomain()).ToList(),
        };
    }
}