// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Website.Stores.ResourceStore
{
    [FeatureState]
    public record ResourceState
    {
        public ResourceState() { }

        public ResourceState(bool isLoading, Scope? resource)
        {
            IsLoading = isLoading;
            Resource = resource;
        }

        public Scope? Resource { get; set; } = new Scope();
        public bool IsLoading { get; set; } = true;
    }
}
