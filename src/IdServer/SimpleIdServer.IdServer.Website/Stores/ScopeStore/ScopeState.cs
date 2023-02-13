// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Website.Stores.ScopeStore
{
    [FeatureState]
    public record ScopeState
    {
        public ScopeState() { }

        public ScopeState(bool isLoading, Scope? scope)
        {
            IsLoading = isLoading;
            Scope = scope;
        }

        public Scope? Scope { get; set; } = new Scope();
        public bool IsLoading { get; set; } = true;
    }
}
