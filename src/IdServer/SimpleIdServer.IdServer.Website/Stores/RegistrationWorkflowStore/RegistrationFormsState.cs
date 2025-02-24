// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using FormBuilder.Models;

namespace SimpleIdServer.IdServer.Website.Stores.RegistrationWorkflowStore;

[FeatureState]
public record RegistrationFormsState
{
    public RegistrationFormsState()
    {

    }

    public RegistrationFormsState(bool isLoading)
    {
        IsLoading = isLoading;
    }

    public RegistrationFormsState(List<FormRecord> formRecords)
    {
        IsLoading = false;
        FormRecords = formRecords;
    }

    public bool IsLoading { get; set; } = true;
    public List<FormRecord> FormRecords { get; set; }
    public int Count
    {
        get
        {
            return FormRecords?.Count() ?? 0;
        }
    }
}