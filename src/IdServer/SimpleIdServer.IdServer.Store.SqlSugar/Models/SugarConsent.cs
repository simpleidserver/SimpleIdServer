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
    [Navigate(NavigateType.ManyToOne, nameof(UserId))]
    public SugarUser User { get; set; }
    [Navigate(NavigateType.OneToMany, nameof(SugarAuthorizedScope.ConsentId))]
    public List<SugarAuthorizedScope> Scopes { get; set; }

    public static SugarConsent Transform(Consent c)
    {
        return new SugarConsent
        {
            Claims = c.Claims == null ? string.Empty : string.Join(",", c.Claims),
            UpdateDateTime = c.UpdateDateTime,
            Status = c.Status,
            Realm = c.Realm,
            SerializedAuthorizationDetails = c.SerializedAuthorizationDetails,
            CreateDateTime = c.CreateDateTime,
            ClientId = c.ClientId,
            Scopes = c.Scopes == null ? new List<SugarAuthorizedScope>() : c.Scopes.Select(s => SugarAuthorizedScope.Transform(s)).ToList()
        };
    }

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
            Realm = Realm,
            Scopes = Scopes == null ? new List<AuthorizedScope>() : Scopes.Select(s => s.ToDomain()).ToList() 
        };
    }
}