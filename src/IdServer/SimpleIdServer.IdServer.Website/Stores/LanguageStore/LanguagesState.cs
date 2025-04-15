// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Website.Stores.LanguageStore;

[FeatureState]
public record LanguagesState
{
    public LanguagesState() 
    {
        Languages = new List<Language>();
        IsLoading = true;
    }

    public LanguagesState(List<Language> languages, bool isLoading)
    {
        Languages = languages;
        IsLoading = isLoading;
    }

    public List<Language> Languages { get; set; }
    public bool IsLoading { get; set; } = true;

    public List<string> Codes
    {
        get
        {
            return Languages.Select(l => l.Code).ToList();
        }
    }
}