// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using Hangfire.Storage.Monitoring;

namespace SimpleIdServer.IdServer.Website.Stores.RecurringJobStore;

[FeatureState]
public record HangfireServersState
{
    public HangfireServersState()
    {
        
    }

    public bool IsLoading { get; set; }

    public int Count => Servers.Count;

    public List<ServerDto> Servers
    {
        get; set;
    } = new List<ServerDto>();
}
