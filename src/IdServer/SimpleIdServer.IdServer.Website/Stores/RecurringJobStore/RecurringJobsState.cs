// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Fluxor;
using SimpleIdServer.IdServer.Api.RecurringJobs;

namespace SimpleIdServer.IdServer.Website.Stores.RecurringJobStore;

[FeatureState]
public record RecurringJobsState
{
    public RecurringJobsState()
    {
        
    }

    public bool IsLoading
    {
        get; set;
    } = false;

    public List<RecurringJobResult> RecurringJobs
    {
        get; set;
    } = new List<RecurringJobResult>();

    public int Count => RecurringJobs.Count;
}