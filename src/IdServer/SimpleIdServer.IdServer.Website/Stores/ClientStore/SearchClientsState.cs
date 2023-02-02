// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using SimpleIdServer.IdServer.Domains;

namespace SimpleIdServer.IdServer.Website.Stores.ClientStore
{
    [FeatureState]
    public record SearchClientsState
    {
        public SearchClientsState() { }

        public SearchClientsState(bool isLoading, IEnumerable<Client> clients)
        {
            Clients = clients.Select(c => new SelectableClient(c));
            Count = clients.Count();
            IsLoading = isLoading;
        }

        public IEnumerable<SelectableClient>? Clients { get; set; } = null;
        public int Count { get; set; } = 0;
        public bool IsLoading { get; set; } = false;
        public bool HasLeastOneClientIsSelected
        {
            get
            {
                return Clients == null ? false : Clients.Any(c => c.IsSelected);
            }
        }
    }

    public class SelectableClient
    {
        public SelectableClient(Client client) 
        {
            Value = client;
        }

        public bool IsSelected { get; set; } = false;
        public bool IsNew { get; set; } = false;
        public Client Value { get; set; }
    }
}
