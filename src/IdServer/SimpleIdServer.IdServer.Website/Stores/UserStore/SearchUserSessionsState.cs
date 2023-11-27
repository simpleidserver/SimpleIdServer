// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Website.Stores.UserStore;

[FeatureState]
public record SearchUserSessionsState
{
    public SearchUserSessionsState() { }

    public SearchUserSessionsState(bool isLoading, IEnumerable<UserSession> userSessions)
    {
        UserSessions = userSessions;
        Count = userSessions.Count();
        IsLoading = isLoading;
    }

    public IEnumerable<UserSession>? UserSessions { get; set; } = null;
    public int Count { get; set; } = 0;
    public bool IsLoading { get; set; } = true;
}