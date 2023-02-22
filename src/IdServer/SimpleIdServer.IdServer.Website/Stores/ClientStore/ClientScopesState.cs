// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Website.Stores.ClientStore
{
    [FeatureState]
    public record ClientScopesState
    {
        public ClientScopesState() { }

        public ClientScopesState(bool isLoading, IEnumerable<Scope> scopes)
        {
            Scopes = scopes.Select(c => new SelectableClientScope(c));
            Count = scopes.Count();
            IsLoading = isLoading;
        }

        public IEnumerable<SelectableClientScope>? Scopes { get; set; } = new List<SelectableClientScope>();
        public int Count { get; set; } = 0;
        public bool IsLoading { get; set; } = false;
        public IEnumerable<EditableClientScope>? EditableScopes { get; set; } = new List<EditableClientScope>();
        public int EditableScopesCount { get; set; } = 0;
        public bool IsEditableScopesLoading { get; set; } = false;
    }

    public class SelectableClientScope
    {
        public SelectableClientScope(Scope scope)
        {
            Value = scope;
        }

        public bool IsSelected { get; set; } = false;
        public bool IsNew { get; set; } = false;
        public Scope Value { get; set; }
    }

    public class EditableClientScope
    {
        public EditableClientScope(Domains.Scope scope)
        {
            Value = scope;
        }

        public bool IsPresent { get; set; }
        public bool IsSelected { get; set; }
        public Domains.Scope Value { get; set; }
    }
}
