// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;
using SqlSugar;

namespace SimpleIdServer.IdServer.Store.SqlSugar.Models;

[SugarTable("Grants")]
public class SugarConsent
{
    [SugarColumn(IsPrimaryKey = true)]
    public string Id { get; set; } = null!;
    public string ClientId { get; set; } = null!;
    public DateTime CreateDateTime { get; set; }
    public DateTime UpdateDateTime { get; set; }
    public string UserId { get; set; } = null!;
    public string? ScopeId { get; set; } = null;
    public ConsentStatus Status { get; set; }
    public string Realm { get; set; }
    public string Claims { get; set; } = null!;
    public string? SerializedAuthorizationDetails { get; set; } = null;
    public SugarUser User { get; set; }
    [Navigate(NavigateType.OneToMany, nameof(SugarAuthorizedScope.ConsentId))]
    public List<SugarAuthorizedScope> Scopes { get; set; }

    public Consent ToDomain()
    {
        return new Consent
        {
            Id = Id,
            ClientId = ClientId,
            CreateDateTime = CreateDateTime,
            UpdateDateTime = UpdateDateTime,
            SerializedAuthorizationDetails = SerializedAuthorizationDetails,
            Status = Status,
            Claims = Claims == null ? new List<string>() : Claims.Split(','),
        };
    }
}