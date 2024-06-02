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
    [Navigate(NavigateType.ManyToOne, nameof(UserId))]
    public SugarUser User { get; set; }

    public static SugarUserSession Transform(UserSession userSession)
    {
        return new SugarUserSession
        {
            AuthenticationDateTime = userSession.AuthenticationDateTime,
            ExpirationDateTime = userSession.ExpirationDateTime,
            IsClientsNotified = userSession.IsClientsNotified,
            SerializedClientIds = userSession.SerializedClientIds,
            SessionId = userSession.SessionId,
            State = userSession.State,
            UserId = userSession.UserId,
            Realm = userSession.Realm
        };
    }

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
            SerializedClientIds = SerializedClientIds,
            User = User?.ToDomain()
        };
    }
}
