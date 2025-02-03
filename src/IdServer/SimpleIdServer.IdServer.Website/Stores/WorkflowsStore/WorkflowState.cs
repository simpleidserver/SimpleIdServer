// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Fluxor;
using FormBuilder.Models;

namespace SimpleIdServer.IdServer.Website.Stores.WorkflowsStore;

[FeatureState]
public record WorkflowState
{
    public WorkflowState()
    {
        
    }

    public bool IsLoading { get; set; }
    public WorkflowRecord Value { get; set; }
}
