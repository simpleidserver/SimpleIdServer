// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using SqlSugar;

namespace SimpleIdServer.IdServer.Store.SqlSugar.Models;

[SugarTable("RealmUser")]
public class SugarRealmUser
{
    [SugarColumn(IsPrimaryKey = true)]
    public string UsersId { get; set; }
    [SugarColumn(IsPrimaryKey = true)]
    public string RealmsName {  get; set; }

    public static SugarRealmUser Transform(RealmUser user)
    {
        return new SugarRealmUser
        {
            RealmsName = user.RealmsName
        };
    }

    public RealmUser ToDomain()
    {
        return new RealmUser
        {
            UsersId = UsersId,
            RealmsName = RealmsName
        };
    }
}
