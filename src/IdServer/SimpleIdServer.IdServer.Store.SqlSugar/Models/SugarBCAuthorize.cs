// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using SimpleIdServer.IdServer.Domains;
using SqlSugar;

namespace SimpleIdServer.IdServer.Store.SqlSugar.Models;

[SugarTable("BCAuthorizeLst")]
public class SugarBCAuthorize
{
    [SugarColumn(IsPrimaryKey = true)]
    public string Id { get; set; } = null!;
    public string? ClientId { get; set; } = null;
    public string? UserId { get; set; } = null;
    public string? NotificationToken { get; set; } = null;
    public string? NotificationMode { get; set; } = null;
    public string? NotificationEdp { get; set; } = null;
    public int? Interval { get; set; } = null;
    public BCAuthorizeStatus LastStatus { get; set; }
    public DateTime ExpirationDateTime { get; set; }
    public DateTime UpdateDateTime { get; set; }
    public DateTime? RejectionSentDateTime { get; set; }
    public DateTime? NextFetchTime { get; set; }
    public string Realm { get; set; } = null!;
    public string? SerializedAuthorizationDetails { get; set; } = null;
    public string Scopes { get; set; }
    [Navigate(NavigateType.OneToMany, nameof(SugarBCAuthorizeHistory.BCAuthorizeId))]
    public List<SugarBCAuthorizeHistory> Histories { get; set; }

    public BCAuthorize ToDomain()
    {
        return new BCAuthorize
        {
            Id = Id,
            ClientId = ClientId,
            UserId = UserId,
            NotificationToken = NotificationToken,
            NotificationMode = NotificationMode,
            NotificationEdp = NotificationEdp,
            Interval = Interval,
            LastStatus = LastStatus,
            ExpirationDateTime = ExpirationDateTime,
            UpdateDateTime = UpdateDateTime,
            RejectionSentDateTime = RejectionSentDateTime,
            NextFetchTime = NextFetchTime,
            Realm = Realm,
            SerializedAuthorizationDetails = SerializedAuthorizationDetails,
            Scopes = Scopes == null ? new List<string>() : Scopes.Split(','),
            Histories = Histories == null ? new List<BCAuthorizeHistory>() : Histories.Select(h => h.ToDomain()).ToList()
        };
    }
}
