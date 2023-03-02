// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Website.Stores.IdProviderStore
{
    [FeatureState]
    public record SearchIdProvidersState
    {
        public SearchIdProvidersState() { }

        public SearchIdProvidersState(ICollection<AuthenticationSchemeProvider> idProviders, bool isLoading)
        {
            IdProviders = idProviders.Select(i => new SelectableIdProvider(i));
            IsLoading = isLoading;
        }

        public IEnumerable<SelectableIdProvider> IdProviders { get; set; }
        public int Count { get; set; } = 0;
        public bool IsLoading { get; set; } = true;
    }

    public class SelectableIdProvider
    {
        public SelectableIdProvider(AuthenticationSchemeProvider idProvider)
        {
            Value = idProvider;
        }

        public bool IsSelected { get; set; } = false;
        public bool IsNew { get; set; } = false;
        public AuthenticationSchemeProvider Value { get; set; }
    }
}
