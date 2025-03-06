// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using Hangfire.Storage.Monitoring;

namespace SimpleIdServer.IdServer.Website.Stores.RecurringJobStore;

[FeatureState]
public record RecurringJobState
{
    public RecurringJobState()
    {
        
    }

    public string Id
    {
        get; set;
    }

    public List<StateHistoryDto> Histories
    {
        get; set;
    } = new List<StateHistoryDto>();

    public bool IsLoading
    {
        get; set;
    } = false;

    public int Count => Histories.Count;
}
