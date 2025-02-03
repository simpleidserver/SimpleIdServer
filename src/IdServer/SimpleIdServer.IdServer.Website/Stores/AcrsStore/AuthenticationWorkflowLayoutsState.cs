// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using FormBuilder.Models.Layout;

namespace SimpleIdServer.IdServer.Website.Stores.AcrsStore;

[FeatureState]
public record AuthenticationWorkflowLayoutsState
{
    public AuthenticationWorkflowLayoutsState()
    {

    }

    public bool IsLoading { get; set; }
    public List<WorkflowLayout> Values { get; set; }
}
