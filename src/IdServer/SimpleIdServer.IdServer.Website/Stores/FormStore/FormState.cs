// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Fluxor;
using FormBuilder.Models;

namespace SimpleIdServer.IdServer.Website.Stores.FormStore;

[FeatureState]
public record FormState
{
    public FormState()
    {
        
    }

    public bool IsLoading { get; set; }
    public FormRecord Form { get; set; }
}
