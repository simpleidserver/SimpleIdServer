// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Website.Stores.IdProviderStore
{
    [FeatureState]
    public record IdProviderState
    {
        public IdProviderState() { }

        public IdProviderState(bool isLoading, AuthenticationSchemeProvider provider)
        {
            IsLoading = isLoading;
            Provider = provider;
        }

        public bool IsLoading { get; set; } = true;
        public AuthenticationSchemeProvider Provider {  get; set; }
    }
}
