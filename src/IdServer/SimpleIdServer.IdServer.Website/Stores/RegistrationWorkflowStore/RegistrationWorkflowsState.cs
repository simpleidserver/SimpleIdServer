// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using SimpleIdServer.IdServer.Api.RegistrationWorkflows;

namespace SimpleIdServer.IdServer.Website.Stores.RegistrationWorkflowStore;

[FeatureState]
public record RegistrationWorkflowsState
{
    public RegistrationWorkflowsState()
    {
            
    }

    public RegistrationWorkflowsState(bool isLoading, ICollection<RegistrationWorkflowResult> registrationWorkflows)
    {
        IsLoading = isLoading;
        RegistrationWorkflows = registrationWorkflows.Select(r => new SelectableRegistrationWorkflow(r, false)).ToList();
        Count = RegistrationWorkflows.Count();
    }

    public bool IsLoading { get; set; } = false;
    public List<SelectableRegistrationWorkflow> RegistrationWorkflows { get; set; } = new List<SelectableRegistrationWorkflow>();
    public int Count { get; set; }
}

public record SelectableRegistrationWorkflow
{
    public SelectableRegistrationWorkflow(RegistrationWorkflowResult registrationWorkflow, bool isSelected)
    {
        RegistrationWorkflow = registrationWorkflow;
        IsSelected = isSelected;
    }

    public RegistrationWorkflowResult RegistrationWorkflow { get; set; }
    public bool IsSelected { get; set; } = false;
    public bool IsNew { get; set; } = false;
}
