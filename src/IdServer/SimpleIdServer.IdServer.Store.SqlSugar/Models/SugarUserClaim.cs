// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using SqlSugar;

namespace SimpleIdServer.IdServer.Store.SqlSugar.Models;

[SugarTable("UserClaims")]
public class SugarUserClaim
{
    [SugarColumn(IsPrimaryKey = true)]
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Value { get; set; } = null!;
    public string? Type { get; set; } = null;
    public string? UserId { get; set; } = null;

    public static SugarUserClaim Transform(UserClaim cl)
    {
        return new SugarUserClaim
        {
            Id = cl.Id,
            Name = cl.Name,
            Type = cl.Type,
            Value = cl.Value,
        };
    }

    public UserClaim ToDomain()
    {
        return new UserClaim
        {
            Id = Id,
            Name = Name,
            Type = Type,
            Value = Value,
            UserId = UserId
        };
    }
}
