// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Website.Stores.ClientStore
{
    [FeatureState]
    public record ClientState
    {
        public ClientState() { }

        public ClientState(bool isLoading, IEnumerable<Client> clients)
        {
            Clients = clients;
            Count = clients.Count();
            IsLoading = isLoading;
        }

        public IEnumerable<Client>? Clients { get; set; } = null;
        public int Count { get; set; } = 0;
        public bool IsLoading { get; set; } = false;
    }
}
