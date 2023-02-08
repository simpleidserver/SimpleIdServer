// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Website.Stores.ResourceStore
{
    [FeatureState]
    public record SearchResourcesState
    {
        public SearchResourcesState() { }

        public SearchResourcesState(bool isLoading, IEnumerable<Scope> scopes)
        {
            Scopes = scopes.Select(c => new SelectableResource(c));
            Count = scopes.Count();
            IsLoading = isLoading;
        }

        public IEnumerable<SelectableResource>? Scopes { get; set; } = null;
        public int Count { get; set; } = 0;
        public bool IsLoading { get; set; } = false;
    }

    public class SelectableResource
    {
        public SelectableResource(Scope scope)
        {
            Value = scope;
        }

        public bool IsSelected { get; set; } = false;
        public bool IsNew { get; set; } = false;
        public Scope Value { get; set; }
    }
}
