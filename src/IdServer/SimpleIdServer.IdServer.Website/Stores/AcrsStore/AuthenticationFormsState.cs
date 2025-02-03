// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using FormBuilder.Models;

namespace SimpleIdServer.IdServer.Website.Stores.AcrsStore;

[FeatureState]
public record AuthenticationFormsState
{
    public AuthenticationFormsState()
    {
        
    }

    public AuthenticationFormsState(bool isLoading)
    {
        IsLoading = isLoading;
    }

    public AuthenticationFormsState(List<FormRecord> formRecords)
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