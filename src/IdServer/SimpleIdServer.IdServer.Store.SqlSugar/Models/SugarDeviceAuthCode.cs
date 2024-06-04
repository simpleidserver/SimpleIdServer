// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using SimpleIdServer.IdServer.Domains;
using SqlSugar;

namespace SimpleIdServer.IdServer.Store.SqlSugar.Models;

[SugarTable("DeviceAuthCodes")]
public class SugarDeviceAuthCode
{
    [SugarColumn(IsPrimaryKey = true)]
    public string DeviceCode { get; set; } = null!;
    public string UserCode { get; set; } = null!;
    public string ClientId { get; set; } = null!;
    public string UserId { get; set; } = null;
    [SugarColumn(IsNullable = true)]
    public string? UserLogin { get; set; } = null;
    public string Scopes { get; set; } = null!;
    public DateTime CreateDateTime { get; set; }
    public DateTime UpdateDateTime { get; set; }
    public DateTime ExpirationDateTime { get; set; }
    [SugarColumn(IsNullable = true)]
    public DateTime? NextAccessDateTime { get; set; } = null;
    public DeviceAuthCodeStatus Status { get; set; }
    public DateTime LastAccessTime { get; set; }
    [Navigate(NavigateType.ManyToOne, nameof(ClientId))]
    public SugarClient Client { get; set; } = null!;
    [Navigate(NavigateType.ManyToOne, nameof(UserId))]
    public SugarUser? User { get; set; } = null;

    public DeviceAuthCode ToDomain()
    {
        return new DeviceAuthCode
        {
            ClientId = ClientId,
            DeviceCode = DeviceCode,
            UserCode = UserCode,
            UserLogin = UserLogin,
            CreateDateTime = CreateDateTime,
            UpdateDateTime = UpdateDateTime,
            ExpirationDateTime = ExpirationDateTime,
            NextAccessDateTime = NextAccessDateTime,
            Status = Status,
            LastAccessTime = LastAccessTime,
            Scopes = Scopes == null ? new List<string>() : Scopes.Split(',')
        };
    }
}
