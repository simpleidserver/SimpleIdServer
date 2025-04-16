// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using FormBuilder.Models;

namespace SimpleIdServer.IdServer.Website.Stores.TemplateStore;

[FeatureState]
public record TemplatesState
{
    public TemplatesState()
    {
        
    }

    public TemplatesState(bool isLoading)
    {
        IsLoading = isLoading;
    }

    public List<Template> Templates
    {
        get; set;
    } = new List<Template>();

    public bool IsLoading
    {
        get; set;
    } = false;
}
