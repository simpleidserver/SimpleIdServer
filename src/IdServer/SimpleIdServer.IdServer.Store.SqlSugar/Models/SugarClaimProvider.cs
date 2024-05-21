// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;
using SqlSugar;

namespace SimpleIdServer.IdServer.Store.SqlSugar.Models;

[SugarTable("ClaimProviders")]
public class SugarClaimProvider
{
    [SugarColumn(IsPrimaryKey = true)]
    public string Id { get; set; } = null!;
    public string ProviderType { get; set; } = null!;
    public string ConnectionString { get; set; } = null!;
    public ClaimType ClaimType { get; set; }

    public ClaimProvider ToDomain()
    {
        return new ClaimProvider
        {
            Id = Id,
            ProviderType = ProviderType,
            ConnectionString = ConnectionString,
            ClaimType = ClaimType
        };
    }
}