// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Website.Stores.ClientStore
{
    [FeatureState]
    public record class ClientRolesState
    {
        public ClientRolesState()
        {

        }

        public ClientRolesState(IEnumerable<Scope> roles, bool isLoading)
        {
            Roles = roles.Select(r => new SelectableClientRole(r)).ToList();
            IsLoading = isLoading;
            Nb = Roles.Count();
        }

        public bool IsLoading { get; set; }
        public ICollection<SelectableClientRole> Roles { get; set; }
        public int Nb { get; set; }
    }

    public class SelectableClientRole
    {
        public SelectableClientRole(Scope value) 
        { 
            Value = value;
        }

        public bool IsSelected { get; set; }
        public bool IsNew { get; set; }
        public Scope Value { get; set; }
    }
}
