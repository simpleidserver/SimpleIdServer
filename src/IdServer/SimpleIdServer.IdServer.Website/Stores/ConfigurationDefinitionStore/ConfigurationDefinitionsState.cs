// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using SimpleIdServer.Configuration.DTOs;

namespace SimpleIdServer.IdServer.Website.Stores.ConfigurationDefinitionStore;

[FeatureState]
public record ConfigurationDefinitionsState
{
    public ConfigurationDefinitionsState()
    {

    }

    public ConfigurationDefinitionsState(IEnumerable<ConfigurationDefResult> configurationDefs, bool isLoading)
    {
        ConfigurationDefs = configurationDefs;
        IsLoading = isLoading;
    }

    public IEnumerable<ConfigurationDefResult> ConfigurationDefs { get; set; } = new List<ConfigurationDefResult>();
    public bool IsLoading { get; set; } = true;
}
