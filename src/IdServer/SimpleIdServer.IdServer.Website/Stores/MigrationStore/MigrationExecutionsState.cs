// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Website.Stores.MigrationStore;

[FeatureState]
public record MigrationExecutionsState
{
    public bool IsLoading
    {
        get; set;
    } = true;

    public int Count
    {
        get
        {
            return Executions.Count;
        }
    }

    public List<MigrationExecution> Executions
    {
        get; set;
    } = new List<MigrationExecution>();
}
