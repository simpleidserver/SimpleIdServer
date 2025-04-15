// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using FormBuilder.Models;

namespace SimpleIdServer.IdServer.Website.Stores.TemplateStore;

[FeatureState]
public record TemplateState
{
    public TemplateState()
    {
        
    }

    public TemplateState(bool isLoading)
    {
        IsLoading = isLoading;
    }

    public bool IsLoading
    {
        get; set;
    } = true;

    public Template Template
    {
        get; set;
    }
}
