// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;

namespace SimpleIdServer.IdServer.Website.Stores.LanguageStore;

[FeatureState]
public record CurrentLanguageState
{
    public CurrentLanguageState()
    {
        
    }

    public string CurrentLanguage { get; set; } = SimpleIdServer.IdServer.Domains.Language.Default;
}
