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
    public string? DeviceType { get; set; } = null;
    public string? Model { get; set; } = null;
    public string? Manufacturer { get; set; } = null;
    public string? Name { get; set; } = null;
    public string? Version { get; set; } = null;
    public string? PushToken { get; set; } = null;
    public string? PushType { get; set; } = null;
    public DateTime CreateDateTime { get; set; }
    public string UserId { get; set; } = null!;
    public User User { get; set; }

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
