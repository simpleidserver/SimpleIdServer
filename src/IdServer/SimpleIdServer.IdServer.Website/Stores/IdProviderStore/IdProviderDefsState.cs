// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Website.Stores.IdProviderStore
{
    [FeatureState]
    public record IdProviderDefsState
    {
        public IdProviderDefsState() { }

        public IdProviderDefsState(ICollection<AuthenticationSchemeProviderDefinition> authProviderDefinitions, bool isLoading)
        {
            AuthProviderDefinitions = authProviderDefinitions;
            IsLoading = isLoading;
        }

        public ICollection<AuthenticationSchemeProviderDefinition> AuthProviderDefinitions { get; set; } = new List<AuthenticationSchemeProviderDefinition>();
        public bool IsLoading { get; set; } = true;
    }
}
