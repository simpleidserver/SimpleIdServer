// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using SimpleIdServer.IdServer.Api.RegistrationWorkflows;

namespace SimpleIdServer.IdServer.Website.Stores.RegistrationWorkflowStore;

[FeatureState]
public record RegistrationWorkflowState
{
    public RegistrationWorkflowState()
    {
            
    }

    public RegistrationWorkflowState(RegistrationWorkflowResult value, bool isLoading)
    {

    }

    public RegistrationWorkflowResult Value { get; set; }
    public bool IsLoading { get; set; }
}
