// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;
using SqlSugar;

namespace SimpleIdServer.IdServer.Store.SqlSugar.Models;

[SugarTable("AuthorizedResource")]
public class SugarAuthorizedResource
{
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public int Id { get; set; }
    public string Resource { get; set; } = null!;
    public string? Audience { get; set; } = null;
    public string? AuthorizedScopeId { get; set; } = null;

    public static SugarAuthorizedResource Transform(AuthorizedResource a)
    {
        return new SugarAuthorizedResource
        {
            Audience = a.Audience,
            Resource = a.Resource
        };
    }

    public AuthorizedResource ToDomain()
    {
        return new AuthorizedResource
        {
            Audience = Audience,
            Resource = Resource
        };
    }
}
