// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Website.Stores.ScopeStore
{
    [FeatureState]
    public record SearchScopesState
    {
        public SearchScopesState() { }

        public SearchScopesState(bool isLoading, IEnumerable<Scope> scopes)
        {
            Scopes = scopes.Select(c => new SelectableScope(c));
            Count = scopes.Count();
            IsLoading = isLoading;
        }

        public IEnumerable<SelectableScope>? Scopes { get; set; } = null;
        public int Count { get; set; } = 0;
        public bool IsLoading { get; set; } = true;
    }

    public class SelectableScope
    {
        public SelectableScope(Scope scope)
        {
            Value = scope;
        }

        public bool IsSelected { get; set; } = false;
        public bool IsNew { get; set; } = false;
        public Scope Value { get; set; }
    }
}
