// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.IdServer.Domains;
using SqlSugar;

namespace SimpleIdServer.IdServer.Store.SqlSugar.Models;

[SugarTable("UserSession")]
public class SugarUserSession
{
    [SugarColumn(IsPrimaryKey = true)]
    public string SessionId { get; set; } = null!;
    public DateTime AuthenticationDateTime { get; set; }
    public DateTime ExpirationDateTime { get; set; }
    public UserSessionStates State { get; set; }
    public string Realm { get; set; }
    public bool IsClientsNotified { get; set; } = false;
    public string SerializedClientIds { get; set; } = string.Empty;
    public string UserId { get; set; } = null!;

    public UserSession ToDomain()
    {
        return new UserSession
        {
            SessionId = SessionId,
            UserId = UserId,
            AuthenticationDateTime = AuthenticationDateTime,
            ExpirationDateTime = ExpirationDateTime,
            State = State,
            Realm = Realm,
            IsClientsNotified = IsClientsNotified,
            SerializedClientIds = SerializedClientIds
        };
    }
}
