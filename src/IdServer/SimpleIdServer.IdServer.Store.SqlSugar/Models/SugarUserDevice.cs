// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;
using SqlSugar;

namespace SimpleIdServer.IdServer.Store.SqlSugar.Models;

[SugarTable("UserDevice")]
public class SugarUserDevice
{
    [SugarColumn(IsPrimaryKey = true)]
    public string Id { get; set; } = null!;
    [SugarColumn(IsNullable = true)]
    public string? DeviceType { get; set; } = null;
    [SugarColumn(IsNullable = true)]
    public string? Model { get; set; } = null;
    [SugarColumn(IsNullable = true)]
    public string? Manufacturer { get; set; } = null;
    [SugarColumn(IsNullable = true)]
    public string? Name { get; set; } = null;
    [SugarColumn(IsNullable = true)]
    public string? Version { get; set; } = null;
    [SugarColumn(IsNullable = true)]
    public string? PushToken { get; set; } = null;
    [SugarColumn(IsNullable = true)]
    public string? PushType { get; set; } = null;
    public DateTime CreateDateTime { get; set; }
    public string UserId { get; set; } = null!;
    [Navigate(NavigateType.ManyToOne, nameof(UserId))]
    public SugarUser User { get; set; }

    public static SugarUserDevice Transform(UserDevice d)
    {
        return new SugarUserDevice
        {
            CreateDateTime = d.CreateDateTime,
            DeviceType = d.DeviceType,
            Id = d.Id,
            Manufacturer = d.Manufacturer,
            Model = d.Model,
            Name = d.Name,
            PushToken = d.PushToken,
            PushType = d.PushType,
            Version = d.Version
        };
    }

    public UserDevice ToDomain()
    {
        return new UserDevice
        {
            Id = Id,
            DeviceType = DeviceType,
            CreateDateTime = CreateDateTime,
            Manufacturer = Manufacturer,
            Model = Model,
            Name = Name,
            PushToken = PushToken,
            PushType = PushType,
            Version = Version
        };
    }
}
