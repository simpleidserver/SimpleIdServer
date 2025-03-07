// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Fluxor;
using SimpleIdServer.IdServer.Api.RecurringJobs;

namespace SimpleIdServer.IdServer.Website.Stores.RecurringJobStore;

[FeatureState]
public record LastFailedJobsState
{
    public LastFailedJobsState()
    {
        
    }

    public bool IsLoading
    {
        get; set;
    }

    public List<FailedJobResult> FailedJobs
    {
        get; set;
    } = new List<FailedJobResult>();

    public int Count
    {
        get; set;
    }
}
