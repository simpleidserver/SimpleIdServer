// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;
using SqlSugar;

namespace SimpleIdServer.IdServer.Store.SqlSugar.Models;

[SugarTable("AuthorizedScope")]
public class SugarAuthorizedScope
{
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public int Id { get; set; }
    public string Scope { get; set; } = null!;
    public string ConsentId { get; set; } = null!;
    [Navigate(NavigateType.OneToMany, nameof(SugarAuthorizedResource.AuthorizedScopeId))]
    public List<SugarAuthorizedResource> AuthorizedResources { get; set; }

    public AuthorizedScope ToDomain()
    {
        return new AuthorizedScope
        {
            Scope = Scope,
            AuthorizedResources = AuthorizedResources.Select(r => r.ToDomain()).ToList()
        };
    }
}
